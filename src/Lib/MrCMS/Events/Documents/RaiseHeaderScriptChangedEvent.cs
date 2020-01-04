using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.Data;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;

namespace MrCMS.Events.Documents
{
    public class RaiseHeaderScriptChangedEvent : OnDataUpdated<Webpage>
    {
        private readonly IEventContext _eventContext;
        private readonly IRepository<Webpage> _repository;

        public RaiseHeaderScriptChangedEvent(IEventContext eventContext, IRepository<Webpage> repository)
        {
            _eventContext = eventContext;
            _repository = repository;
        }
        //public void Execute(OnUpdatedArgs<Webpage> args)
        //{
        //    if (!args.HasChanged(x => x.CustomHeaderScripts))
        //        return;

        //    _eventContext.Publish<IOnHeaderScriptChanged, ScriptChangedEventArgs<Webpage>>(
        //        new ScriptChangedEventArgs<Webpage>(args.Item, args.Item.CustomHeaderScripts,
        //            args.Original?.CustomHeaderScripts));
        //}

        public override async Task Execute(ChangeInfo data)
        {
            var changed =
                data.PropertiesUpdated.FirstOrDefault(x => x.Name == nameof(Webpage.CustomHeaderScripts));
            if (changed == null)
                return;
            var webpage = await _repository.Load(data.EntityId);
            await _eventContext.Publish<IOnHeaderScriptChanged, ScriptChangedEventArgs<Webpage>>(
                           new ScriptChangedEventArgs<Webpage>(webpage, changed.CurrentValue?.ToString(),
                               changed.OriginalValue?.ToString()));
        }
    }
}