using Microsoft.EntityFrameworkCore;
using MrCMS.Entities.Messaging;

namespace MrCMS.DbConfiguration.Models
{
    public class CreateMessageModel : ICreateModel
    {
        public void Create(ModelBuilder builder)
        {
            builder.Entity<LegacyMessageTemplate>(typeBuilder => typeBuilder.ToTable("MessageTemplate"));
            builder.Entity<MessageTemplateData>();

            builder.Entity<QueuedMessage>();
            builder.Entity<QueuedMessageAttachment>();
        }
    }
}