namespace EasyNetQ.Migrations {
    using System;

    public interface IDelete {
        IExchangeDelete Exchange(String exchange);
        IExchangeDelete Exchange(Type messageType);
        IQueueDelete Queue(String queue);
        IQueueDelete Queue(Type messageType, String subscriptionId);
    }
}