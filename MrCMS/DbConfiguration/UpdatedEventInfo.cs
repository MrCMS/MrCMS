namespace MrCMS.DbConfiguration
{
    public abstract class UpdatedEventInfo
    {
        public bool PreTransactionHandled { get; set; }
        public bool PostTransactionHandled { get; set; }
        public abstract object ObjectBase { get; }
        public abstract object OriginalVersionBase { get; }
    }
}