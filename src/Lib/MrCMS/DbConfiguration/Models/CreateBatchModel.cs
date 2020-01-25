using Microsoft.EntityFrameworkCore;
using MrCMS.Batching.Entities;
using MrCMS.Helpers;

namespace MrCMS.DbConfiguration.Models
{
    public class CreateBatchModel : ICreateModel
    {
        public void Create(ModelBuilder builder)
        {
            builder.Entity<Batch>();
            builder.Entity<BatchJob>(batchJob =>
            {
                var discriminatorBuilder = batchJob.HasDiscriminator<string>("discriminator");
                foreach (var type in TypeHelper.GetAllConcreteTypesAssignableFrom<BatchJob>())
                    discriminatorBuilder.HasValue(type, type.FullName);
            });
            builder.Entity<BatchRun>();
            builder.Entity<BatchRunResult>();
        }
    }
}