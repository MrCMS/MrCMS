using System;
using System.Collections.Generic;
using MrCMS.Entities;
using MrCMS.Entities.Documents;
using MrCMS.Entities.People;
using MrCMS.Events;
using MrCMS.Helpers;
using MrCMS.Website;
using NHibernate;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MrCMS.DbConfiguration.Configuration
{
    public class SaveDocumentVersions : IOnUpdated<Document>
    {
        private User GetUser(ISession session)
        {
            if (CurrentRequestData.CurrentUser != null)
                return session.Load<User>(CurrentRequestData.CurrentUser.Id);
            if (CurrentRequestData.CurrentContext != null && CurrentRequestData.CurrentContext.User != null)
            {
                var currentUser =
                    session.QueryOver<User>().Where(
                        user => user.Email == CurrentRequestData.CurrentContext.User.Identity.Name).
                            SingleOrDefault();
                return currentUser;
            }
            return null;
        }

        public void Execute(OnUpdatedArgs<Document> args)
        {
            var document = args.Item;
            if (document != null && !document.IsDeleted)
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