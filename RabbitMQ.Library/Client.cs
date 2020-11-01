namespace RabbitMQ.Library
{
    using System.Text;
    using Newtonsoft.Json;
    using RabbitMQ.Client;

    public class Client<T>
    {
        private readonly ConnectionFactory _factory = null;
        private readonly IConnection _connection = null;
        private readonly IModel _channel = null;
        private readonly IBasicProperties _properties = null;
        private readonly string _queueName = null;

        public Client(string hostname, string virtualhost, string username, string password, string queueName)
        {
            _factory = new ConnectionFactory() { HostName = hostname, VirtualHost = virtualhost, UserName = username, Password = password };
            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();
            _queueName = queueName;

            _properties = _channel.CreateBasicProperties();
            _properties.Persistent = true;

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
                basicProperties: _properties,
                body: jsonMessage
            );
        }

        public T Get()
        {       
            T returnMessage = default;
                
            var queueMessage = _channel.BasicGet(_queueName, false);

            if (queueMessage == null)
                return returnMessage;

            returnMessage = JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(queueMessage.Body.Span));
            
            _channel.BasicAck(queueMessage.DeliveryTag, false);

            return returnMessage;
        }
    }
}
