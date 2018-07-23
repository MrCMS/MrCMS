
namespace MrCMS.Messages.Security
{
    public class SettingScriptChangeModel
    {
        public string PreviousValue { get; set; }
        public string CurrentValue { get; set; }

        // TODO: add status 
        //public ScriptChangeStatus Status
        //{
        //    get
        //    {
        //        if (string.IsNullOrWhiteSpace(PreviousValue) && !string.IsNullOrWhiteSpace(CurrentValue))
        //            return ScriptChangeStatus.Added;
        //        if (!string.IsNullOrWhiteSpace(PreviousValue) && string.IsNullOrWhiteSpace(CurrentValue))
        //            return ScriptChangeStatus.Removed;
        //        return ScriptChangeStatus.Modified;
        //    }
        //}

    }
}