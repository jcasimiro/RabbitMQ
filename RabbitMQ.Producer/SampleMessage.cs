namespace RabbitMQ.Producer
{
    using System;

    public class SampleMessage
    {
        public SampleMessage()
        {
            MessageId = Guid.NewGuid();
        }

        public string Content { get; set; }
        public Guid MessageId { get; set; }

        public bool Equals(SampleMessage message)
        {
            return this.Content.Equals(message.Content) && this.MessageId.Equals(message.MessageId);
        }
    }
}