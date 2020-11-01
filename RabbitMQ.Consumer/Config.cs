namespace RabbitMQ.Consumer
{
    public class Config
    {
        public string Hostname { get; set; }
        public string VirtualHost { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string QueueName { get; set; }
    }
}
