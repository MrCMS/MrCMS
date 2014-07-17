namespace MrCMS.Tasks
{
    public abstract class SchedulableTask : BaseExecutableTask
    {
        public override sealed bool Schedulable
        {
            get { return true; }
        }

        public override sealed string GetData()
        {
            return string.Empty;
        }

        public override sealed void SetData(string data)
        {
        }
    }
}