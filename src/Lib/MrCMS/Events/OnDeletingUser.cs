using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MrCMS.Common;
using MrCMS.Data;
using MrCMS.Entities.People;

namespace MrCMS.Events
{
    public class OnDeletingUser : OnDataDeleting<User>
    {

        public override Task<IResult> OnDeleting(User entity, DbContext dbContext)
        {
            //foreach (UserRole userRole in entity.UserRoles)
            //    userRole.Users.Remove(entity);
            //entity.Roles.Clear();

            //return Success;
            return Success;
        }
    }
}