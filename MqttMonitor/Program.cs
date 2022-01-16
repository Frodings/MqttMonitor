using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MqttMonitor.Core;

namespace MqttMonitor
{
    //client
    class Program
    {
        static void Main(string[] args)
        {
            //get configuration
            string mqttServer = ConfigurationManager.AppSettings["MqttServerAddress"];
            int mqttPort = int.Parse(ConfigurationManager.AppSettings["MqttServerPort"]);
            
            string[] credentials = GetCredentials();
            string username = credentials[1];
            string password = credentials[2];

            MqttClient client = CreateMqttClient(mqttServer, mqttPort, username, password);
            client.SetLogger(new ConsoleLogger());
            client.MessageReceived += client_MessageReceived;

            RunAsync(client).GetAwaiter().GetResult();
        }

        static async Task RunAsync(MqttClient client)
        {
           await client.ConnectAsync();

           Console.ReadLine();

           await client.DisconnectAsync();            
        }

        private static MqttClient CreateMqttClient(string server, int port, string user, string pw)
        {
            List<string> topics = new List<string>()
            {
                "$SYS/broker/uptime",
                "$SYS/broker/load/messages/received/1min",
                "$SYS/broker/load/publish/sent/1min"
            };

            MqttClient client = new MqttClient("mqttMonitor", server, port, topics);
            client.SetCredentials(user, pw);
            client.SetOptions(useTLS: true);

            return client;
        }

        private static void client_MessageReceived(object sender, MqttMessageReceivedEventArgs e)
        {
            Console.WriteLine(e.ReceivedTime.ToString("s"));
            Console.WriteLine(e.Topic);
            Console.WriteLine(e.Payload);
            Console.WriteLine();

        }

        private static string[] GetCredentials()
        {
            // temporary - placeholder until proper way to store and handle credentials is implemented
            return Environment.GetCommandLineArgs();
        }

    }

}
