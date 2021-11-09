using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace Services.DataCommon
{
    public sealed class MigrationsGenerator : SqlServerMigrationsSqlGenerator
    {
        public MigrationsGenerator(
            MigrationsSqlGeneratorDependencies dependencies,
            IMigrationsAnnotationProvider migrationsAnnotations)
            : base(dependencies, migrationsAnnotations)
        {
        }

        protected override void SequenceOptions(CreateSequenceOperation operation, IModel model, MigrationCommandListBuilder builder)
        {
            builder.AppendLine("CACHE 100");
        }

        protected override void Generate(
            MigrationOperation operation,
            IModel model,
            MigrationCommandListBuilder builder)
        {
            if (operation is CreateIndexOperation ||
                operation is DropIndexOperation ||
                operation is AddForeignKeyOperation ||
                operation is DropForeignKeyOperation ||
                operation is AlterDatabaseOperation)
            {
                return;
            }

            if (operation is CreateTableOperation createTable && createTable.ForeignKeys.Count > 0)
            {
                createTable.ForeignKeys.Clear();
            }

            base.Generate(operation, model, builder);
        }
    }
}
