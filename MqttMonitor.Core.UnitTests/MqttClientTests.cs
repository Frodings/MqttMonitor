using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using MqttMonitor.Core;

namespace MqttMonitor.Core.UnitTests
{
    [TestFixture]
    public class MqttClientTests
    {
        /// <summary>
        /// Tester data (pakket i MqttMessageReceivedEventArgs) som returneres fra event MessageReceived
        /// </summary>
        [Test]
        public void CreateMessageReceivedEventArgs_ReturnedEventArgs_ReceivedTimeIsUTC()
        {
            TestableMqttClient client = new TestableMqttClient();
            MqttMessageReceivedEventArgs msgEventArgs = client.GetMessageReceivedEventArgs();

            Assert.True(msgEventArgs.ReceivedTime.Kind == DateTimeKind.Utc);
            Assert.True(msgEventArgs.ReceivedTime == msgEventArgs.ReceivedTime.ToUniversalTime());
        }

    }
}
