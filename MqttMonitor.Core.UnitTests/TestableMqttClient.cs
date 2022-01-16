using System.Collections.Generic;

namespace MqttMonitor.Core.UnitTests
{
    public class TestableMqttClient : MqttClient
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
