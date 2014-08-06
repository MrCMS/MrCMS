using System;
using MrCMS.Entities;
using MrCMS.Events;
using MrCMS.Website;

namespace MrCMS.DbConfiguration.Configuration
{
    public class SetOnUpdatingProperties : IOnUpdating<SystemEntity>
    {
        public void Execute(OnUpdatingArgs<SystemEntity> args)
        {
            DateTime now = CurrentRequestData.Now;
            SystemEntity systemEntity = args.Item;
            systemEntity.UpdatedOn = now;
        }
    }
}