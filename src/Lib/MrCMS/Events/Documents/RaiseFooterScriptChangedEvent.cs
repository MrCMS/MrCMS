using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Internal;
using MrCMS.Data;
using MrCMS.Entities.Documents.Web;
using MrCMS.Services;

namespace MrCMS.Events.Documents
{
    public class RaiseFooterScriptChangedEvent : OnDataUpdated<Webpage>
    {
        private readonly IEventContext _eventContext;
        private readonly IRepository<Webpage> _repository;

        public RaiseFooterScriptChangedEvent(IEventContext eventContext, IRepository<Webpage> repository)
        {
            _eventContext = eventContext;
            _repository = repository;
        }

        public override async Task Execute(ChangeInfo data)
        {
            var updatedProperty = data.PropertiesUpdated.FirstOrDefault(x => x.Name != nameof(Webpage.CustomFooterScripts));
            if (updatedProperty == null)
                return;

            var webpage = await _repository.Load(data.EntityId);
            await _eventContext.Publish<IOnFooterScriptChanged, ScriptChangedEventArgs<Webpage>>(
                           new ScriptChangedEventArgs<Webpage>(webpage, updatedProperty.CurrentValue?.ToString(),
                               updatedProperty.OriginalValue?.ToString()));
        }
    }
}