using System.Collections.Generic;
using AutoMapper;
using IdentityServer4.Models;
using MrCMS.Web.IdentityServer.NHibernate.Storage.Entities;

namespace MrCMS.Web.IdentityServer.NHibernate.Storage.Mappings.Entities
{
    /// <summary>
    /// Defines entity/model mapping for API resources and Identity resources.
    /// </summary>
    public class ResourceStoreMappingProfile : Profile
    {
        public ResourceStoreMappingProfile()
        {
            CreateMap<ApiResourceProperty, KeyValuePair<string, string>>()
                .ReverseMap();

            CreateMap<Storage.Entities.ApiResource, global::IdentityServer4.Models.ApiResource>(MemberList.Destination)
                .ConstructUsing(src => new global::IdentityServer4.Models.ApiResource())
                .ForMember(dest => dest.ApiSecrets, opt => opt.MapFrom(src => src.Secrets))
                .ReverseMap()
                    .ForMember(dest => dest.Scopes, opt =>
                    {
                        opt.MapFrom(src => src.Scopes);
                        opt.UseDestinationValue();
                    })
                    .ForMember(dest => dest.Secrets, opt =>
                    {
                        opt.MapFrom(src => src.ApiSecrets);
                        opt.UseDestinationValue();
                    })
                    .ForMember(dest => dest.UserClaims, opt =>
                    {
                        opt.MapFrom(src => src.UserClaims);
                        opt.UseDestinationValue();
                    })
                    .ForMember(dest => dest.Properties, opt =>
                    {
                        opt.MapFrom(src => src.Properties);
                        opt.UseDestinationValue();
                    });

            CreateMap<ApiResourceClaim, string>()
                .ConstructUsing(x => x.Type)
                .ReverseMap()
                    .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src));

            CreateMap<ApiSecret, global::IdentityServer4.Models.Secret>(MemberList.Destination)
                .ForMember(dest => dest.Type, opt => opt.Condition(srs => srs != null))
                .ReverseMap();

            CreateMap<ApiScope, Scope>(MemberList.Destination)
                .ConstructUsing(src => new Scope())
                .ReverseMap()
                    .ForMember(dest => dest.UserClaims, opt =>
                    {
                        opt.MapFrom(src => src.UserClaims);
                        opt.UseDestinationValue();
                    });

            CreateMap<ApiScopeClaim, string>()
                .ConstructUsing(x => x.Type)
                .ReverseMap()
                    .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src));

            CreateMap<IdentityResourceProperty, KeyValuePair<string, string>>()
                .ReverseMap();

            CreateMap<global::MrCMS.Web.IdentityServer.NHibernate.Storage.Entities.IdentityResource, global::IdentityServer4.Models.IdentityResource>(MemberList.Destination)
                .ConstructUsing(src => new global::IdentityServer4.Models.IdentityResource())
                .ReverseMap()
                    .ForMember(dest => dest.UserClaims, opt => 
                    {
                        opt.MapFrom(src => src.UserClaims);
                        opt.UseDestinationValue();
                    })
                    .ForMember(dest => dest.Properties, opt =>
                    {
                        opt.MapFrom(src => src.Properties);
                        opt.UseDestinationValue();
                    });

            CreateMap<IdentityClaim, string>()
                .ConstructUsing(x => x.Type)
                .ReverseMap()
                    .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src));
        }
    }
}
