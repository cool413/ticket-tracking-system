using Models.Entities;

namespace Databases.TicketSystemContext.Repositories
{
    public interface ITicketRepository : ITicketSystemBaseRepository<Ticket>
    {
    }

    public class TicketRepository : TicketSystemBaseRepository<Ticket>, ITicketRepository
    {
        private TicketSystemDbContext _ticketSystemDbContext;

        public TicketRepository(TicketSystemDbContext ticketSystemDbContext) : base(ticketSystemDbContext)
        {
            _ticketSystemDbContext = ticketSystemDbContext;
        }
    }
}
