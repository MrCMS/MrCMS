using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.Entities.People;

namespace MrCMS.DbConfiguration.Overrides
{
    public class UserOverride : IAutoMappingOverride<User>
    {
        public void Override(AutoMapping<User> mapping)
        {
            mapping.HasManyToMany(user => user.Roles).Cascade.SaveUpdate().Cache.ReadWrite();
            mapping.HasMany(user => user.UserProfileData).KeyColumn("UserId");
        }
    }
}