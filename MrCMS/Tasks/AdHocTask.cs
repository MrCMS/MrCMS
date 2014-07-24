namespace MrCMS.Tasks
{
    public abstract class AdHocTask : BaseExecutableTask
    {
        public override sealed bool Schedulable
        {
            get { return false; }
        }
    }
}