using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Subscriber
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter the queue name: ");
            var queueName = Console.ReadLine();

            ConnectionFactory factory = new ConnectionFactory
            {
                HostName = "localhost",
                VirtualHost = "/",
                Port = 5672,
                UserName = "guest",
                Password = "guest"
            };

            IConnection connection = factory.CreateConnection();
            IModel channel = connection.CreateModel();

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, eventArgs) =>
            {
                var message = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
                Console.WriteLine($"Subscribe {queueName}, message: {message}");
            };

            var consumerTag = channel.BasicConsume(queueName, true, consumer);
            Console.WriteLine($"Subescribed to the {queueName}. Press a key to exit.");

            Console.ReadLine();
            channel.Close();
            connection.Close();
        }
    }
}
