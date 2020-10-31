using System;
using System.Text;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace PushPullDemo
{
    class Program
    {
        private static IModel _channel;
        static void Main()
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
            //ReadMessagesWithPushModel();
            ReadMessageWithPullModel();

            _channel.Close();
            connection.Close();
        }

        private static void ReadMessagesWithPushModel()
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (sender, e) =>
            {
                var message = Encoding.UTF8.GetString(e.Body.ToArray());
                Console.WriteLine($"Message: {message}");
            };
            var consumerTag = _channel.BasicConsume("q1", true, consumer);
            Console.WriteLine(consumerTag);

            Console.WriteLine("Subscribed. Press a key to unsubscribe and exit");
            Console.ReadKey();

            _channel.BasicCancel(consumerTag);

        }

        private static void ReadMessageWithPullModel()
        {
            Console.WriteLine("Reading message from queue. Press 'e' to exit.");

            while (true)
            {
                Console.WriteLine("Tryieng to get a message from the queue...");
                BasicGetResult result = _channel.BasicGet("q1", true);
                if (result != null)
                {
                    var message = Encoding.UTF8.GetString(result.Body.ToArray());
                    Console.WriteLine($"Message: {message}");
                }

                if (Console.KeyAvailable)
                {
                    var kayInfo = Console.ReadKey();
                    if (kayInfo.KeyChar == 'e' || kayInfo.KeyChar == 'E')
                    {
                        return;
                    }
                }

                Thread.Sleep(5000);
            }
        }
    }
}
