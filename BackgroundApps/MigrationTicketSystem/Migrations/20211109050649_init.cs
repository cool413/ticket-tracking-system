using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Services.DataCommon.Extensions;

namespace BackgroundApps.MigrationTicketSystem.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "system");

            migrationBuilder.CreateTable(
                name: "Menu",
                schema: "system",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false, comment: "序號")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", nullable: false, defaultValueSql: "(N'')", comment: "名稱"),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "getdate()", comment: "建立日期"),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", nullable: false, defaultValueSql: "(N'SYSTEM')", comment: "建立者"),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "getdate()", comment: "修改日期"),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(50)", nullable: false, defaultValueSql: "(N'SYSTEM')", comment: "修改者")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Menu", x => x.ID)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                schema: "system",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false, comment: "序號")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", nullable: false, defaultValueSql: "(N'')", comment: "名稱"),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "getdate()", comment: "建立日期"),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", nullable: false, defaultValueSql: "(N'SYSTEM')", comment: "建立者"),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "getdate()", comment: "修改日期"),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(50)", nullable: false, defaultValueSql: "(N'SYSTEM')", comment: "修改者")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.ID)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "RoleAuthority",
                schema: "system",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false, comment: "序號")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleID = table.Column<int>(type: "int", nullable: false, comment: "權限ID"),
                    MenuID = table.Column<int>(type: "int", nullable: false, comment: "菜單ID"),
                    CanInsert = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "(1)", comment: "是否可新增"),
                    CanDelete = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "(1)", comment: "是否可刪除"),
                    CanUpdate = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "(1)", comment: "是否可修改"),
                    CanRead = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "(1)", comment: "是否可讀"),
                    CanResolve = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "(1)", comment: "是否可解決"),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "getdate()", comment: "建立日期"),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", nullable: false, defaultValueSql: "(N'SYSTEM')", comment: "建立者"),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "getdate()", comment: "修改日期"),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(50)", nullable: false, defaultValueSql: "(N'SYSTEM')", comment: "修改者")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleAuthority", x => x.ID)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "Ticket",
                schema: "system",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false, comment: "序號")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<byte>(type: "tinyint", nullable: false, defaultValueSql: "(0)", comment: "類別: 1.bug單"),
                    Summary = table.Column<string>(type: "nvarchar(50)", nullable: false, defaultValueSql: "(N'')", comment: "摘要"),
                    Description = table.Column<string>(type: "nvarchar(500)", nullable: false, defaultValueSql: "(N'')", comment: "描述"),
                    UserID = table.Column<int>(type: "int", nullable: false, comment: "使用者ID"),
                    Status = table.Column<byte>(type: "tinyint", nullable: false, defaultValueSql: "(0)", comment: "狀態"),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "getdate()", comment: "建立日期"),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", nullable: false, defaultValueSql: "(N'SYSTEM')", comment: "建立者"),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "getdate()", comment: "修改日期"),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(50)", nullable: false, defaultValueSql: "(N'SYSTEM')", comment: "修改者")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ticket", x => x.ID)
                        .Annotation("SqlServer:Clustered", true);
                });

            migrationBuilder.CreateTable(
                name: "User",
                schema: "system",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false, comment: "序號")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Account = table.Column<string>(type: "varchar(50)", unicode: false, nullable: false, defaultValueSql: "('')", comment: "帳號"),
                    RoleID = table.Column<int>(type: "int", nullable: false, defaultValueSql: "(0)", comment: "權限ID"),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "getdate()", comment: "建立日期"),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", nullable: false, defaultValueSql: "(N'SYSTEM')", comment: "建立者"),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "getdate()", comment: "修改日期"),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(50)", nullable: false, defaultValueSql: "(N'SYSTEM')", comment: "修改者")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.ID)
                        .Annotation("SqlServer:Clustered", true);
                });
            
            
            migrationBuilder.SqlResources("Index.system.RoleAuthority.UIX_RoIeID_MenuID.sql");
            
            migrationBuilder.SqlResources("Seed.system.User.sql");
            migrationBuilder.SqlResources("Seed.system.Menu.sql");
            migrationBuilder.SqlResources("Seed.system.Role.sql");
            migrationBuilder.SqlResources("Seed.system.RoleAuthority.sql");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Menu",
                schema: "system");

            migrationBuilder.DropTable(
                name: "Role",
                schema: "system");

            migrationBuilder.DropTable(
                name: "RoleAuthority",
                schema: "system");

            migrationBuilder.DropTable(
                name: "Ticket",
                schema: "system");

            migrationBuilder.DropTable(
                name: "User",
                schema: "system");
        }
    }
}
