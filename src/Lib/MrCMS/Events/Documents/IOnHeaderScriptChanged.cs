using MrCMS.Entities.Documents.Web;

namespace MrCMS.Events.Documents
{
    public interface IOnHeaderScriptChanged : IEvent<ScriptChangedEventArgs<Webpage>> { }
}