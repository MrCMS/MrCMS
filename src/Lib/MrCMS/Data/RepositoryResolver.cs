using System;
using Microsoft.Extensions.DependencyInjection;
using MrCMS.Entities;

namespace MrCMS.Data
{
    public class RepositoryResolver : IRepositoryResolver
    {
        private readonly IServiceProvider _serviceProvider;

        public RepositoryResolver(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public IRepository<T> GetRepository<T>() where T : class, IHaveId, IHaveSite
        {
            return _serviceProvider.GetRequiredService<IRepository<T>>();
        }

        public IGlobalRepository<T> GetGlobalRepository<T>() where T : class, IHaveId
        {
            return _serviceProvider.GetRequiredService<IGlobalRepository<T>>();
        }
    }
}