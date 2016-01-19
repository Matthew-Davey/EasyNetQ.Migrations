namespace EasyNetQ.Migrations {
    using System;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class MigrationAttribute : Attribute {
        public UInt64 Version { get; set; }

        public MigrationAttribute(UInt64 version) {
            Version = version;
        }
    }
}