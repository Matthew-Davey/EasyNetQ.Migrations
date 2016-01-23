using System;

namespace EasyNetQ.Migrations {
    public interface IQueuePurge {
        IQueuePurge OnVirtualHost(String virtualHost);
    }
}
