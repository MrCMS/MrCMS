using MrCMS.Entities.Documents.Web;

namespace MrCMS.Events.Documents
{
    public class ScriptChangedEventArgs<T>
    {
        public ScriptChangedEventArgs(T holder, string currentValue, string previousValue)
        {
            Holder = holder;
            CurrentValue = currentValue;
            PreviousValue = previousValue;
        }

        public T Holder { get; }
        public string PreviousValue { get; }
        public string CurrentValue { get; }

        public ScriptChangeStatus Status
        {
            get
            {
                if (string.IsNullOrWhiteSpace(PreviousValue) && !string.IsNullOrWhiteSpace(CurrentValue))
                    return ScriptChangeStatus.Added;
                if (!string.IsNullOrWhiteSpace(PreviousValue) && string.IsNullOrWhiteSpace(CurrentValue))
                    return ScriptChangeStatus.Removed;
                return ScriptChangeStatus.Modified;
            }
        }
    }
}