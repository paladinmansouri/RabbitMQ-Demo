using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Response
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

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, eventArgs) =>
            {
                var message = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
                Console.WriteLine($"Message received: {message}");

                var response = $"Response for the {message}";
                channel.BasicPublish("","request",null,
                    Encoding.UTF8.GetBytes(response));
            };
 
            channel.BasicConsume("response", true, consumer);

            Console.WriteLine("Press a key for exist");
            Console.ReadLine();

            channel.Close();
            connection.Close();
        }
    }
}
