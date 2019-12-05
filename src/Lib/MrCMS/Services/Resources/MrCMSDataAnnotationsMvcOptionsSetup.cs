using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using MrCMS.Website.Metadata;

namespace MrCMS.Services.Resources
{
    /// <summary>
    /// Sets up default options for <see cref="MvcOptions"/>.
    /// </summary>
    public class MrCMSDataAnnotationsMvcOptionsSetup : IConfigureOptions<MvcOptions>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IValidationAttributeAdapterProvider _validationAttributeAdapterProvider;
        private readonly IOptions<MvcDataAnnotationsLocalizationOptions> _dataAnnotationLocalizationOptions;

        public MrCMSDataAnnotationsMvcOptionsSetup(
            IValidationAttributeAdapterProvider validationAttributeAdapterProvider,
            IOptions<MvcDataAnnotationsLocalizationOptions> dataAnnotationLocalizationOptions)
        {
            _validationAttributeAdapterProvider = validationAttributeAdapterProvider ?? throw new ArgumentNullException(nameof(validationAttributeAdapterProvider));
            _dataAnnotationLocalizationOptions = dataAnnotationLocalizationOptions ?? throw new ArgumentNullException(nameof(dataAnnotationLocalizationOptions));
        }

        public MrCMSDataAnnotationsMvcOptionsSetup(
            IValidationAttributeAdapterProvider validationAttributeAdapterProvider,
            IOptions<MvcDataAnnotationsLocalizationOptions> dataAnnotationLocalizationOptions,
            IServiceProvider serviceProvider)
            : this(validationAttributeAdapterProvider, dataAnnotationLocalizationOptions)
        {
            _serviceProvider = serviceProvider;
        }

        public void Configure(MvcOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var metadataDetailsProvider = options.ModelMetadataDetailsProviders.FirstOrDefault(x => x.GetType().Name == "DataAnnotationsMetadataProvider");
            
            if (metadataDetailsProvider != null)
                options.ModelMetadataDetailsProviders.Remove(metadataDetailsProvider);

            options.ModelMetadataDetailsProviders.Add(new MrCMSDataAnnotationsMetadataProvider(
                _dataAnnotationLocalizationOptions,
                _serviceProvider));

        }
    }
}