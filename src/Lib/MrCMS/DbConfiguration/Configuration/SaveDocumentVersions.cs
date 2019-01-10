using System;
using MrCMS.Entities.Documents;
using MrCMS.Entities.People;
using MrCMS.Events;
using MrCMS.Helpers;
using MrCMS.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NHibernate;

namespace MrCMS.DbConfiguration.Configuration
{
    public class SaveDocumentVersions : IOnUpdated<Document>
    {
        private User GetUser(ISession session)
        {
            return session.GetService<IGetCurrentUser>().Get();
        }

        public void Execute(OnUpdatedArgs<Document> args)
        {
            var document = args.Item;
            if (document != null && !document.IsDeleted && args.Original != null)
            {
                var propertyInfos = document.GetType().GetVersionProperties();

                var jObject = new JObject();

                var anyChanges = false;
                foreach (var propertyInfo in propertyInfos)
                {
                    if (propertyInfo == null)
                        continue;

                    var oldValue = propertyInfo.GetValue(args.Original) ??
                                   (propertyInfo.PropertyType.IsValueType
                                        ? Activator.CreateInstance(propertyInfo.PropertyType)
                                        : null);
                    var newValue = propertyInfo.GetValue(args.Item);

                    if (oldValue != null)
                        if (!oldValue.Equals(newValue))
                            anyChanges = true;

                    if (oldValue == null && newValue != null)
                        anyChanges = true;

                    jObject.Add(propertyInfo.Name, new JRaw(JsonConvert.SerializeObject(oldValue)));

                }
                if (anyChanges)
                {
                    var s = args.Session;
                    var documentVersion = new DocumentVersion
                    {
                        Document = document,
                        Data = JsonConvert.SerializeObject(jObject),
                        User = GetUser(s),
                    };
                    document.Versions.Add(documentVersion);
                    s.Transact(session => session.Save(documentVersion));
                }
            }
        }
    }
}