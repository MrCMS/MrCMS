using System;
using MrCMS.Entities;
using MrCMS.Events;

namespace MrCMS.DbConfiguration.Configuration
{
    public class SetOnUpdatingProperties : IOnUpdating<SystemEntity>
    {
        public void Execute(OnUpdatingArgs<SystemEntity> args)
        {
            var now = DateTime.UtcNow;
            var systemEntity = args.Item;
            systemEntity.UpdatedOn = now;
        }
    }
}