using System.Security.Principal;
using MrCMS.Entities.People;
using NHibernate;

namespace MrCMS.Web.Areas.Admin.Services
{
    public class NotificationHubService : INotificationHubService
    {
        private readonly ISession _session;

        public NotificationHubService(ISession session)
        {
            _session = session;
        }

        public User GetUser(IPrincipal user)
        {
            if (user == null || string.IsNullOrWhiteSpace(user.Identity.Name))
                return null;
            return _session.QueryOver<User>()
                .Where(u => u.Email == user.Identity.Name)
                .Take(1)
                .Cacheable()
                .SingleOrDefault();
        }
    }
}