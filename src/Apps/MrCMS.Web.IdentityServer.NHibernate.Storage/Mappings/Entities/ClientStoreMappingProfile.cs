using System.Collections.Generic;
using System.Security.Claims;
using AutoMapper;
using MrCMS.Web.IdentityServer.NHibernate.Storage.Entities;
using IdentityServer4.Models;

namespace MrCMS.Web.IdentityServer.NHibernate.Storage.Mappings.Entities
{
    /// <summary>
    /// Entity to model mapping (and vice-versa) for clients.
    /// </summary>
    public class ClientStoreMappingProfile : Profile
    {
        public ClientStoreMappingProfile()
        {
            CreateMap<ClientProperty, KeyValuePair<string, string>>()
                .ReverseMap();

            CreateMap<Storage.Entities.Client, IdentityServer4.Models.Client>()
                .ForMember(dest => dest.ProtocolType, opt => opt.Condition(srs => srs != null))
                .ReverseMap()
                    .ForMember(dest => dest.AllowedGrantTypes, opt => 
                    {
                        opt.MapFrom(src => src.AllowedGrantTypes);
                        opt.UseDestinationValue();
                    })
                    .ForMember(dest => dest.ClientSecrets, opt =>
                    {
                        opt.MapFrom(src => src.ClientSecrets);
                        opt.UseDestinationValue();
                    })
                    .ForMember(dest => dest.RedirectUris, opt =>
                    {
                        opt.MapFrom(src => src.RedirectUris);
                        opt.UseDestinationValue();
                    })
                    .ForMember(dest => dest.PostLogoutRedirectUris, opt =>
                    {
                        opt.MapFrom(src => src.PostLogoutRedirectUris);
                        opt.UseDestinationValue();
                    })
                    .ForMember(dest => dest.AllowedScopes, opt =>
                    {
                        opt.MapFrom(src => src.AllowedScopes);
                        opt.UseDestinationValue();
                    })
                    .ForMember(dest => dest.IdentityProviderRestrictions, opt =>
                    {
                        opt.MapFrom(src => src.IdentityProviderRestrictions);
                        opt.UseDestinationValue();
                    })
                    .ForMember(dest => dest.Claims, opt =>
                    {
                        opt.MapFrom(src => src.Claims);
                        opt.UseDestinationValue();
                    })
                    .ForMember(dest => dest.AllowedCorsOrigins, opt => 
                    {
                        opt.MapFrom(src => src.AllowedCorsOrigins);
                        opt.UseDestinationValue();
                    })
                    .ForMember(dest => dest.Properties, opt =>
                    {
                        opt.MapFrom(src => src.Properties);
                        opt.UseDestinationValue();
                    });

            CreateMap<ClientGrantType, string>()
                .ConstructUsing(src => src.GrantType)
                .ReverseMap()
                    .ForMember(dest => dest.GrantType, opt => opt.MapFrom(src => src));

            CreateMap<ClientRedirectUri, string>()
                .ConstructUsing(src => src.RedirectUri)
                .ReverseMap()
                    .ForMember(dest => dest.RedirectUri, opt => opt.MapFrom(src => src));

            CreateMap<ClientPostLogoutRedirectUri, string>()
                .ConstructUsing(src => src.PostLogoutRedirectUri)
                .ReverseMap()
                    .ForMember(dest => dest.PostLogoutRedirectUri, opt => opt.MapFrom(src => src));

            CreateMap<ClientScope, string>()
                .ConstructUsing(src => src.Scope)
                .ReverseMap()
                    .ForMember(dest => dest.Scope, opt => opt.MapFrom(src => src));

            CreateMap<ClientSecret, IdentityServer4.Models.Secret>(MemberList.Destination)
                .ForMember(dest => dest.Type, opt => opt.Condition(srs => srs != null))
                .ReverseMap();

            CreateMap<ClientIdPRestriction, string>()
                .ConstructUsing(src => src.Provider)
                .ReverseMap()
                    .ForMember(dest => dest.Provider, opt => opt.MapFrom(src => src));

            CreateMap<ClientClaim, Claim>(MemberList.None)
                .ConstructUsing(src => new Claim(src.Type, src.Value))
                .ReverseMap();

            CreateMap<ClientCorsOrigin, string>()
                .ConstructUsing(src => src.Origin)
                .ReverseMap()
                    .ForMember(dest => dest.Origin, opt => opt.MapFrom(src => src));
        }
    }
}
