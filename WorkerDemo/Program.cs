using System;
using System.Text;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace WorkerDemo
{
    class Program
    {
        static void Main()
        {
            //RoundRobin();
            //UnequalTask();
            UnAutomaticAct();
        }

        private static void UnAutomaticAct()
        {
            Console.WriteLine("Enter the for this worker");
            var workerName = Console.ReadLine();

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
            consumer.Received += (sender, eventArgs) =>
            {
                var message = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
                var durationInSecond = Int32.Parse(message);
                Console.WriteLine($"{workerName} DurationInSecond: {durationInSecond}");
                Thread.Sleep(durationInSecond * 1000);
                Console.WriteLine("Finished Task");
                channel.BasicAck(eventArgs.DeliveryTag,false);
            };

            var consumerTag = channel.BasicConsume("q1", false, consumer);

            Console.WriteLine("Waiting for message. Press a key to exit.");
            Console.ReadKey();

            channel.Close();
            connection.Close();
        }

        private static void UnequalTask()
        {
            Console.WriteLine("Enter the for this worker");
            var workerName = Console.ReadLine();

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
                var durationInSecond = Int32.Parse(message);
                Console.WriteLine($"{workerName} DurationInSecond: {durationInSecond}");
                Thread.Sleep(durationInSecond * 1000);
                Console.WriteLine("Finished Task");
            };

            var consumerTag = channel.BasicConsume("q1", true, consumer);

            Console.WriteLine("Waiting for message. Press a key to exit.");
            Console.ReadKey();

            channel.Close();
            connection.Close();
        }

        private static void RoundRobin()
        {
            Console.WriteLine("Enter the for this worker");
            var workerName = Console.ReadLine();

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
                Console.WriteLine($"{workerName} message: {message}");
            };

            var consumerTag = channel.BasicConsume("q1", true, consumer);

            Console.WriteLine("Waiting for message. Press a key to exit.");
            Console.ReadKey();

            channel.Close();
            connection.Close();
        }
    }
}
