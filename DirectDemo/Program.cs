using System;
using System.Text;
using RabbitMQ.Client;

namespace DirectDemo
{
    class Program
    {
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
            IModel channel = connection.CreateModel();
            channel.ExchangeDeclare("ex.direct", "direct", true);

            channel.QueueDeclare("my.queue1", true, false, false, null);
            channel.QueueDeclare("my.queue2", true, false, false, null);

            channel.QueueBind("my.queue1", "ex.direct", "queue1", null);
            channel.QueueBind("my.queue2", "ex.direct", "queue2", null);

            channel.BasicPublish("ex.direct", "queue1",
                null, Encoding.UTF8.GetBytes("Message 1"));
            channel.BasicPublish("ex.direct", "queue2",
                null, Encoding.UTF8.GetBytes("Message 2"));

            Console.WriteLine("press any key to exit.");
            Console.ReadKey();

            channel.QueueDelete("my.queue1");
            channel.QueueDelete("my.queue2");
            channel.ExchangeDelete("ex.direct");

            channel.Close();
            connection.Close();
        }
    }
}
