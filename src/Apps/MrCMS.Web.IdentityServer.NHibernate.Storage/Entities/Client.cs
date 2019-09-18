using System;
using System.Collections.Generic;
using IdentityServer4.Models;
using MrCMS.Entities;

namespace MrCMS.Web.IdentityServer.NHibernate.Storage.Entities
{
    #pragma warning disable 1591

    public class Client : SystemEntity
    {
        public virtual bool Enabled { get; set; } = true;
        public virtual string ClientId { get; set; }
        public virtual string ProtocolType { get; set; } = "oidc";
        public virtual ISet<ClientSecret> ClientSecrets { get; set; } = new HashSet<ClientSecret>();
        public virtual bool RequireClientSecret { get; set; } = true;
        public virtual string ClientName { get; set; }
        public virtual string Description { get; set; }
        public virtual string ClientUri { get; set; }
        public virtual string LogoUri { get; set; }
        public virtual bool RequireConsent { get; set; } = true;
        public virtual bool AllowRememberConsent { get; set; } = true;
        public virtual bool AlwaysIncludeUserClaimsInIdToken { get; set; }
        public virtual ISet<ClientGrantType> AllowedGrantTypes { get; set; } = new HashSet<ClientGrantType>();
        public virtual bool RequirePkce { get; set; }
        public virtual bool AllowPlainTextPkce { get; set; }
        public virtual bool AllowAccessTokensViaBrowser { get; set; }
        public virtual ISet<ClientRedirectUri> RedirectUris { get; set; } = new HashSet<ClientRedirectUri>();
        public virtual ISet<ClientPostLogoutRedirectUri> PostLogoutRedirectUris { get; set; } = new HashSet<ClientPostLogoutRedirectUri>();
        public virtual string FrontChannelLogoutUri { get; set; }
        public virtual bool FrontChannelLogoutSessionRequired { get; set; } = true;
        public virtual string BackChannelLogoutUri { get; set; }
        public virtual bool BackChannelLogoutSessionRequired { get; set; } = true;
        public virtual bool AllowOfflineAccess { get; set; }
        public virtual ISet<ClientScope> AllowedScopes { get; set; } = new HashSet<ClientScope>();
        public virtual int IdentityTokenLifetime { get; set; } = 300;
        public virtual int AccessTokenLifetime { get; set; } = 3600;
        public virtual int AuthorizationCodeLifetime { get; set; } = 300;
        public virtual int? ConsentLifetime { get; set; } = null;
        public virtual int AbsoluteRefreshTokenLifetime { get; set; } = 2592000;
        public virtual int SlidingRefreshTokenLifetime { get; set; } = 1296000;
        public virtual int RefreshTokenUsage { get; set; } = (int)TokenUsage.OneTimeOnly;
        public virtual bool UpdateAccessTokenClaimsOnRefresh { get; set; }
        public virtual int RefreshTokenExpiration { get; set; } = (int)TokenExpiration.Absolute;
        public virtual int AccessTokenType { get; set; } = (int)IdentityServer4.Models.AccessTokenType.Jwt;
        public virtual bool EnableLocalLogin { get; set; } = true;
        public virtual ISet<ClientIdPRestriction> IdentityProviderRestrictions { get; set; } = new HashSet<ClientIdPRestriction>();
        public virtual bool IncludeJwtId { get; set; }
        public virtual ISet<ClientClaim> Claims { get; set; } = new HashSet<ClientClaim>();
        public virtual bool AlwaysSendClientClaims { get; set; }
        public virtual string ClientClaimsPrefix { get; set; } = "client_";
        public virtual string PairWiseSubjectSalt { get; set; }
        public virtual ISet<ClientCorsOrigin> AllowedCorsOrigins { get; set; } = new HashSet<ClientCorsOrigin>();
        public virtual ISet<ClientProperty> Properties { get; set; } = new HashSet<ClientProperty>();
        public virtual DateTime Created { get; set; } = DateTime.UtcNow;
        public virtual DateTime? Updated { get; set; }
        public virtual DateTime? LastAccessed { get; set; }
        public virtual int? UserSsoLifetime { get; set; }
        public virtual string UserCodeType { get; set; }
        public virtual int DeviceCodeLifetime { get; set; } = 300;
        public virtual bool NonEditable { get; set; }
    }
}
