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
            
        }
    }
}