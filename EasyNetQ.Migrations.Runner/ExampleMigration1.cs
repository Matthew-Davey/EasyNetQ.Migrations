namespace EasyNetQ.Migrations.Runner {
    [Migration(1)]
    public class ExampleMigration1 : Migration {
        public override void Apply() {
            Declare.VirtualHost("Test");

            Declare.Exchange("myExchange")
                .AsType(ExchangeType.Topic)
                .OnVirtualHost("Test")
                .Durable();

            Declare.Queue("myQueue")
                .OnVirtualHost("Test")
                .Durable();

            Declare.Binding()
                .OnVirtualHost("Test")
                .FromExchange("myExchange")
                .ToQueue("myQueue")
                .RoutingKey("#");
        }
    }
}