using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Options;
using MQTTnet.Client.Subscribing;

namespace MqttMonitor.Core
{
    public class MqttClient
    {
        private readonly string _clientId;
        private readonly string _serverAddress;
        private readonly int _serverPort;
        private string _username = "";
        private string _password;
        private bool _useTLS = false;
        private IEnumerable<string> _topics;
        private ILogger _logger;
        private IMqttClient _client;

        public MqttClient(string clientId, string serverAddress, int serverPort, IEnumerable<string> subscribeTopics)
        {
            _clientId = clientId;
            _serverAddress = serverAddress;
            _serverPort = serverPort;
            _topics = subscribeTopics;
        }

        public event EventHandler<MqttMessageReceivedEventArgs> MessageReceived;


        public void SetCredentials(string username, string password)
        {
            _username = username;
            _password = password;
        }


        public void SetOptions(bool useTLS)
        {
            _useTLS = useTLS;
        }

        public void SetLogger(ILogger logger)
        {
            _logger = logger;
        }

        public async Task ConnectAsync()
        {
            try
            {
                IMqttClient mqttClient = CreateInternalClient();
                
                IMqttClientOptions options = CreateClientOptions();

                await mqttClient.ConnectAsync(options, CancellationToken.None);

                if (mqttClient.IsConnected)
                {
                    Log($"Connected to server {_serverAddress}:{_serverPort}");

                    foreach (string topic in _topics)
                    {
                        await mqttClient.SubscribeAsync(new MqttClientSubscribeOptionsBuilder().WithTopicFilter(topic).Build());
                        Log($"Subscribed to '{topic}");
                    }
                }          
            }
            catch (Exception ex) when (
                ex is MQTTnet.Adapter.MqttConnectingFailedException ||
                ex is MQTTnet.Exceptions.MqttCommunicationTimedOutException||
                ex is MQTTnet.Exceptions.MqttCommunicationException)
            {
                Log($"{ex.GetType().ToString()}: {ex.Message}");
                CleanUp();
            }

        }

        public async Task DisconnectAsync()
        {
            if (_client == null)
                return;

            await _client.DisconnectAsync();
            CleanUp();
        }

        private void OnMessageReceived(MqttApplicationMessageReceivedEventArgs e)
        {
            MqttMessageReceivedEventArgs msgArgs = CreateMessageReceivedEventArgs(e.ApplicationMessage.Topic, e.ApplicationMessage.Payload);

            MessageReceived?.Invoke(this, msgArgs);
        }

        protected MqttMessageReceivedEventArgs CreateMessageReceivedEventArgs(string topic, byte[] payload)
        {
            var newEventArgs = new MqttMessageReceivedEventArgs()
            {
                ReceivedTime = DateTime.UtcNow,
                Topic = topic,
                Payload = Encoding.UTF8.GetString(payload)
            };

            return newEventArgs;
        }

        private IMqttClient CreateInternalClient()
        {
            if (_client != null)
                return _client;

            var factory = new MqttFactory();
            _client = factory.CreateMqttClient();

            _client.UseDisconnectedHandler(e => Log("Disconnected from server"));

            Action<MqttApplicationMessageReceivedEventArgs> msgReceivedHandler = OnMessageReceived;
            _client.UseApplicationMessageReceivedHandler(msgReceivedHandler);

            return _client;
        }


        private IMqttClientOptions CreateClientOptions()
        {
            var tlsOptions = new MqttClientOptionsBuilderTlsParameters
            {
                AllowUntrustedCertificates = true,
                IgnoreCertificateChainErrors = true,
                IgnoreCertificateRevocationErrors = true,
                UseTls = _useTLS
            };

            IMqttClientOptions options = new MqttClientOptionsBuilder()
                .WithClientId(_clientId)
                .WithTcpServer(_serverAddress, _serverPort)
                .WithCredentials(_username, _password)
                .WithTls(tlsOptions)
                .Build();

            return options;
        }

        private void CleanUp()
        {
            _client?.Dispose();
            _client = null;
        }


        private void Log(string message)
        {
                _logger?.Log(message);
        }

    }
       
}
