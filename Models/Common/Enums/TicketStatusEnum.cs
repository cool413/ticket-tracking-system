namespace Models.Common.Enums
{
    public enum TicketStatusEnum : byte
    {
        /// <summary>
        /// 未知
        /// </summary>
        UnKnow  = 0,

        /// <summary>
        /// 代辦
        /// </summary>
        Pending = 1, 
        
        /// <summary>
        /// 進行中
        /// </summary>
        InProgress = 1, 
        
        /// <summary>
        /// 解決
        /// </summary>
        Resolve = 1, 
    }
}