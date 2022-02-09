using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;

namespace MrCMS.Events.Documents
{
    public class RaiseHeaderScriptChangedEvent : IOnUpdated<Webpage>
    {
        private readonly IEventContext _eventContext;

        public RaiseHeaderScriptChangedEvent(IEventContext eventContext)
        {
            _eventContext = eventContext;
        }

        public async Task Execute(OnUpdatedArgs<Webpage> args)
        {
            if (!args.HasChanged(x => x.CustomHeaderScripts))
                return;

            await _eventContext.Publish<IOnHeaderScriptChanged, ScriptChangedEventArgs<Webpage>>(
                new ScriptChangedEventArgs<Webpage>(args.Item, args.Item.CustomHeaderScripts,
                    args.Original?.CustomHeaderScripts));
        }
    }
}