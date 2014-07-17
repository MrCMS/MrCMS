namespace MrCMS.Models
{
    public class VersionChange
    {
        public string Property { get; set; }

        public object PreviousValue { get; set; }

        public object CurrentValue { get; set; }

        public bool AnyChange
        {
            get { return !Equals(PreviousValue, CurrentValue); }
        }
    }
}