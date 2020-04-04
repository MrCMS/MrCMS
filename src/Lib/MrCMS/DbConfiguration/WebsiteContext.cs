using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Options;
using MrCMS.Data;
using MrCMS.Entities;
using MrCMS.Entities.Documents.Web;
using MrCMS.Entities.Multisite;
using MrCMS.Helpers;
using MrCMS.Settings;
using MrCMS.Website;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;

namespace MrCMS.DbConfiguration
{
    public class WebsiteContext : DbContext, ISystemDatabase
    {
        private readonly IGetModelCreators _getModelCreators;
        private readonly IOptions<SystemConfigurationSettings> _systemConfigurationSettings;

        public WebsiteContext(IGetModelCreators getModelCreators, DbContextOptions<WebsiteContext> options, IOptions<SystemConfigurationSettings> systemConfigurationSettings)
            : base(options)
        {
            _getModelCreators = getModelCreators;
            _systemConfigurationSettings = systemConfigurationSettings;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var modelCreator in _getModelCreators.GetCreators())
                modelCreator.Create(modelBuilder);

            // use non-pluralised tablenames
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                //entity.SetTableName((entity.BaseType ?? entity).DisplayName());
                if (entity.BaseType == null && entity.ClrType.IsImplementationOf(typeof(IHaveId))) 
                    entity.SetPrimaryKey(entity.FindProperty(nameof(IHaveId.Id)));

                //dates should be saved in utc and retrieved in local time.
                modelBuilder.UseValueConverterForType(typeof(DateTime), DateTimeConverter());
                modelBuilder.UseValueConverterForType(typeof(DateTime?), NullableDateTimeConverter());

                // Replace column names            
                foreach (var property in entity.GetProperties())
                    property.SetColumnName(property.Name);
            }

            //todo: is this better for ef as its the standard implementation by MS
            //modelBuilder.Entity<SiteEntity>(x => x.HasQueryFilter(entity => entity.IsDeleted == false));

            // prevent cascade deletes

            var cascadeFKs = modelBuilder.Model.GetEntityTypes()
                .SelectMany(t => t.GetForeignKeys())
                .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

            foreach (var fk in cascadeFKs)
                fk.DeleteBehavior = DeleteBehavior.Restrict;
        }

        private ValueConverter<DateTime?, DateTime?> NullableDateTimeConverter()
        {
            return new ValueConverter<DateTime?, DateTime?>(
                v => !v.HasValue ? (DateTime?) null :  v.Value.Kind == DateTimeKind.Utc ? v : TimeZoneInfo.ConvertTime(v.Value, 
                    _systemConfigurationSettings.Value.TimeZoneInfo,TimeZoneInfo.Utc), 
                v => !v.HasValue ? (DateTime?) null : 
                    TimeZoneInfo.ConvertTime(v.Value, TimeZoneInfo.Utc,
                        _systemConfigurationSettings.Value.TimeZoneInfo));
        }

        private ValueConverter<DateTime, DateTime> DateTimeConverter()
        {
            return new ValueConverter<DateTime, DateTime>(
                v => v.Kind == DateTimeKind.Utc ? v : TimeZoneInfo.ConvertTime(v, 
                    _systemConfigurationSettings.Value.TimeZoneInfo,TimeZoneInfo.Utc), 
                v =>
                    TimeZoneInfo.ConvertTime(v, TimeZoneInfo.Utc,
                        _systemConfigurationSettings.Value.TimeZoneInfo));
        }

        public bool IsMrCMSInstalled => Set<Site>().Any();
    }

    //public class WebpageConfiguration : IEntityTypeConfiguration<Webpage>
    //{
    //    public void Configure(EntityTypeBuilder<Webpage> builder)
    //    {
    //        builder.HasDiscriminator<string>(CreateCoreModel.WebpageDiscriminator);
    //    }
    //}
}