using System;

namespace Models.Entities
{
    public sealed class User
    {
        public int ID { get; set; }
        public string Account { get; set; }
        public long RoleID { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime LastModifiedAt { get; set; }
        public string LastModifiedBy { get; set; }
    }
}