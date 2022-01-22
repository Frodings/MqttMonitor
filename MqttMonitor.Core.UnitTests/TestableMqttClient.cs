using System.Collections.Generic;
using MqttMonitor.DataAccess;

namespace MqttMonitor.UnitTests
{
    public class TestableMqttClient : MqttService
    {
        public TestableMqttClient()
            : base("", "", 1883, null)
        {
        }

        public TestableMqttClient(string clientId, string serverAddress, int serverPort, IEnumerable<string> subscribeTopics)
            : base(clientId, serverAddress, serverPort, subscribeTopics)
        {
        }

        public MqttMessageReceivedEventArgs GetMessageReceivedEventArgs()
        {
            var randomDummyBytes = new byte[] { 2, 4, 6, 124, 3, 35 };
            MqttMessageReceivedEventArgs msgEventArgs = base.CreateMessageReceivedEventArgs("dummyTopic", randomDummyBytes);

            return msgEventArgs;
                
        }
    }
}
