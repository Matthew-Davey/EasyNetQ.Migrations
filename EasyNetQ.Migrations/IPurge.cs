namespace EasyNetQ.Migrations {
    using System;

    public interface IPurge {
        void Queue(String queue);
        void Queue(Type messageType, String subscriptionId);
    }
}