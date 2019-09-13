// Based on the IdentityServer4.EntityFramework - authors - Brock Allen & Dominick Baier.
// https://github.com/IdentityServer/IdentityServer4.EntityFramework

// Also Based on IdentityServer4.Admin by Jan Škoruba
//https://github.com/skoruba/IdentityServer4.Admin

// Modified by Charles Ejedawe

using System.Linq;
using AutoMapper;
using MrCMS.Web.Apps.IdentityServer4.Admin.Core.Dtos.Configuration;
using MrCMS.Web.IdentityServer.NHibernate.Storage.Entities;
using X.PagedList;

namespace MrCMS.Web.Apps.IdentityServer4.Admin.Core.Mappers
{
    public class ApiResourceMapperProfile : Profile
    {
        public ApiResourceMapperProfile()
        {
            // entity to model
            CreateMap<ApiResource, ApiResourceDto>(MemberList.Destination)
                .ForMember(x => x.UserClaims, opts => opts.MapFrom(src => src.UserClaims.Select(x => x.Type)));

            CreateMap<ApiScope, ApiScopesDto>(MemberList.Destination)
                .ForMember(x => x.UserClaims, opt => opt.MapFrom(src => src.UserClaims.Select(x => x.Type)))
                .ForMember(x => x.ApiResourceId, opt => opt.MapFrom(src => src.ApiResource.Id))
                .ForMember(x => x.ApiScopeId, opt => opt.MapFrom(src => src.Id));

            CreateMap<ApiScope, ApiScopeDto>(MemberList.Destination)
                .ForMember(x => x.UserClaims, opt => opt.MapFrom(src => src.UserClaims.Select(x => x.Type)));

            CreateMap<ApiSecret, ApiSecretsDto>(MemberList.Destination)
                .ForMember(dest => dest.Type, opt => opt.Condition(srs => srs != null))
                .ForMember(x => x.ApiSecretId, opt => opt.MapFrom(x => x.Id))
                .ForMember(x => x.ApiResourceId, opt => opt.MapFrom(x => x.ApiResource.Id));

            CreateMap<ApiSecret, ApiSecretDto>(MemberList.Destination)
                .ForMember(dest => dest.Type, opt => opt.Condition(srs => srs != null));

            CreateMap<ApiResourceProperty, ApiResourcePropertyDto>(MemberList.Destination)
                .ReverseMap();

            CreateMap<ApiResourceProperty, ApiResourcePropertiesDto>(MemberList.Destination)
                .ForMember(dest => dest.Key, opt => opt.Condition(srs => srs != null))
                .ForMember(x => x.ApiResourcePropertyId, opt => opt.MapFrom(x => x.Id))
                .ForMember(x => x.ApiResourceId, opt => opt.MapFrom(x => x.ApiResource.Id));

            //PagedLists
            CreateMap<PagedList<ApiResource>, ApiResourcesDto>(MemberList.Destination)
                .ForMember(x => x.ApiResources, opt => opt.MapFrom(src => src.Select(x => x)));

            CreateMap<PagedList<ApiScope>, ApiScopesDto>(MemberList.Destination)
                .ForMember(x => x.Scopes, opt => opt.MapFrom(src => src.Select(x => x)));

            CreateMap<PagedList<ApiSecret>, ApiSecretsDto>(MemberList.Destination)
                .ForMember(x => x.ApiSecrets, opt => opt.MapFrom(src => src.Select(x => x)));

            CreateMap<PagedList<ApiResourceProperty>, ApiResourcePropertiesDto>(MemberList.Destination)
                .ForMember(x => x.ApiResourceProperties, opt => opt.MapFrom(src => src.Select(x => x)));

            // model to entity
            CreateMap<ApiResourceDto, ApiResource>(MemberList.Source)
                .ForMember(x => x.UserClaims, opts => opts.MapFrom(src => src.UserClaims.Select(x => new ApiResourceClaim { Type = x })));

            CreateMap<ApiSecretsDto, ApiSecret>(MemberList.Source)
                .ForMember(x => x.ApiResource, opts => opts.MapFrom(src => new ApiResource() { Id = src.ApiResourceId }))
                .ForMember(x => x.Id, opt => opt.MapFrom(src => src.ApiSecretId));

            CreateMap<ApiScopesDto, ApiScope>(MemberList.Source)
                .ForMember(x => x.UserClaims, opts => opts.MapFrom(src => src.UserClaims.Select(x => new ApiScopeClaim { Type = x })))
                .ForMember(x => x.Id, opt => opt.MapFrom(src => src.ApiScopeId));

            CreateMap<ApiScopeDto, ApiScope>(MemberList.Source)
                .ForMember(x => x.UserClaims, opts => opts.MapFrom(src => src.UserClaims.Select(x => new ApiScopeClaim { Type = x })));

            CreateMap<ApiResourcePropertiesDto, ApiResourceProperty>(MemberList.Source)
                .ForMember(x => x.ApiResource, dto => dto.MapFrom(src => new ApiResource() { Id = src.ApiResourceId }))
                .ForMember(x => x.Id, opt => opt.MapFrom(src => src.ApiResourcePropertyId));
        }
    }
}
