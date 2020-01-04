namespace MrCMS.Data
{
    public abstract class OnDataSaved
    {
        public virtual int Priority => 0;
    }
}