using System;
using System.Text;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Worker
{
    public class Program
    {
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
            IModel channel = connection.CreateModel();
            channel.BasicQos(0,1,false);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, args) =>
            {
                var message = Encoding.UTF8.GetString(args.Body.ToArray());
                Console.Write($"Message: {message} ");
                Thread.Sleep(2000);
                channel.BasicAck(args.DeliveryTag,false);
                Console.WriteLine("Finished!");
            };
            var consumeTag = channel.BasicConsume("priority", false, consumer);
            Console.WriteLine("Start to listening");
            Console.ReadKey();
        }
    }
}
