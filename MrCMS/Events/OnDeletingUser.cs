using MrCMS.DbConfiguration;
using MrCMS.Entities.People;

namespace MrCMS.Events
{
    public class OnDeletingUser : IOnDeleting
    {
        public void Execute(OnDeletingArgs args)
        {
            var user = args.Item as User;
            if (user == null)
                return;

            foreach (UserRole userRole in user.Roles)
                userRole.Users.Remove(user);
            user.Roles.Clear();
        }
    }
}