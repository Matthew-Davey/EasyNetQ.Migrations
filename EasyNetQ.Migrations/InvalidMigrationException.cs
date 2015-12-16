namespace EasyNetQ.Migrations {
    using System;

    public class InvalidMigrationException : Exception {
        public InvalidMigrationException(String message) : base(message) {
        }
    }
}