using Databases.TicketSystemContext.Configurations;
using Microsoft.EntityFrameworkCore;
using Models.Entities;
using Services.DataCommon;

namespace Databases.TicketSystemContext
{
    public class TicketSystemDbContext : DbContext
    {
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<RoleAuthority> RoleAuthority { get; set; }
        public virtual DbSet<Menu> Menu { get; set; }
        public virtual DbSet<Ticket> Ticket { get; set; }
       

        public TicketSystemDbContext(DbContextOptions<TicketSystemDbContext> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            CustomDbFunctions.DbFunctionsRes(modelBuilder);

            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new RoleConfiguration());
            modelBuilder.ApplyConfiguration(new RoleAuthorityConfiguration());
            modelBuilder.ApplyConfiguration(new MenuConfiguration());
            modelBuilder.ApplyConfiguration(new TicketConfiguration());
            
        }
    }
}