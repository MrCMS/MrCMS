using System;
using System.Threading.Tasks;
using MrCMS.Data;
using MrCMS.Entities.Documents;
using MrCMS.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MrCMS.Events
{
    public class SaveDocumentVersions : OnDataUpdated<Document>
    {
        private readonly IGetCurrentUser _getCurrentUser;
        private readonly IRepository<DocumentVersion> _repository;

        public SaveDocumentVersions(IGetCurrentUser getCurrentUser, IRepository<DocumentVersion> repository)
        {
            _getCurrentUser = getCurrentUser;
            _repository = repository;
        }
        public override async Task Execute(ChangeInfo data)
        {
            var jObject = new JObject();

            var anyChanges = false;
            foreach (var property in data.Properties.Keys)
            {
                if (property == null)
                    continue;

                var oldValue = data.OriginalValues[property];
                var newValue = data.Properties[property];

                if (oldValue != null)
                    if (!oldValue.Equals(newValue))
                        anyChanges = true;

                if (oldValue == null && newValue != null)
                    anyChanges = true;

                jObject.Add(property, new JRaw(JsonConvert.SerializeObject(oldValue)));
            }
            if (anyChanges)
            {
                var documentVersion = new DocumentVersion
                {
                    DocumentId = data.EntityId,
                    Data = JsonConvert.SerializeObject(jObject),
                    UserId = _getCurrentUser.GetId()
                };

                await _repository.Add(documentVersion);
            }
        }
    }
}