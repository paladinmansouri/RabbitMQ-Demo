using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Request
{
    class Program
    {
        static void Main()
        {
            //Create two queue with request and response
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
            };

            channel.BasicConsume("request", true, consumer);

            while (true)
            {
                Console.Write("Enter your request: ");
                var request = Console.ReadLine();

                if(request == "exit")
                    break;
                if (request != null)
                    channel.BasicPublish("", "response", null,
                        Encoding.UTF8.GetBytes(request));
            }
            
            channel.Close();
            connection.Close();
        }
    }
}
