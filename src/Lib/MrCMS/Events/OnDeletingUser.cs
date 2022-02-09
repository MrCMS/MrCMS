using System.Threading.Tasks;
using MrCMS.Entities.People;

namespace MrCMS.Events
{
    public class OnDeletingUser : IOnDeleting<User>
    {
        public Task Execute(OnDeletingArgs<User> args)
        {
            User user = args.Item;

            user?.Roles.Clear();
            return Task.CompletedTask;
        }
    }
}