﻿using System.Collections.Generic;
using AutoMapper;
using MrCMS.Web.Apps.IdentityServer4.Admin.Core.Dtos.Common;
using MrCMS.Web.Apps.IdentityServer4.Admin.Core.Dtos.Configuration;
using MrCMS.Web.IdentityServer.NHibernate.Storage.Entities;
using MrCMS.Web.IdentityServer.NHibernate.Storage.Extensions;
using X.PagedList;

namespace MrCMS.Web.Apps.IdentityServer4.Admin.Core.Mappers
{
    public static class ClientMappers
    {
        static ClientMappers()
        {
            Mapper = new MapperConfiguration(cfg => cfg.AddProfile<ClientMapperProfile>())
                .CreateMapper();
        }

        internal static IMapper Mapper { get; }

        public static ClientDto ToModel(this Client client)
        {
            return Mapper.Map<ClientDto>(client);
        }

        public static ClientSecretsDto ToModel(this PagedList<ClientSecret> clientSecret)
        {
            return Mapper.Map<ClientSecretsDto>(clientSecret);
        }

        public static ClientClaimsDto ToModel(this PagedList<ClientClaim> clientClaims)
        {
            return Mapper.Map<ClientClaimsDto>(clientClaims);
        }

        public static ClientsDto ToModel(this PagedList<Client> clients)
        {
            return Mapper.Map<ClientsDto>(clients);
        }

        public static ClientPropertiesDto ToModel(this PagedList<ClientProperty> clientProperties)
        {
            return Mapper.Map<ClientPropertiesDto>(clientProperties);
        }
        
		public static Client ToEntity(this ClientDto client)
        {
            return Mapper.Map<Client>(client);
        }

		public static ClientSecretsDto ToModel(this ClientSecret clientSecret)
		{
			return Mapper.Map<ClientSecretsDto>(clientSecret);
		}
        
        public static ClientSecret ToEntity(this ClientSecretsDto clientSecret)
		{
			return Mapper.Map<ClientSecret>(clientSecret);
		}

        public static ClientClaimsDto ToModel(this ClientClaim clientClaim)
		{
			return Mapper.Map<ClientClaimsDto>(clientClaim);
		}

        public static ClientPropertiesDto ToModel(this ClientProperty clientProperty)
        {
            return Mapper.Map<ClientPropertiesDto>(clientProperty);
        }

        public static ClientClaim ToEntity(this ClientClaimsDto clientClaim)
		{
			return Mapper.Map<ClientClaim>(clientClaim);
		}

        public static ClientProperty ToEntity(this ClientPropertiesDto clientProperties)
        {
            return Mapper.Map<ClientProperty>(clientProperties);
        }

        public static SelectItemDto ToModel(this SelectItem selectItem)
        {
            return Mapper.Map<SelectItemDto>(selectItem);
        }

        public static List<SelectItemDto> ToModel(this List<SelectItem> selectItem)
        {
            return Mapper.Map<List<SelectItemDto>>(selectItem);
        }
    }
}