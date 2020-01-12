using System;
using System.Collections.Generic;
using System.Linq;
using MrCMS.Common;

namespace MrCMS.Data
{
    public class GetPrePersistence : IGetPrePersistence
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IReflectionHelper _reflectionHelper;

        public GetPrePersistence(IServiceProvider serviceProvider, IReflectionHelper reflectionHelper)
        {
            _serviceProvider = serviceProvider;
            _reflectionHelper = reflectionHelper;
        }
        public IEnumerable<OnDataAdding> GetOnAdding<T>() where T : class
        {
            var types = _reflectionHelper.GetAllConcreteImplementationsOf<OnDataAdding<T>>();
            return types.Select(type => _serviceProvider.GetService(type)).Cast<OnDataAdding>();
        }

        public IEnumerable<OnDataUpdating> GetOnUpdating<T>() where T : class
        {
            var types = _reflectionHelper.GetAllConcreteImplementationsOf<OnDataUpdating<T>>();
            return types.Select(type => _serviceProvider.GetService(type)).Cast<OnDataUpdating>();
        }

        public IEnumerable<OnDataDeleting> GetOnDeleting<T>() where T : class
        {
            var types = _reflectionHelper.GetAllConcreteImplementationsOf<OnDataDeleting<T>>();
            return types.Select(type => _serviceProvider.GetService(type)).Cast<OnDataDeleting>();
        }
    }
}