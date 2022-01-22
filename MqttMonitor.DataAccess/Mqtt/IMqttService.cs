using System;
using System.Threading.Tasks;
using MqttMonitor.Core;

namespace MqttMonitor.DataAccess
{
    public interface IMqttService
    {
        event EventHandler<MqttMessageReceivedEventArgs> MessageReceived;

        Task ConnectAsync();
        Task DisconnectAsync();
        void SetCredentials(string username, string password);
        void SetLogger(ILogger logger);
        void SetOptions(bool useTLS);
    }
}