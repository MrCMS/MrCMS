using System;
using System.Web;
using MrCMS.Entities;
using MrCMS.Entities.Documents;
using MrCMS.Entities.People;
using MrCMS.Helpers;
using MrCMS.Website;
using NHibernate;
using NHibernate.Event;
using NHibernate.Event.Default;
using NHibernate.Persister.Entity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace MrCMS.DbConfiguration.Configuration
{
    public class SaveOrUpdateListener : DefaultSaveOrUpdateEventListener, IPreUpdateEventListener, IPreInsertEventListener
    {
        public bool OnPreUpdate(PreUpdateEvent @event)
        {
            var baseEntity = @event.Entity as BaseEntity;
            if (baseEntity != null)
            {
                var now = DateTime.UtcNow;
                if (!DbHasSetCreatedOn(@event.Persister, @event.State))
                    SetCreatedOn(@event.Persister, @event.State, baseEntity, now);
                SetUpdatedOn(@event.Persister, @event.State, baseEntity, now);
            }
            return false;
        }

        private bool DbHasSetCreatedOn(IEntityPersister persister, object[] state)
        {
            int index = Array.IndexOf(persister.PropertyNames, "CreatedOn");

            var currentCreatedOn = (DateTime)state[index];
            DateTime defaultDateTime = default(DateTime);
            return !DateTime.Equals(currentCreatedOn, defaultDateTime);
        }

        private void SetUpdatedOn(IEntityPersister persister, object[] state, BaseEntity baseEntity, DateTime date)
        {
            Set(persister, state, "UpdatedOn", date);

            baseEntity.UpdatedOn = date;
        }
        private void SetCreatedOn(IEntityPersister persister, object[] state, BaseEntity baseEntity, DateTime date)
        {
            Set(persister, state, "CreatedOn", date);

            baseEntity.CreatedOn = date;
        }

        private void Set(IEntityPersister persister, object[] state, string propertyName, object value)
        {
            int index = Array.IndexOf(persister.PropertyNames, propertyName);
            if (index == -1)
                return;
            state[index] = value;
        }

        public bool OnPreInsert(PreInsertEvent @event)
        {
            var baseEntity = @event.Entity as BaseEntity;
            if (baseEntity != null)
            {
                var now = DateTime.UtcNow;
                SetCreatedOn(@event.Persister, @event.State, baseEntity, now);
                SetUpdatedOn(@event.Persister, @event.State, baseEntity, now);
            }
            return false;
        }
    }

    public class PostCommitEventListener : IPostUpdateEventListener
    {
        public void OnPostUpdate(PostUpdateEvent @event)
        {
            try
            {
                var session = @event.Session.SessionFactory.OpenSession();
                if (@event.Entity is Document)
                {
                    var propertyInfos =
                        @event.Entity.GetType().GetProperties().Where(
                            info =>
                            info.CanWrite && !typeof(BaseEntity).IsAssignableFrom(info.PropertyType) &&
                            !info.PropertyType.IsGenericType).ToList();

                    var propertyNames = @event.Persister.PropertyNames;

                    var jObject = new JObject();

                    var anyChanges = false;
                    for (int i = 0; i < propertyNames.Length; i++)
                    {
                        string propertyName = propertyNames[i];

                        if (propertyInfos.All(info => info.Name != propertyName))
                            continue;

                        var oldValue = @event.OldState[i];
                        var newValue = @event.State[i];
                        if(oldValue != newValue)
                            anyChanges = true;
                        jObject.Add(propertyName, new JRaw(JsonConvert.SerializeObject(oldValue)));
                    }
                    if (anyChanges)
                    {
                        var document = GetDocument(session, @event.Entity as Document);

                        var documentVersion = new DocumentVersion
                                                  {
                                                      Document = document,
                                                      Data = JsonConvert.SerializeObject(jObject),
                                                      User = GetUser(session),
                                                      CreatedOn = DateTime.UtcNow,
                                                      UpdatedOn = DateTime.UtcNow
                                                  };
                        document.Versions.Add(documentVersion);
                        using (var transaction = session.BeginTransaction())
                        {
                            session.Save(documentVersion);
                            session.Update(document);
                            transaction.Commit();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        private Document GetDocument(ISession session, Document document)
        {
            return
                session.QueryOver<Document>().Fetch(doc => doc.Versions).Eager.Where(
                    doc => doc.Id == document.Id).List().FirstOrDefault();
        }

        private User GetUser(ISession session)
        {
            if (MrCMSApplication.CurrentContext != null && MrCMSApplication.CurrentContext.User != null)
            {
                var currentUser =
                    session.QueryOver<User>().Where(
                        user => user.Email == MrCMSApplication.CurrentContext.User.Identity.Name).
                        SingleOrDefault();
                return currentUser;
            }
            return null;
        }
    }
}
