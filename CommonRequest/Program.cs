using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using CommonCode;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Constants = CommonCode.Constants;

namespace CommonRequest
{
    class Program
    {
        static void Main()
        {
            ConcurrentDictionary<string, CalculationRequest> waitingRequests =
                new ConcurrentDictionary<string, CalculationRequest>();

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

            channel.QueueDeclare("request", true, false, false, null);
            channel.QueueDeclare("response", true, false, false, null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, eventArgs) =>
            {
                var requestId = Encoding.UTF8.GetString(
                    (byte[]) eventArgs.BasicProperties.Headers[Constants.RequestIdHeaderKey]);
                if (waitingRequests.TryGetValue(requestId, out var request))
                {
                    var messageData = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
                    CalculationResponse response = JsonConvert.DeserializeObject<CalculationResponse>(messageData);

                    Console.WriteLine($"Calculation result: {request} = {response}");

                }
                var message = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
                Console.WriteLine($"Message received: {message}");
            };
            

            channel.BasicConsume("response", true, consumer);

            Console.WriteLine("Press a key to send requests");
            Console.ReadKey();

            SendRequest(waitingRequests, channel, new CalculationRequest(2, 4, OperationType.Add));
            SendRequest(waitingRequests, channel, new CalculationRequest(14, 6, OperationType.Subtract));
            SendRequest(waitingRequests, channel, new CalculationRequest(50, 2, OperationType.Add));
            SendRequest(waitingRequests, channel, new CalculationRequest(30, 6, OperationType.Subtract));

            Console.ReadKey();
            channel.Close();
            connection.Close();
        }

        private static void SendRequest(ConcurrentDictionary<string, CalculationRequest> waitingRequests,
            IModel channel,
            CalculationRequest request)
        {
            var requestId = new Guid().ToString();
            var requestData = JsonConvert.SerializeObject(request);

            waitingRequests[requestId] = request;
            var basicProperties = channel.CreateBasicProperties();
            basicProperties.Headers = new Dictionary<string, object>
            {
                {Constants.RequestIdHeaderKey, Encoding.UTF8.GetBytes(requestId)}
            };

            channel.BasicPublish("","request",basicProperties,Encoding.UTF8.GetBytes(requestData));
        }
    }
}
