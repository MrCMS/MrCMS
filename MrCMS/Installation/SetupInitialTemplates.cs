using System;
using MrCMS.Entities.Messaging;
using MrCMS.Helpers;
using NHibernate;

namespace MrCMS.Installation
{
    public class SetupInitialTemplates : IOnInstallation
    {
        private readonly ISession _session;

        public SetupInitialTemplates(ISession session)
        {
            _session = session;
        }

        public int Priority { get { return int.MaxValue; } }
        public void Install(InstallModel model)
        {
            _session.Transact(session =>
            {
                foreach (Type type in TypeHelper.GetAllConcreteMappedClassesAssignableFrom<MessageTemplate>())
                {
                    if (session.CreateCriteria(type).List().Count == 0)
                    {
                        var messageTemplate = Activator.CreateInstance(type) as MessageTemplate;
                        if (messageTemplate != null && messageTemplate.GetInitialTemplate(session) != null)
                            session.Save(messageTemplate.GetInitialTemplate(session));
                    }
                }
            });
        }
    }
}