namespace EasyNetQ.Migrations.Runner {
    [Migration(2)]
    public class ExampleMigration2 : Migration {
        public override void Apply() {
            Declare.Exchange("myExchange")
                .OnVirtualHost("Test")
                .AsType(ExchangeType.Topic)
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