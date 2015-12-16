namespace EasyNetQ.Migrations.Runner {
    using System;
    using EasyNetQ.Management.Client;

    class Program {
        static void Main(String[] args) {
            var managementClient = new ManagementClient("http://localhost", "guest", "guest");

            var migration = new ExampleMigration();

            migration.Run(managementClient);
        }
    }
}
