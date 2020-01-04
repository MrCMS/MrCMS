using System.Collections.Generic;
using System.Linq;
using MrCMS.Data;
using MrCMS.Entities;
using MrCMS.Entities.People;

namespace MrCMS.Services
{
    public class BelongToUserLookupService : IBelongToUserLookupService
    {
        private readonly IRepositoryResolver _repositoryResolver;

        public BelongToUserLookupService( IRepositoryResolver repositoryResolver)
        {
            _repositoryResolver = repositoryResolver;
        }

        public T Get<T>(User user) where T : SystemEntity, IBelongToUser
        {
            return _repositoryResolver.GetGlobalRepository<T>().Query().FirstOrDefault(arg => arg.User == user);
        }

        public IList<T> GetAll<T>(User user) where T : SystemEntity, IBelongToUser
        {
            return _repositoryResolver.GetGlobalRepository<T>().Query().Where(arg => arg.User == user).ToList();
        }
    }
}