namespace EasyNetQ.Migrations {
    using System;

    public interface IDelete {
        void VirtualHost(String name);
        IExchangeDelete Exchange(String exchange);
        IExchangeDelete Exchange(Type messageType);
        IQueueDelete Queue(String queue);
        IQueueDelete Queue(Type messageType, String subscriptionId);
    }
}