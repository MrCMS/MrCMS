using System;
using System.Collections.Generic;
using System.Web.Mvc;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using MrCMS.DbConfiguration;
using MrCMS.Entities;
using MrCMS.Helpers;
using NHibernate;
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

        public override IEnumerable<ModelMetadata> GetMetadataForProperties(object container, Type containerType)
        {
            var metadataForProperties = base.GetMetadataForProperties(container, containerType);
            return metadataForProperties;
        }

        public override ModelMetadata GetMetadataForProperty(Func<object> modelAccessor, Type containerType, string propertyName)
        {
            var metadataForProperty = base.GetMetadataForProperty(modelAccessor, containerType, propertyName);
            var displayName = _kernel.Get<IStringResourceProvider>()
                .GetDisplayName(containerType, propertyName);
            if (!string.IsNullOrWhiteSpace(displayName))
                metadataForProperty.DisplayName = displayName;
            return metadataForProperty;
        }

        public override ModelMetadata GetMetadataForType(Func<object> modelAccessor, Type modelType)
        {
            var metadataForType = base.GetMetadataForType(modelAccessor, modelType);
            return metadataForType;
        }
    }

    public interface IStringResourceProvider
    {
        string GetDisplayName(Type type, string propertyName);
    }

    public class StringResourceProvider : IStringResourceProvider
    {
        private readonly ISession _session;

        public StringResourceProvider(ISession session)
        {
            _session = session;
        }

        public string GetDisplayName(Type type, string propertyName)
        {
            var key = type.FullName + "." + propertyName;
            var singleOrDefault = _session.QueryOver<StringResource>()
                .Where(resource => resource.Key == key)
                .Take(1)
                .Cacheable()
                .SingleOrDefault();
            if (singleOrDefault == null)
            {
                singleOrDefault = new StringResource {Key = key};
                _session.Transact(session => session.Save(singleOrDefault));
            }
            return singleOrDefault.Value;
        }
    }

    public class StringResource : SiteEntity
    {
        public virtual string Key { get; set; }
        public virtual string Value { get; set; }
    }

    public class StringResourceOverride : IAutoMappingOverride<StringResource>
    {
        public void Override(AutoMapping<StringResource> mapping)
        {
            mapping.Map(resource => resource.Key).MakeVarCharMax();
            mapping.Map(resource => resource.Value).MakeVarCharMax();
        }
    }
}