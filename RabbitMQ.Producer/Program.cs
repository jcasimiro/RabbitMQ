namespace RabbitMQ.Producer
{
    using System;
    using RabbitMQ.Library;
    using Microsoft.Extensions.Configuration;
    using System.Threading;
    using System.Threading.Tasks;

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

            var random = new Random();

            Parallel.For(0, 100, i => {

                int sleepTime = 50 * random.Next(1, 10);
                
                Thread.Sleep(sleepTime);

                rabbitMqClient.Put
                (
                    new SampleMessage
                    {
                        Content = $"Test message with sleep time: {sleepTime}ms."
                    }
                 );
            });

            rabbitMqClient.Put
            (
                new SampleMessage
                {
                    Content = "end."
                }
             );

            Console.WriteLine("End producing messages.");
        }
    }
}
