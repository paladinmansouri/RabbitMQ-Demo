using System;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;

namespace HeadersDemo
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
            channel.ExchangeDeclare("ex.headers", "headers", true);

            channel.QueueDeclare("my.queue1", true, false, false, null);
            channel.QueueDeclare("my.queue2", true, false, false, null);

            channel.QueueBind("my.queue1", "ex.headers", "", new Dictionary<string, object>()
            {
                {"x-match", "any"},
                {"job", "convert"},
                {"format", "jpeg"}
            });
            channel.QueueBind("my.queue2", "ex.headers", "", new Dictionary<string, object>
            {
                {"x-match", "all"},
                {"job", "convert"},
                {"format", "jpeg"}
            });

            IBasicProperties properties = channel.CreateBasicProperties();
            properties.Headers = new Dictionary<string, object> { { "job", "convert" }, { "format", "jpeg" } };
            channel.BasicPublish("ex.headers", "", properties, Encoding.UTF8.GetBytes("Message 1"));

            properties = channel.CreateBasicProperties();
            properties.Headers = new Dictionary<string, object> { { "job", "convert" }, { "format", "png" } };
            channel.BasicPublish("ex.headers", "", properties, Encoding.UTF8.GetBytes("Message 2"));

            Console.WriteLine("press any key to exit.");
            Console.ReadKey();

            channel.QueueDelete("my.queue1");
            channel.QueueDelete("my.queue2");
            channel.ExchangeDelete("ex.headers");

            channel.Close();
            connection.Close();
        }
    }
}
