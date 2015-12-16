namespace EasyNetQ.Migrations {
    using System;

    public interface IQueueDelete {
        IQueueDelete OnVirtualHost(String virtualHost);
    }
}