using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MrCMS.Common;
using MrCMS.Entities;

namespace MrCMS.Data
{
    public class GetValidators : IGetValidators
    {
        private readonly IReflectionHelper _reflectionHelper;
        private readonly IServiceProvider _serviceProvider;

        public GetValidators(IReflectionHelper reflectionHelper, IServiceProvider serviceProvider)
        {
            _reflectionHelper = reflectionHelper;
            _serviceProvider = serviceProvider;
        }

        public IEnumerable<DatabaseValidatorBase> GetDatabaseValidators<TSubtype>()
        {
            var type = typeof(TSubtype);
            var validatorTypes = new HashSet<Type>();
            while (_reflectionHelper.IsImplementationOf(type, typeof(IHaveId)))
            {
                foreach (var concreteType in _reflectionHelper.GetAllConcreteImplementationsOf(
                    typeof(DatabaseValidatorBase<>).MakeGenericType(type)))
                    validatorTypes.Add(concreteType);
                type = type.GetTypeInfo().BaseType;
            }

            return validatorTypes.Select(t => _serviceProvider.GetService(t))
                .OfType<DatabaseValidatorBase>();
        }

        public IEnumerable<EntityValidatorBase> GetEntityValidators<TSubtype>()
        {
            var type = typeof(TSubtype);
            var validatorTypes = new HashSet<Type>();
            while (_reflectionHelper.IsImplementationOf(type, typeof(IHaveId)))
            {
                foreach (var concreteType in _reflectionHelper.GetAllConcreteImplementationsOf(
                    typeof(EntityValidatorBase<>).MakeGenericType(type)))
                    validatorTypes.Add(concreteType);
                type = type.GetTypeInfo().BaseType;
            }

            return validatorTypes.Select(t => _serviceProvider.GetService(t))
                .OfType<EntityValidatorBase>();
        }
    }
}