using System;
using System.Text;
using Microsoft.AspNetCore.SignalR;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SignalRChat.Hubs;

namespace SignalRChat.Services
{
    public class RabbitMQService : IRabbitMQService
    {
        protected readonly ConnectionFactory _factory;
        protected readonly IConnection _connection;
        protected readonly IModel _channel;

        // protected readonly IServiceProvider _serviceProvider;

        private readonly IHubContext<ChatHub> _hubContext;

        public RabbitMQService(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
            // Opens the connections to RabbitMQ
            _factory = new ConnectionFactory() { HostName = "localhost" };
            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();
        }

        public virtual void Connect()
        {
            // Declare a RabbitMQ Queue
            _channel.QueueDeclare(queue: "TestQueue", durable: true, exclusive: false, autoDelete: false);

            var consumer = new EventingBasicConsumer(_channel);

            // When we receive a message from SignalR
            consumer.Received += delegate (object model, BasicDeliverEventArgs ea)
            {
                // Get the ChatHub from SignalR (using DI)

                var messageBody = ea.Body.ToArray();
                          var message = Encoding.UTF8.GetString(messageBody);

                string[] content =  message.Split('|');
                if(content.Length > 1)
                {
                    _hubContext.Clients.Client(content[0]).SendAsync("ReceiveMessage", "RabbitMQ", content[1]);
                }
                else{
                // Send message to all users in SignalR
                _hubContext.Clients.All.SendAsync("ReceiveMessage", "RabbitMQ", message);
                }

            };

            // Consume a RabbitMQ Queue
            _channel.BasicConsume(queue: "TestQueue", autoAck: true, consumer: consumer);
        }

    }
}
