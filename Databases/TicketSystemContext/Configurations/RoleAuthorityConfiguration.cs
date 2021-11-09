using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models.Entities;
using Services.DataCommon;

namespace Databases.TicketSystemContext.Configurations
{
    public sealed class RoleAuthorityConfiguration : IEntityTypeConfiguration<RoleAuthority>
    {
        public void Configure(EntityTypeBuilder<RoleAuthority> entity)
        {
            entity.ToTable(nameof(RoleAuthority), "system")
                .HasKey(c => c.ID)
                .HasName($"PK_{nameof(RoleAuthority)}")
                .IsClustered();
            
            entity.Property(e => e.ID)
                .HasColumnName(nameof(RoleAuthority.ID))
                .HasColumnType(SqlDbTypes.Int)
                .ValueGeneratedOnAdd()
                .UseIdentityColumn(1,1)
                .HasComment("序號")
                .IsRequired();

            entity.Property(e => e.RoleID)
                .HasColumnName(nameof(RoleAuthority.RoleID))
                .HasColumnType(SqlDbTypes.Int)
                .ValueGeneratedNever()
                .HasComment("權限ID")
                .IsRequired();

            entity.Property(e => e.MenuID)
                .HasColumnName(nameof(RoleAuthority.MenuID))
                .HasColumnType(SqlDbTypes.Int)
                .ValueGeneratedNever()
                .HasComment("菜單ID")
                .IsRequired();
            
            entity.Property(e => e.CanInsert)
                .HasColumnName(nameof(RoleAuthority.CanInsert))
                .HasColumnType(SqlDbTypes.Bit)
                .ValueGeneratedNever()
                .HasComment("是否可新增")
                .HasDefaultValueSql("(1)")
                .IsRequired();

            entity.Property(e => e.CanDelete)
                .HasColumnName(nameof(RoleAuthority.CanDelete))
                .HasColumnType(SqlDbTypes.Bit)
                .ValueGeneratedNever()
                .HasComment("是否可刪除")
                .HasDefaultValueSql("(1)")
                .IsRequired();
            
            entity.Property(e => e.CanUpdate)
                .HasColumnName(nameof(RoleAuthority.CanUpdate))
                .HasColumnType(SqlDbTypes.Bit)
                .ValueGeneratedNever()
                .HasComment("是否可修改")
                .HasDefaultValueSql("(1)")
                .IsRequired();
            
            entity.Property(e => e.CanRead)
                .HasColumnName(nameof(RoleAuthority.CanRead))
                .HasColumnType(SqlDbTypes.Bit)
                .ValueGeneratedNever()
                .HasComment("是否可讀")
                .HasDefaultValueSql("(1)")
                .IsRequired();
            
            entity.Property(e => e.CanResolve)
                .HasColumnName(nameof(RoleAuthority.CanResolve))
                .HasColumnType(SqlDbTypes.Bit)
                .ValueGeneratedNever()
                .HasComment("是否可解決")
                .HasDefaultValueSql("(1)")
                .IsRequired();
            
            entity.Property(e => e.CreatedAt)
                .HasColumnName(nameof(RoleAuthority.CreatedAt))
                .HasColumnType(SqlDbTypes.Datetime)
                .HasDefaultValueSql("getdate()")
                .HasComment("建立日期")
                .IsRequired();

            entity.Property(e => e.CreatedBy)
                .HasColumnName(nameof(RoleAuthority.CreatedBy))
                .HasColumnType(SqlDbTypes.Nvarchar(50))
                .HasDefaultValueSql("(N'SYSTEM')")
                .HasComment("建立者")
                .IsUnicode(true)
                .IsRequired();

            entity.Property(e => e.LastModifiedAt)
                .HasColumnName(nameof(RoleAuthority.LastModifiedAt))
                .HasColumnType(SqlDbTypes.Datetime)
                .HasDefaultValueSql("getdate()")
                .HasComment("修改日期")
                .IsRequired();

            entity.Property(e => e.LastModifiedBy)
                .HasColumnName(nameof(RoleAuthority.LastModifiedBy))
                .HasColumnType(SqlDbTypes.Nvarchar(50))
                .HasDefaultValueSql("(N'SYSTEM')")
                .HasComment("修改者")
                .IsUnicode(true)
                .IsRequired();
        }
    }
}
