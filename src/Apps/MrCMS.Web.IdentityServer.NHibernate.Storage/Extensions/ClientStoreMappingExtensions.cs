using AutoMapper;
using MrCMS.Web.IdentityServer.NHibernate.Storage.Entities;
using MrCMS.Web.IdentityServer.NHibernate.Storage.Mappings.Entities;

namespace MrCMS.Web.IdentityServer.NHibernate.Storage.Extensions
{
    /// <summary>
    /// Add mapping profile for <see cref="Client"/> to AutoMapper configuration.
    /// </summary>
    public static class ClientStoreMappingExtensions
    {
        private static IMapper Mapper;

        static ClientStoreMappingExtensions()
        {
            //Mapper = new MapperConfiguration(cfg => cfg.AddProfile<ClientStoreMappingProfile>())
            //    .CreateMapper();
        }

        /// <summary>
        /// Maps a "Client" entity to a "Client" model.
        /// </summary>
        /// <param name="entity">The entity.</param>
        //public static IdentityServer4.Models.Client ToModel(this Client entity)
        //{
        //    return Mapper.Map<IdentityServer4.Models.Client>(entity);
        //}

        ///// <summary>
        ///// Maps a "Client" model to a "Client" entity.
        ///// </summary>
        ///// <param name="model">The model.</param>
        //public static Client ToEntity(this IdentityServer4.Models.Client model)
        //{
        //    return Mapper.Map<Client>(model);
        //}
    }
}
