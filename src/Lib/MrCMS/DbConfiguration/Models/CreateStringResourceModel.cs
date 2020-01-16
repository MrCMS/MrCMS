using Microsoft.EntityFrameworkCore;
using MrCMS.Entities.Resources;

namespace MrCMS.DbConfiguration.Models
{
    public class CreateStringResourceModel : ICreateModel
    {
        public void Create(ModelBuilder builder)
        {
            builder.Entity<StringResource>();
        }
    }
}