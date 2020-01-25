using Microsoft.EntityFrameworkCore;
using MrCMS.Logging;

namespace MrCMS.DbConfiguration.Models
{
    public class CreateLogModel : ICreateModel
    {
        public void Create(ModelBuilder builder)
        {
            builder.Entity<Log>();
        }
    }
}