using Microsoft.EntityFrameworkCore.Migrations;

namespace Services.DataCommon.Extensions
{
    public static class MigrationBuilderExtensions
    {
        public static void SqlResources(this MigrationBuilder migrationBuilder, string resourceName, bool suppressTransaction = false)
        {
            migrationBuilder.Sql(ResourceHelper.GetSql(resourceName), suppressTransaction);
        }

        public static void AddUniqueConstraint(this MigrationBuilder migrationBuilder,
            string scheam, string table, string[] columns, bool suppressTransaction = false)
        {
            var columnUnderlineStr = string.Join("_", columns);
            var columnCamaStr = string.Join("],[", columns);
            migrationBuilder.Sql($"ALTER TABLE [{scheam}].[{table}] ADD CONSTRAINT [ck_{table}_{columnUnderlineStr}] UNIQUE ([{columnCamaStr}] );", suppressTransaction);
        }

        public static void AddColumnNotExists(this MigrationBuilder migrationBuilder, string scheam, string table,
            string colname, string datatype, string defaultValue, string comment = "", bool isnull = false, bool suppressTransaction = false)
        {
            migrationBuilder.Sql($@"if not exists(select 1 from sys.columns where name = N'{colname}' and object_id = object_id(N'[{scheam}].[{table}]')) begin ALTER TABLE [{scheam}].[{table}] Add [{colname}] {datatype} {(isnull ? "" : "not null")} CONSTRAINT DF_{table}_{colname} DEFAULT({defaultValue}) WITH VALUES;
  end;", suppressTransaction);

            migrationBuilder.Sql($@"if not exists(select 1 from sys.EXTENDED_PROPERTIES where [major_id] = object_id(N'[{scheam}].[{table}]') AND [name] = N'MS_Description'  and [value]=N'{comment}' ) begin EXEC sys.sp_addextendedproperty @name = N'MS_Description', @value = N'{comment}', @level0type = N'SCHEMA', @level0name = N'{scheam}', @level1type = N'TABLE', @level1name = N'{table}' , @level2type=N'COLUMN',@level2name=N'{colname}'; end else begin exec sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'{comment}' , @level0type=N'SCHEMA',@level0name=N'{scheam}', @level1type=N'TABLE',@level1name=N'{table}', @level2type=N'COLUMN',@level2name=N'{colname}'  end ;", suppressTransaction);
        }

        public static void AlterTableColDataTypeOnExists(this MigrationBuilder migrationBuilder, string scheam, string table,
            string colname, string datatype, bool isnull = false, bool suppressTransaction = false)
        {
            migrationBuilder.Sql($"if exists(select 1 from sys.columns where name = N'{colname}' and object_id = object_id(N'[{scheam}].[{table}]')) begin ALTER TABLE [{scheam}].[{table}] ALTER Column [{colname}] {datatype} {(isnull ? "" : "not null")} end;", suppressTransaction);
        }

        public static void AddColOnNotExists(this MigrationBuilder migrationBuilder, string scheam, string table,
            string colname, string datatype, string def = "", bool isnull = false, bool suppressTransaction = false)
        {
            migrationBuilder.Sql($"IF COL_LENGTH(N'{scheam}.{table}', N'{colname}') IS  NULL BEGIN ALTER TABLE [{scheam}].[{table}] ADD [{colname}] {datatype} {(isnull ? "" : "not null")}  {(string.IsNullOrEmpty(def) ? "" : $"default {def};")} end;", suppressTransaction);
        }

        public static void DropUniqueConstraintOnExists(this MigrationBuilder migrationBuilder, string scheam, string table, string[] columns, bool suppressTransaction = false)
        {
            var constraintName = $"ck_{table}_{string.Join("_", columns)}";
            migrationBuilder.Sql($"if exists (select 1 from sys.objects where type = 'UQ' and [name] = '{constraintName}') begin alter table [{scheam}].[{table}] drop constraint {constraintName} end;", suppressTransaction);
        }

        public static void DropColumnOnExists(this MigrationBuilder migrationBuilder,
            string schema, string table, string column, bool suppressTransaction = false)
        {
            migrationBuilder.Sql($"ALTER TABLE [{schema}].[{table}] DROP COLUMN IF EXISTS [{column}];", suppressTransaction);
        }

        public static void AddCheckConstraint(this MigrationBuilder migrationBuilder,
            string schema, string table, string column, string checkConstraint, bool suppressTransaction = false)
            => migrationBuilder.Sql($"ALTER TABLE [{schema}].[{table}] ADD CONSTRAINT [ck_{table}_{column}] CHECK ([{column}] {checkConstraint});", suppressTransaction);

        public static void EnableCheckConstraint(this MigrationBuilder migrationBuilder,
            string scheam, string table, string constraintName, bool suppressTransaction = false)
            => migrationBuilder.Sql($"ALTER TABLE [{scheam}].[{table}] WITH  CHECK CHECK CONSTRAINT [{constraintName}]", suppressTransaction);

        public static void DisableCheckConstraint(this MigrationBuilder migrationBuilder, string schema, string table, string constraintName, bool suppressTransaction = false)
            => migrationBuilder.Sql($"ALTER TABLE [{schema}].[{table}] NOCHECK CONSTRAINT [{constraintName}]", suppressTransaction);

        public static void DropStoreProcedureOnExists(this MigrationBuilder migrationBuilder, string scheam, string spName, bool suppressTransaction = false)
            => migrationBuilder.Sql($"if exists(select 1 from sys.objects where type='P' and name='{spName}') begin DROP PROCEDURE [{scheam}].[{spName}] end;", suppressTransaction);

        public static void DropSequenceOnExists(this MigrationBuilder migrationBuilder,
            string scheam, string seqName, bool suppressTransaction = false)
            => migrationBuilder.Sql($"if exists(select 1 from sys.objects where object_id = object_id('[{scheam}].[{seqName}]') and type = 'SO' and name='{seqName}') begin DROP SEQUENCE [{scheam}].[{seqName}]; end;", suppressTransaction);

        public static void DropTableTypeOnExists(this MigrationBuilder migrationBuilder,
            string scheam, string tableTypeName, bool suppressTransaction = false)
            => migrationBuilder.Sql($"if type_id('[{scheam}].[{tableTypeName}]') is not null begin DROP TYPE [{scheam}].[{tableTypeName}]; end;", suppressTransaction);

        public static void DropIndexOnExists(this MigrationBuilder migrationBuilder,
            string scheam, string indexName, string tableName, bool suppressTransaction = false)
            => migrationBuilder.Sql($"if exists (select 1 from sys.indexes where name = '{indexName}' and object_id = object_id('[{scheam}].[{tableName}]')) begin drop index [{indexName}] on [{scheam}].[{tableName}]; end;", suppressTransaction);

        public static void DropFunctionOnExists(this MigrationBuilder migrationBuilder,
            string scheam, string functionName, bool suppressTransaction = false)
            => migrationBuilder.Sql($"if exists(select * from sys.objects where  name='{functionName}' AND type in (N'FN', N'IF')) begin DROP FUNCTION [{scheam}].[{functionName}] end;", suppressTransaction);

        public static void DropTableOnExists(this MigrationBuilder migrationBuilder, string schema, string tableName, bool suppressTransaction = false)
            => migrationBuilder.Sql($"drop table  if exists [{schema}].[{tableName}]", suppressTransaction);

        public static void AddTemporalTableSupport(this MigrationBuilder migrationBuilder,
           string schema,
           string tableName,
           string historyTableSchema = "dbo",
           string historyTableName = "History",
           string retention = " 30 Days",
           bool enablConsistencyCheck = true)
        {
            migrationBuilder.Sql($@"ALTER TABLE [{schema}].[{tableName}] ADD
            SysStartTime datetime2(3) GENERATED ALWAYS AS ROW START HIDDEN NOT NULL,
            SysEndTime datetime2(3) GENERATED ALWAYS AS ROW END HIDDEN NOT NULL,
            PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime);");

            var consistencyCheck = string.Empty;
            if (enablConsistencyCheck)
            {
                consistencyCheck = " ,DATA_CONSISTENCY_CHECK = ON";
            }

            var newHistoryTableName = $"{tableName}History";
            if (historyTableName != "History")
            {
                newHistoryTableName = historyTableName;
            }

            migrationBuilder.Sql($@"ALTER TABLE [{schema}].[{tableName}]
SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [{historyTableSchema}].[{newHistoryTableName}],HISTORY_RETENTION_PERIOD = {retention} {consistencyCheck}));");
        }

        public static void DropTemporalTable(this MigrationBuilder migrationBuilder,
            string schema,
            string tableName,
            string historyTableSchema = "dbo",
            string historyTableName = "History",
            bool dropHistoryTable = true)
        {
            migrationBuilder.Sql($@"ALTER TABLE [{schema}].[{tableName}] SET (SYSTEM_VERSIONING = OFF);");

            migrationBuilder.Sql($@"ALTER TABLE [{schema}].[{tableName}] DROP PERIOD FOR SYSTEM_TIME;");

            migrationBuilder.Sql($@"ALTER TABLE [{schema}].[{tableName}] DROP Column SysStartTime;");

            migrationBuilder.Sql($@"ALTER TABLE [{schema}].[{tableName}] DROP Column SysEndTime;");

            if (dropHistoryTable)
            {
                var newHistoryTableName = $"{tableName}History";
                if (historyTableName != "History")
                {
                    newHistoryTableName = historyTableName;
                }

                migrationBuilder.Sql($@"DROP TABLE IF EXISTS [{historyTableSchema}].[{newHistoryTableName}];");
            }
        }
    }
}
