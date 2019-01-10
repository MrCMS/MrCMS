using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace MrCMS.Services.Resources
{
    /// <summary>
    /// Extensions for configuring MVC data annotations using an <see cref="IMvcBuilder"/>.
    /// </summary>
    public static class MrCMSDataAnnotationsMvcCoreBuilderExtensions
    {
        /// <summary>
        /// Registers MVC data annotations.
        /// </summary>
        /// <param name="builder">The <see cref="IMvcBuilder"/>.</param>
        /// <returns>The <see cref="IMvcBuilder"/>.</returns>
        public static IMvcBuilder AddMrCMSDataAnnotations(this IMvcBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            AddDataAnnotationsServices(builder.Services);
            return builder;
        }

        // Internal for testing.
        internal static void AddDataAnnotationsServices(IServiceCollection services)
        {
            services.TryAddEnumerable(
                ServiceDescriptor.Transient<IConfigureOptions<MvcOptions>, MrCMSDataAnnotationsMvcOptionsSetup>());
            services.TryAddSingleton<IValidationAttributeAdapterProvider, ValidationAttributeAdapterProvider>();
        }
    }
}