using System;
using System.Text;
using RabbitMQ.Client;

namespace Publisher
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

            while (true)
            {
                Console.Write("Enter the message: ");
                var message = Console.ReadLine();

                if (message == "exit")
                    break;

                if (message != null)
                    channel.BasicPublish("ex.fanout", "", null,
                        Encoding.UTF8.GetBytes(message));
            }

            channel.Close();
            connection.Close();
        }
    }
}
