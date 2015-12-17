namespace EasyNetQ.Migrations.Runner {
    public class ExampleMigration2 : Migration {
        public override void Apply() {
            Delete.Queue("myQueue")
                .OnVirtualHost("Test");

            Delete.Exchange("myExchange")
                .OnVirtualHost("Test");

            Delete.VirtualHost("Test");
        }
    }
}