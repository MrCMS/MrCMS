using AutoMapper;

namespace MrCMS.Web.IdentityServer.NHibernate.Storage.Mappings.Entities
{
    /// <summary>
    /// Entity to model mapping (and vice-versa) for persisted grants.
    /// </summary>
    public class PersistedGrantStoreMappingProfile : Profile
    {
        public PersistedGrantStoreMappingProfile()
        {
            CreateMap<global::MrCMS.Web.IdentityServer.NHibernate.Storage.Entities.PersistedGrant, global::IdentityServer4.Models.PersistedGrant>(MemberList.Destination)
                .ReverseMap()
                    .ForMember(dest => dest.Key, map => map.MapFrom(src => src.Key));
        }
    }
}