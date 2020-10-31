using System;
using System.Text;
using RabbitMQ.Client;

namespace TopicDemo
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
            channel.ExchangeDeclare("ex.topic", "topic", true);

            channel.QueueDeclare("my.queue1", true, false, false, null);
            channel.QueueDeclare("my.queue2", true, false, false, null);
            channel.QueueDeclare("my.queue3", true, false, false, null);

            channel.QueueBind("my.queue1", "ex.topic", "e.m", null);
            channel.QueueBind("my.queue2", "ex.topic", "e.c", null);
            channel.QueueBind("my.queue3", "ex.topic", "e.#", null);

            channel.BasicPublish("ex.topic", "e.m",
                null, Encoding.UTF8.GetBytes("Message 1"));
            channel.BasicPublish("ex.topic", "e.c",
                null, Encoding.UTF8.GetBytes("Message 2"));
            channel.BasicPublish("ex.topic", "e",
                null, Encoding.UTF8.GetBytes("Message 3"));

            Console.WriteLine("press any key to exit.");
            Console.ReadKey();

            channel.QueueDelete("my.queue1");
            channel.QueueDelete("my.queue2");
            channel.QueueDelete("my.queue3");
            channel.ExchangeDelete("ex.topic");

            channel.Close();
            connection.Close();
        }
    }
}
