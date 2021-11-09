using Models.Entities;

namespace Databases.TicketSystemContext.Repositories
{
    public interface IUserRepository : ITicketSystemBaseRepository<User>
    {
    }

    public class UserRepository : TicketSystemBaseRepository<User>, IUserRepository
    {
        private TicketSystemDbContext _ticketSystemDbContext;

        public UserRepository(TicketSystemDbContext ticketSystemDbContext) : base(ticketSystemDbContext)
        {
            _ticketSystemDbContext = ticketSystemDbContext;
        }
    }
}
