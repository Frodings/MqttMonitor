using System;

namespace MqttMonitor
{
    public class Secret : ISecret
    {
        public string[] GetLoginCredentials()
        {
            throw new NotImplementedException();
        }
    }

}
