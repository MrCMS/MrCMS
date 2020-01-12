using Microsoft.EntityFrameworkCore;
using MrCMS.Entities.Settings;

namespace MrCMS.DbConfiguration.Models
{
    public class CreateSettingsModel : ICreateModel
    {
        public void Create(ModelBuilder builder)
        {
            builder.Entity<SystemSetting>();
            builder.Entity<Setting>();
        }
    }
}