using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace AlternateDemo
{
    class Program
    {
        static void Main(string[] args)
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
            channel.ExchangeDeclare("ex.fanout", "fanout", true);
            channel.ExchangeDeclare("ex.direct", "direct", true,false, new Dictionary<string, object>
            {
                {"alternate-exchange","ex.fanout"}
            });

            channel.QueueDeclare("my.queue1", true, false, false, null);
            channel.QueueDeclare("my.queue2", true, false, false, null);
            channel.QueueDeclare("my.queue3", true, false, false, null);

            channel.QueueBind("my.queue1", "ex.direct", "abc", null);
            channel.QueueBind("my.queue2", "ex.direct", "def", null);
            channel.QueueBind("my.queue3", "ex.fanout", "", null);

            channel.BasicPublish("ex.direct", "abc",
                null, Encoding.UTF8.GetBytes("Message abc"));
            channel.BasicPublish("ex.direct", "xyz",
                null, Encoding.UTF8.GetBytes("Message xyz"));

            Console.WriteLine("press any key to exit.");
            Console.ReadKey();

            channel.QueueDelete("my.queue1");
            channel.QueueDelete("my.queue2");
            channel.QueueDelete("my.queue3");
            channel.ExchangeDelete("ex.direct");
            channel.ExchangeDelete("ex.fanout");

            channel.Close();
            connection.Close();
        }
    }
}
