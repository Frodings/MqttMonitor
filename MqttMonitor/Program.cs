using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MqttMonitor.Core;
using MqttMonitor.DataAccess;

namespace MqttMonitor
{
    
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

            IMqttService mqttService = CreateMqttService(mqttServer, mqttPort, username, password);
            mqttService.SetLogger(new ConsoleLogger());
            mqttService.MessageReceived += MqttService_MessageReceived;

            RunAsync(mqttService).GetAwaiter().GetResult();
        }

        static async Task RunAsync(IMqttService service)
        {
           await service.ConnectAsync();

           Console.ReadLine();

           await service.DisconnectAsync();            
        }

        private static IMqttService CreateMqttService(string server, int port, string user, string pw)
        {
            List<string> topics = new List<string>()
            {
                "$SYS/broker/uptime",
                "$SYS/broker/load/messages/received/1min",
                "$SYS/broker/load/publish/sent/1min"
            };

            IMqttService client = new MqttService("mqttMonitor", server, port, topics);
            client.SetCredentials(user, pw);
            client.SetOptions(useTLS: true);

            return client;
        }

        private static void MqttService_MessageReceived(object sender, MqttMessageReceivedEventArgs e)
        {
            Console.WriteLine(e.ReceivedTime.ToString("s"));
            Console.WriteLine(e.Topic);
            Console.WriteLine(e.Payload);
            Console.WriteLine();

        }

        private static string[] GetCredentials()
        {
            // temporary - placeholder until proper way to handle credentials is implemented
            return Environment.GetCommandLineArgs();
        }

    }

}
