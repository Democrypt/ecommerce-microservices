using RabbitMQ.Client;
using System.Text;

namespace CatalogService.Messaging
{
    public class RabbitMqProducer
    {
        private readonly IConnection _connection;

        public RabbitMqProducer()
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost"
            };
            _connection = factory.CreateConnection();
        }

        public void SendMessage(string message)
        {
            using var channel = _connection.CreateModel();
            channel.QueueDeclare(queue: "catalogQueue",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: "",
                                 routingKey: "catalogQueue",
                                 basicProperties: null,
                                 body: body);
        }
    }
}
