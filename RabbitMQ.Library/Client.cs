namespace RabbitMQ.Library
{
    using System.Text;
    using System.Threading;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;
    using Newtonsoft.Json;

    public class Client<T>
    {
        private ConnectionFactory _factory = null;
        private IConnection _connection = null;
        private IModel _channel = null;
        private string _queueName = null;

        public Client(string hostname, string virtualhost, string username, string password, string queueName)
        {
            _factory = new ConnectionFactory() { HostName = hostname, VirtualHost = virtualhost, UserName = username, Password = password };
            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();
            _queueName = queueName;

            _channel.QueueDeclare
            (
                queue: _queueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );
        }

        ~Client()
        {
            _channel.Close();
            _channel.Dispose();

            _connection.Close();
            _connection.Dispose();
        }

        public void Put(T message)
        {
            var jsonMessage = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

            _channel.BasicPublish
            (
                exchange: "",
                routingKey: _queueName,
                basicProperties: null,
                body: jsonMessage
            );
        }

        public T Get()
        {
            var consumer = new EventingBasicConsumer(_channel);
            T returnMessage = default;

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.Span;
                returnMessage = JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(body));
            };

            _channel.BasicConsume(queue: _queueName,
                                 autoAck: true,
                                 consumer: consumer);

            while (returnMessage == null)
                Thread.Sleep(10);

            return returnMessage;
        }
    }
}
