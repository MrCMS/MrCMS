using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.Entities.People;

namespace MrCMS.DbConfiguration.Overrides
{
    public class UserRoleOverride : IAutoMappingOverride<UserRole>
    {
        public void Override(AutoMapping<UserRole> mapping)
        {
            mapping.HasManyToMany(role => role.FrontEndWebpages).Inverse().Table("FrontEndWebpageRoles").Cache.ReadWrite();
            mapping.HasManyToMany(role => role.AdminWebpages).Inverse().Table("AdminWebpageRoles").Cache.ReadWrite();
            mapping.HasManyToMany(role => role.Users).Inverse().Cascade.SaveUpdate().Cache.ReadWrite();
        }
    }
}