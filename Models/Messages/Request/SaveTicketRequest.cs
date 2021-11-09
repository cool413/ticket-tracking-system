using Models.DTOs;
using Models.Entities;

namespace Models.Messages.Request
{
    public class SaveTicketRequest
    {
        public long? ID { get; set; }
        public byte Type { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
        public int UserID { get; set; }
        public byte Status { get; set; }


        public static implicit operator Ticket(SaveTicketRequest request) => new Ticket
        {
            ID = request.ID ?? 0,
            Type = request.Type,
            UserID = request.UserID,
            Summary = request.Summary,
            Description = request.Description,
            Status = request.Status,
        };

    }
}