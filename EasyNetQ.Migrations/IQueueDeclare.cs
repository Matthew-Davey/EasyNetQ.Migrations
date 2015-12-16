namespace EasyNetQ.Migrations {
    using System;

    public interface IQueueDeclare {
        IQueueDeclare OnVirtualHost(String vhost);
        IQueueDeclare Durable();
        IQueueDeclare AutoDelete();
        IQueueDeclare MessageTTL(TimeSpan ttl);
        IQueueDeclare AutoExpire(TimeSpan autoExpire);
        IQueueDeclare MaxLength(Int64 maxLength);
        IQueueDeclare MaxLengthBytes(Int64 maxLength);
        IQueueDeclare DeadLetterExchange(String dlx);
        IQueueDeclare DeadLetterExchange(String dlx, String routingKey);
        IQueueDeclare MaximumPriority(Int32 maxPriority);
    }
}