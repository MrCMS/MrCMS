using MrCMS.Entities.Documents.Web;
using MrCMS.Services;

namespace MrCMS.Events.Documents
{
    public class RaiseFooterScriptChangedEvent : IOnUpdated<Webpage>
    {
        public void Execute(OnUpdatedArgs<Webpage> args)
        {
            if (!args.HasChanged(x => x.CustomFooterScripts))
                return;

            EventContext.Instance.Publish<IOnFooterScriptChanged, ScriptChangedEventArgs<Webpage>>(
                new ScriptChangedEventArgs<Webpage>(args.Item, args.Item.CustomFooterScripts,
                    args.Original?.CustomFooterScripts));
        }
    }
}