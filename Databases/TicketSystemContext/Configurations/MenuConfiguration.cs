using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models.Entities;
using Services.DataCommon;

namespace Databases.TicketSystemContext.Configurations
{
    public sealed class MenuConfiguration : IEntityTypeConfiguration<Menu>
    {
        public void Configure(EntityTypeBuilder<Menu> entity)
        {
            entity.ToTable(nameof(Menu), "system")
                .HasKey(c => c.ID)
                .HasName($"PK_{nameof(Menu)}")
                .IsClustered();

            entity.Property(e => e.ID)
                .HasColumnName(nameof(Menu.ID))
                .HasColumnType(SqlDbTypes.Int)
                .ValueGeneratedOnAdd()
                .UseIdentityColumn(1,1)
                .HasComment("序號")
                .IsRequired();

            entity.Property(e => e.Name)
                .HasColumnName(nameof(Menu.Name))
                .HasColumnType(SqlDbTypes.Nvarchar(50))
                .HasDefaultValueSql("(N'')")
                .HasComment("名稱")
                .IsUnicode()
                .IsRequired();

            entity.Property(e => e.CreatedAt)
                .HasColumnName(nameof(Menu.CreatedAt))
                .HasColumnType(SqlDbTypes.Datetime)
                .HasDefaultValueSql("getdate()")
                .HasComment("建立日期")
                .IsRequired();

            entity.Property(e => e.CreatedBy)
                .HasColumnName(nameof(Menu.CreatedBy))
                .HasColumnType(SqlDbTypes.Nvarchar(50))
                .HasDefaultValueSql("(N'SYSTEM')")
                .HasComment("建立者")
                .IsUnicode(true)
                .IsRequired();

            entity.Property(e => e.LastModifiedAt)
                .HasColumnName(nameof(Menu.LastModifiedAt))
                .HasColumnType(SqlDbTypes.Datetime)
                .HasDefaultValueSql("getdate()")
                .HasComment("修改日期")
                .IsRequired();

            entity.Property(e => e.LastModifiedBy)
                .HasColumnName(nameof(Menu.LastModifiedBy))
                .HasColumnType(SqlDbTypes.Nvarchar(50))
                .HasDefaultValueSql("(N'SYSTEM')")
                .HasComment("修改者")
                .IsUnicode(true)
                .IsRequired();
        }
    }
}
