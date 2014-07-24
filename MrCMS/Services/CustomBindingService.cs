using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Entities;
using MrCMS.Helpers;
using Ninject;

namespace MrCMS.Services
{
    public class CustomBindingService : ICustomBindingService
    {
        private readonly IKernel _kernel;

        static CustomBindingService()
        {
            ApplyCustomBindingTypes = new Dictionary<Type, HashSet<Type>>();

            foreach (Type type in TypeHelper.GetAllConcreteMappedClassesAssignableFrom<SystemEntity>().Where(type => !type.ContainsGenericParameters))
            {
                var hashSet = new HashSet<Type>();

                var thisType = type;
                while (typeof(SystemEntity).IsAssignableFrom(thisType))
                {
                    foreach (var assignType in TypeHelper.GetAllConcreteTypesAssignableFrom(
                        typeof(CustomBinderBase<>).MakeGenericType(type)))
                    {
                        hashSet.Add(assignType);
                    }
                    thisType = thisType.BaseType;

                }

                ApplyCustomBindingTypes.Add(type, hashSet);
            }
        }

        public CustomBindingService(IKernel kernel)
        {
            _kernel = kernel;
        }

        private static readonly Dictionary<Type, HashSet<Type>> ApplyCustomBindingTypes;

        public void ApplyCustomBinding<T>(T entity, ControllerContext controllerContext) where T : SystemEntity
        {
            if (entity == null)
            {
                return;
            }
            var type = entity.GetType();
            if (ApplyCustomBindingTypes.ContainsKey(type))
            {
                foreach (
                    var binderType in
                        ApplyCustomBindingTypes[type].Select(assignViewDataType => _kernel.Get(assignViewDataType))
                    )
                {
                    var customBinder = binderType as CustomBinderBase;
                    if (customBinder != null) customBinder.ApplyCustomBinding(entity, controllerContext);
                }
            }
        }
    }
}