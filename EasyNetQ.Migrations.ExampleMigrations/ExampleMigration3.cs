namespace EasyNetQ.Migrations.ExampleMigrations {
    [Migration(3)]
    public class ExampleMigration3 : Migration {
        public override void Apply() {
            Delete.Queue("myQueue")
                .OnVirtualHost("Test");

            Delete.Exchange("myExchange")
                .OnVirtualHost("Test");

            Delete.VirtualHost("Test");
        }
    }
}