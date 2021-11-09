using System;

namespace Models.Messages.Response
{
    public sealed class GetTicketsResponse
    {
        public long ID { get; set; }
        public byte Type { get; set; }
        public string TypeName { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
        public string UserAccount { get; set; }
        public byte Status { get; set; }
        public string StatusName { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime LastModifiedAt { get; set; }
        public string LastModifiedBy { get; set; }
    }
}