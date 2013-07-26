using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.Entities.People;

namespace MrCMS.DbConfiguration.Overrides
{
    public class UserProfileDataOverride : IAutoMappingOverride<UserProfileData>
    {
        public void Override(AutoMapping<UserProfileData> mapping)
        {
            mapping.DiscriminateSubClassesOnColumn("ProfileInfoType");
        }
    }
}