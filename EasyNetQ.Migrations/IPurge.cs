namespace EasyNetQ.Migrations {
    using System;

    public interface IPurge {
        IQueuePurge Queue(String queue);
        IQueuePurge Queue(Type messageType, String subscriptionId);
    }
}