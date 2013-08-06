using System;
using MrCMS.Entities.Messaging;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Tasks;

namespace MrCMS.Installation
{
    internal class SetupMessageTemplates : BackgroundTask
    {
        public SetupMessageTemplates(Site site)
            : base(site)
        {
        }

        public override void Execute()
        {
            Session.Transact(s =>
                                 {
                                     foreach (var type in TypeHelper.GetAllConcreteMappedClassesAssignableFrom<MessageTemplate>())
                                     {
                                         if (s.CreateCriteria(type).List().Count == 0)
                                         {
                                             var messageTemplate = Activator.CreateInstance(type) as MessageTemplate;
                                             if (messageTemplate != null && messageTemplate.GetInitialTemplate() != null)
                                                 s.Save(messageTemplate.GetInitialTemplate());
                                         }
                                     }
                                 });
        }
    }
}