using MrCMS.Entities.Documents.Web;
using MrCMS.Services;

namespace MrCMS.Events.Documents
{
    public class RaiseHeaderScriptChangedEvent : IOnUpdated<Webpage>
    {
        public void Execute(OnUpdatedArgs<Webpage> args)
        {
            if (!args.HasChanged(x => x.CustomHeaderScripts))
                return;

            EventContext.Instance.Publish<IOnHeaderScriptChanged, ScriptChangedEventArgs<Webpage>>(
                new ScriptChangedEventArgs<Webpage>(args.Item, args.Item.CustomHeaderScripts,
                    args.Original?.CustomHeaderScripts));
        }
    }
}