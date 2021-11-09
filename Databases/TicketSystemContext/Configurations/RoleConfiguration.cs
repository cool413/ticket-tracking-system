using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models.Entities;
using Services.DataCommon;

namespace Databases.TicketSystemContext.Configurations
{
    public sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> entity)
        {
            entity.ToTable(nameof(Role), "system")
                .HasKey(c => c.ID)
                .HasName($"PK_{nameof(Role)}")
                .IsClustered();

            entity.Property(e => e.ID)
                .HasColumnName(nameof(Role.ID))
                .HasColumnType(SqlDbTypes.Int)
                .ValueGeneratedOnAdd()
                .UseIdentityColumn(1,1)
                .HasComment("序號")
                .IsRequired();

            entity.Property(e => e.Name)
                .HasColumnName(nameof(Role.Name))
                .HasColumnType(SqlDbTypes.Nvarchar(50))
                .HasDefaultValueSql("(N'')")
                .HasComment("名稱")
                .IsUnicode()
                .IsRequired();

            
            entity.Property(e => e.CreatedAt)
                .HasColumnName(nameof(Role.CreatedAt))
                .HasColumnType(SqlDbTypes.Datetime)
                .HasDefaultValueSql("getdate()")
                .HasComment("建立日期")
                .IsRequired();

            entity.Property(e => e.CreatedBy)
                .HasColumnName(nameof(Role.CreatedBy))
                .HasColumnType(SqlDbTypes.Nvarchar(50))
                .HasDefaultValueSql("(N'SYSTEM')")
                .HasComment("建立者")
                .IsUnicode(true)
                .IsRequired();

            entity.Property(e => e.LastModifiedAt)
                .HasColumnName(nameof(Role.LastModifiedAt))
                .HasColumnType(SqlDbTypes.Datetime)
                .HasDefaultValueSql("getdate()")
                .HasComment("修改日期")
                .IsRequired();

            entity.Property(e => e.LastModifiedBy)
                .HasColumnName(nameof(Role.LastModifiedBy))
                .HasColumnType(SqlDbTypes.Nvarchar(50))
                .HasDefaultValueSql("(N'SYSTEM')")
                .HasComment("修改者")
                .IsUnicode(true)
                .IsRequired();
        }
    }
}
