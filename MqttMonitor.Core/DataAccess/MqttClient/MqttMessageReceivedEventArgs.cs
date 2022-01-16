using System;

namespace MqttMonitor.Core
{
    public class MqttMessageReceivedEventArgs : EventArgs
    {
        public DateTime ReceivedTime { get; set; }
        public string Topic { get; set; }
        public string Payload { get; set; }
    }
}

