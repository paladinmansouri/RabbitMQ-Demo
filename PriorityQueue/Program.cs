using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace PriorityQueue
{
    class Program
    {
        private static IModel _channel;
        public static void Main()
        {
            ConnectionFactory factory = new ConnectionFactory
            {
                HostName = "localhost",
                VirtualHost = "/",
                Port = 5672,
                UserName = "guest",
                Password = "guest"
            };

            IConnection connection = factory.CreateConnection();
            _channel = connection.CreateModel();

            _channel.QueueDeclare("priority", true, false, false,
                new Dictionary<string, object> {{"x-max-priority", 10}});

            Console.WriteLine("Press any key to start to publish message");
            Console.ReadKey();
            
            SendMessage("Message 2", 2);
            SendMessage("Message 4", 4);
            SendMessage("Message 1",1);
            SendMessage("Message 3",3);
            SendMessage("Message 5",5);

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();

            _channel.QueueDelete("priority");
            _channel.Close();
            connection.Close();
        }

        private static void SendMessage(string message,byte priority)
        {
            var basicProperties = _channel.CreateBasicProperties();
            basicProperties.Priority = priority;
            _channel.BasicPublish("", "priority", true, basicProperties, 
                Encoding.UTF8.GetBytes(message));
        }
    }
}
