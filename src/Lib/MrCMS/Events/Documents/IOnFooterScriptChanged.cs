using MrCMS.Entities.Documents.Web;

namespace MrCMS.Events.Documents
{
    public interface IOnFooterScriptChanged : IEvent<ScriptChangedEventArgs<Webpage>> { }
}