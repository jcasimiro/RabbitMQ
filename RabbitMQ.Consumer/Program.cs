namespace RabbitMQ.Consumer
{
    using System;
    using System.Threading;
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

            bool mainLoopActive = true;
            int messageCount = 0;

            while (mainLoopActive) //infinite loop
            {
                var message = rabbitMqClient.Get();
                
                if ( message != default(SampleMessage) )
                {
                    messageCount++;
                    Console.WriteLine($"Received message ({messageCount}) | {message.MessageId}: {message.Content}.");
                    if (message.Content.Equals("end.")) mainLoopActive = false;
                }

                Thread.Sleep(10);
            }

            Console.WriteLine("End consuming messages.");
        }
    }
}
