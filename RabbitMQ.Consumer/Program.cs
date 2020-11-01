namespace RabbitMQ.Consumer
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

            var rabbitMqClient = new Client<SampleMessage>
            (
                rabbitMQConfiguration.Hostname,
                rabbitMQConfiguration.VirtualHost,
                rabbitMQConfiguration.Username,
                rabbitMQConfiguration.Password,
                rabbitMQConfiguration.QueueName
            );

            while (true) //infinite loop
            {
                var message = rabbitMqClient.Get();
                Console.WriteLine($"Received message | {message.MessageId}: {message.Content}.");
                if (message.Equals("end.")) break;
            }

            Console.WriteLine("End consuming messages.");
        }
    }
}
