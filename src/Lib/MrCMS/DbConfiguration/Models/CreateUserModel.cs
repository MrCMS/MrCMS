using Microsoft.EntityFrameworkCore;
using MrCMS.Common;
using MrCMS.Entities.People;

namespace MrCMS.DbConfiguration.Models
{
    public class CreateUserModel : ICreateModel
    {
        private readonly IReflectionHelper _reflectionHelper;

        public CreateUserModel(IReflectionHelper reflectionHelper)
        {
            _reflectionHelper = reflectionHelper;
        }

        public void Create(ModelBuilder builder)
        {
            builder.Entity<User>(user =>
            {
                user.HasMany(x => x.UserToRoles);
            });
            builder.Entity<UserRole>(role => { role.HasMany(x => x.UserToRoles); });
            builder.Entity<UserClaim>();
            builder.Entity<UserLogin>();
            builder.Entity<UserProfileData>(profileData =>
            {
                var discriminatorBuilder = profileData.HasDiscriminator<string>("ProfileInfoType");
                foreach (var type in _reflectionHelper.GetAllConcreteImplementationsOf<UserProfileData>())
                    discriminatorBuilder.HasValue(type, type.FullName);

            });
        }
    }
}