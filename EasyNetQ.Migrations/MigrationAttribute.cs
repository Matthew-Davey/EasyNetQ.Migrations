namespace EasyNetQ.Migrations {
    using System;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class MigrationAttribute : Attribute {
        public Int64 Version { get; set; }

        public MigrationAttribute(Int64 version) {
            Version = version;
        }
    }
}