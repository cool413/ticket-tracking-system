using System;

namespace Models.DTOs
{
    public sealed class SaveTicketDto
    {
        public long ID { get; set; }
        public byte? Type { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
        public int? UserID { get; set; }
        public byte? Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime LastModifiedAt { get; set; }
        public string LastModifiedBy { get; set; }
    }
}