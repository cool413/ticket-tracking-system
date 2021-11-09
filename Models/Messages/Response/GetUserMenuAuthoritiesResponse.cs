namespace Models.Messages.Response
{
    public sealed class GetUserMenuAuthoritiesResponse
    {
        public int MenuID { get; set; }
        public bool CanInsert { get; set; }
        public bool CanDelete { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanRead { get; set; }
        public bool CanResolve { get; set; }
    }
}