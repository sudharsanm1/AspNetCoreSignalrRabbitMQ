using System;
using System.Text;
using RabbitMQ.Client;

namespace send
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost"  };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "TestQueue",
                    durable: true,
                    autoDelete: false,
                    exclusive: false,
                    arguments: null
                    );

                    string message = "Hello From Rabbit MQ Messager";
                    Console.WriteLine("Enter the Message to send with User - ");
                    message = Console.ReadLine();
                    var body = Encoding.UTF8.GetBytes(message);
                    channel.BasicPublish(exchange: "",
                    routingKey: "TestQueue",
                    basicProperties :null,
                    body: body);
                    

                    Console.WriteLine(" [x] sent {0}", message);
                }
            }
            Console.WriteLine("Hello World!");
        }
    }
}
