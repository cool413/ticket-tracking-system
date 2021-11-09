namespace Services.DataCommon
{
    public static class SqlDbTypes
    {
        public static string Varchar(int maxLength) => $"varchar({maxLength})";

        public static string VarcharMax() => $"varchar(max)";

        public static string Nvarchar(int maxLength) => $"nvarchar({maxLength})";

        public static string NvarcharMax() => $"nvarchar(max)";

        public static string Char(int maxLength) => $"char({maxLength})";

        public static string Nchar(int maxLength) => $"nchar({maxLength})";

        public const string Bit = "bit";

        public const string Tinyint = "tinyint";
        public const string Smallint = "smallint";
        public const string Int = "int";
        public const string Bigint = "bigint";

        public static string Decimal(int precision, int scale) => $"decimal({precision},{scale})";

        public static string Float(int precision) => $"float({precision})";

        public const string Date = "date";
        public const string Datetime = "datetime";
        public const string Datetime2 = "datetime2";
        public const string Datetimeoffset = "datetimeoffset";
        public const string Timestamp = "timestamp";
        public const string Time = "time";
        public const string Uniqueidentifier = "uniqueidentifier";

        public static string Varbinary(int maxLength) => $"varbinary({maxLength})";

        public static string Binary(int maxLength) => $"binary({maxLength})";

        public const string Hierarchyid = "hierarchyid";

        public const string Amount = "decimal(19,4)";
        public const string CurrencyCode = "char(3)";
        public const string VarcharDatetimeoffset = "varchar(40)";
        public const string DisplayId = "varchar(20)";
        public const string AccountNumber = "varchar(25)";
    }
}
