using Microsoft.EntityFrameworkCore;
using MrCMS.Entities.Notifications;

namespace MrCMS.DbConfiguration.Models
{
    public class CreateNotificationModel : ICreateModel
    {
        public void Create(ModelBuilder builder)
        {
            builder.Entity<Notification>();
        }
    }
}