using MrCMS.Entities.People;

namespace MrCMS.Events
{
    public class OnDeletingUser : IOnDeleting<User>
    {
        public void Execute(OnDeletingArgs<User> args)
        {
            User user = args.Item;
            if (user == null)
                return;

            foreach (UserRole userRole in user.Roles)
                userRole.Users.Remove(user);
            user.Roles.Clear();
        }
    }
}