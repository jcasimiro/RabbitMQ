namespace RabbitMQ_Samples
{
    using System;
    using RabbitMQ.Library;
    using Microsoft.Extensions.Configuration;

    class Program
    {
        static void Main(string[] args)
        {
            IConfiguration Configuration = new ConfigurationBuilder()
              .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
              .Build();

            var rabbitMQConfiguration = Configuration.GetSection("RabbitMQ").Get<Config>();

            var putMessage = new SampleMessage
            {
                Content = "Test message"
            };

            var rabbitMqClient = new Client<SampleMessage>
            (
                rabbitMQConfiguration.Hostname,
                rabbitMQConfiguration.VirtualHost,
                rabbitMQConfiguration.Username,
                rabbitMQConfiguration.Password,
                rabbitMQConfiguration.QueueName
            );

            rabbitMqClient.Put(putMessage);

            var getMessage = rabbitMqClient.Get();

            Console.WriteLine($"Same message: {getMessage.Equals(putMessage)}");
            Console.ReadLine();
        }
    }
}
