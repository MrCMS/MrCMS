using MrCMS.Entities.Documents.Web;
using MrCMS.Services;

namespace MrCMS.Events.Documents
{
    public class RaiseFooterScriptChangedEvent : IOnUpdated<Webpage>
    {
        private readonly IEventContext _eventContext;

        public RaiseFooterScriptChangedEvent(IEventContext eventContext)
        {
            _eventContext = eventContext;
        }
        public void Execute(OnUpdatedArgs<Webpage> args)
        {
            if (!args.HasChanged(x => x.CustomFooterScripts))
                return;

            _eventContext.Publish<IOnFooterScriptChanged, ScriptChangedEventArgs<Webpage>>(
                new ScriptChangedEventArgs<Webpage>(args.Item, args.Item.CustomFooterScripts,
                    args.Original?.CustomFooterScripts));
        }
    }
}