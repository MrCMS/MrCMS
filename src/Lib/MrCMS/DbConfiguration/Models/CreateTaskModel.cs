using Microsoft.EntityFrameworkCore;
using MrCMS.Tasks;
using MrCMS.Tasks.Entities;

namespace MrCMS.DbConfiguration.Models
{
    public class CreateTaskModel : ICreateModel
    {
        public void Create(ModelBuilder builder)
        {
            builder.Entity<TaskSettings>();

            builder.Entity<QueuedTask>();
        }
    }
}