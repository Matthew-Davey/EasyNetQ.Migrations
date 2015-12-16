namespace EasyNetQ.Migrations {
    using System;

    public interface IDelete {
        void User(String user);
        void Exchange(String exchange);
        void Exchange(Type messageType);
        void Queue(String queue);
        void Queue(Type messageType, String subscriptionId);
    }
}