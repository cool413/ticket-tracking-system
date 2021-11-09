using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models.Entities;
using Services.DataCommon;

namespace Databases.TicketSystemContext.Configurations
{
    public sealed class TicketConfiguration : IEntityTypeConfiguration<Ticket>
    {
        public void Configure(EntityTypeBuilder<Ticket> entity)
        {
            entity.ToTable(nameof(Ticket), "system")
                .HasKey(c => c.ID)
                .HasName($"PK_{nameof(Ticket)}")
                .IsClustered();

            entity.Property(e => e.ID)
                .HasColumnName(nameof(Ticket.ID))
                .HasColumnType(SqlDbTypes.Bigint)
                .ValueGeneratedOnAdd()
                .UseIdentityColumn(1,1)
                .HasComment("序號")
                .IsRequired();
            
            entity.Property(e => e.Type)
                .HasColumnName(nameof(Ticket.Type))
                .HasColumnType(SqlDbTypes.Tinyint)
                .HasDefaultValueSql("(0)")
                .HasComment("類別: 1.bug單")
                .IsRequired();

            entity.Property(e => e.Summary)
                .HasColumnName(nameof(Ticket.Summary))
                .HasColumnType(SqlDbTypes.Nvarchar(50))
                .HasDefaultValueSql("(N'')")
                .HasComment("摘要")
                .IsUnicode()
                .IsRequired();
            
            entity.Property(e => e.Description)
                .HasColumnName(nameof(Ticket.Description))
                .HasColumnType(SqlDbTypes.Nvarchar(500))
                .HasDefaultValueSql("(N'')")
                .HasComment("描述")
                .IsUnicode()
                .IsRequired();
            
            entity.Property(e => e.UserID)
                .HasColumnName(nameof(Ticket.UserID))
                .HasColumnType(SqlDbTypes.Int)
                .ValueGeneratedNever()
                .HasComment("使用者ID")
                .IsRequired();
            
            entity.Property(e => e.Status)
                .HasColumnName(nameof(Ticket.Status))
                .HasColumnType(SqlDbTypes.Tinyint)
                .HasDefaultValueSql("(0)")
                .HasComment("狀態")
                .IsRequired();

            entity.Property(e => e.CreatedAt)
                .HasColumnName(nameof(Ticket.CreatedAt))
                .HasColumnType(SqlDbTypes.Datetime)
                .HasDefaultValueSql("getdate()")
                .HasComment("建立日期")
                .IsRequired();

            entity.Property(e => e.CreatedBy)
                .HasColumnName(nameof(Ticket.CreatedBy))
                .HasColumnType(SqlDbTypes.Nvarchar(50))
                .HasDefaultValueSql("(N'SYSTEM')")
                .HasComment("建立者")
                .IsUnicode(true)
                .IsRequired();

            entity.Property(e => e.LastModifiedAt)
                .HasColumnName(nameof(Ticket.LastModifiedAt))
                .HasColumnType(SqlDbTypes.Datetime)
                .HasDefaultValueSql("getdate()")
                .HasComment("修改日期")
                .IsRequired();

            entity.Property(e => e.LastModifiedBy)
                .HasColumnName(nameof(Ticket.LastModifiedBy))
                .HasColumnType(SqlDbTypes.Nvarchar(50))
                .HasDefaultValueSql("(N'SYSTEM')")
                .HasComment("修改者")
                .IsUnicode(true)
                .IsRequired();
        }
    }
}
