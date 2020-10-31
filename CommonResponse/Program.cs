using CommonCode;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace CommonResponse
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
                var messageData = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
                CalculationRequest request = JsonConvert.DeserializeObject<CalculationRequest>(messageData);

                Console.WriteLine($"Request received: {request}");

                CalculationResponse response = new CalculationResponse();

                if (request.OperationType == OperationType.Add)
                {
                    response.Result = request.Number1 + request.Number2;
                }
                else
                {
                    response.Result = request.Number1 - request.Number2;
                }

                var responseData = JsonConvert.SerializeObject(response);
                var basicProperties = channel.CreateBasicProperties();                
                basicProperties.Headers = new Dictionary<string, object>() {
                    { CommonCode.Constants.RequestIdHeaderKey ,
                        eventArgs.BasicProperties.Headers[CommonCode.Constants.RequestIdHeaderKey]}
                };

                channel.BasicPublish("", "response", basicProperties,
                    Encoding.UTF8.GetBytes(responseData));
            };

            channel.BasicConsume("request", true, consumer);

            Console.WriteLine("Press a key for exist");
            Console.ReadLine();

            channel.Close();
            connection.Close();
        }
    }
}
