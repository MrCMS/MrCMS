using System;
using Microsoft.EntityFrameworkCore;

namespace MrCMS.DbConfiguration
{
    //public class CreateAuditModel : ICreateModel
    //{
    //    private readonly IReflectionHelper _reflectionHelper;

    //    public CreateAuditModel(IReflectionHelper reflectionHelper)
    //    {
    //        _reflectionHelper = reflectionHelper;
    //    }

    //    public void Create(ModelBuilder builder)
    //    {
    //        builder.Entity<AuditLog>(typeBuilder =>
    //        {
    //            var logBuilder = typeBuilder.HasDiscriminator<string>("LogType");
    //            foreach (var type in _reflectionHelper.GetAllConcreteImplementationsOf(typeof(AuditLog)))
    //                logBuilder.HasValue(type, type.Name);
    //            typeBuilder.Property<DateTime>("Timestamp").HasDefaultValueSql("GETUTCDATE()");
    //            typeBuilder.HasOne(x => x.User).WithMany().OnDelete(DeleteBehavior.SetNull);
    //        });
    //        builder.Entity<PropertyUpdated>();
    //    }
    //}
}