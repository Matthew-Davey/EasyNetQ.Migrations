namespace EasyNetQ.Migrations.Runner {
    using System;
    using EasyNetQ.Management.Client;

    class Program {
        static void Main(String[] args) {
            var managementClient = new ManagementClient("http://localhost", "guest", "guest");

            var migration1 = new ExampleMigration1();
            var migration2 = new ExampleMigration2();

            migration1.Run(managementClient);

            Console.WriteLine("Press any key to continue");
            Console.Read();

            migration2.Run(managementClient);
        }
    }
}
