using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models.Entities;
using Services.DataCommon;

namespace Databases.TicketSystemContext.Configurations
{
    public sealed class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> entity)
        {
            entity.ToTable(nameof(User), "system")
                .HasKey(c => c.ID)
                .HasName($"PK_{nameof(User)}")
                .IsClustered();

            entity.Property(e => e.ID)
                .HasColumnName(nameof(User.ID))
                .HasColumnType(SqlDbTypes.Int)
                .ValueGeneratedOnAdd()
                .UseIdentityColumn(1,1)
                .HasComment("序號")
                .IsRequired();

            entity.Property(e => e.Account)
                .HasColumnName(nameof(User.Account))
                .HasColumnType(SqlDbTypes.Varchar(50))
                .HasDefaultValueSql("('')")
                .HasComment("帳號")
                .IsUnicode(false)
                .IsRequired();

            entity.Property(e => e.RoleID)
                 .HasColumnName(nameof(User.RoleID))
                 .HasColumnType(SqlDbTypes.Int)
                 .HasDefaultValueSql("(0)")
                 .HasComment("權限ID")
                 .IsRequired();

            entity.Property(e => e.CreatedAt)
                .HasColumnName(nameof(User.CreatedAt))
                .HasColumnType(SqlDbTypes.Datetime)
                .HasDefaultValueSql("getdate()")
                .HasComment("建立日期")
                .IsRequired();

            entity.Property(e => e.CreatedBy)
                .HasColumnName(nameof(User.CreatedBy))
                .HasColumnType(SqlDbTypes.Nvarchar(50))
                .HasDefaultValueSql("(N'SYSTEM')")
                .HasComment("建立者")
                .IsUnicode(true)
                .IsRequired();

            entity.Property(e => e.LastModifiedAt)
                .HasColumnName(nameof(User.LastModifiedAt))
                .HasColumnType(SqlDbTypes.Datetime)
                .HasDefaultValueSql("getdate()")
                .HasComment("修改日期")
                .IsRequired();

            entity.Property(e => e.LastModifiedBy)
                .HasColumnName(nameof(User.LastModifiedBy))
                .HasColumnType(SqlDbTypes.Nvarchar(50))
                .HasDefaultValueSql("(N'SYSTEM')")
                .HasComment("修改者")
                .IsUnicode(true)
                .IsRequired();
        }
    }
}
