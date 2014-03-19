using System;
using System.Web.Mvc;
using MrCMS.Services.Resources;
using Ninject;

namespace MrCMS.Website
{
    public class MrCMSMetadataProvider : DataAnnotationsModelMetadataProvider
    {
        private readonly IKernel _kernel;

        public MrCMSMetadataProvider(IKernel kernel)
        {
            _kernel = kernel;
        }

        public override ModelMetadata GetMetadataForProperty(Func<object> modelAccessor, Type containerType, string propertyName)
        {
            var metadataForProperty = base.GetMetadataForProperty(modelAccessor, containerType, propertyName);
            if (CurrentRequestData.DatabaseIsInstalled)
            {
                var key = string.Format("{0}.{1}", containerType.FullName, propertyName);
                var displayName = _kernel.Get<IStringResourceProvider>()
                                         .GetValue(key,
                                                   metadataForProperty.DisplayName ?? metadataForProperty.PropertyName);
                if (!string.IsNullOrWhiteSpace(displayName))
                    metadataForProperty.DisplayName = displayName;
            }
            return metadataForProperty;
        }
    }
}