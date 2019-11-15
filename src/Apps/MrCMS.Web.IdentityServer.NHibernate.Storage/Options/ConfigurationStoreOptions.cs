namespace MrCMS.Web.IdentityServer.NHibernate.Storage.Options
{
    /// <summary>
    /// Options for configuring the configuration store.
    /// </summary>
    public class ConfigurationStoreOptions : StoreOptionsBase
    {
        /// <summary>
        /// If true, enables the cache on the configuration store.
        /// </summary>
        public bool EnableConfigurationStoreCache { get; set; } = false;

        /// <summary>
        /// Gets or sets the identity resource table configuration.
        /// </summary>
        /// <value>
        /// The identity resource.
        /// </value>
        public TableDefinition IdentityResource { get; set; } = new TableDefinition("IdentityResources");

        /// <summary>
        /// Gets or sets the identity claim table configuration.
        /// </summary>
        /// <value>
        /// The identity claim.
        /// </value>
        public TableDefinition IdentityClaim { get; set; } = new TableDefinition("IdentityClaims");


        /// <summary>
        /// Gets or sets the API resource table configuration.
        /// </summary>
        /// <value>
        /// The API resource.
        /// </value>
        public TableDefinition ApiResource { get; set; } = new TableDefinition("ApiResources");

        /// <summary>
        /// Gets or sets the API secret table configuration.
        /// </summary>
        /// <value>
        /// The API secret.
        /// </value>
        public TableDefinition ApiSecret { get; set; } = new TableDefinition("ApiSecrets");

        /// <summary>
        /// Gets or sets the API scope table configuration.
        /// </summary>
        /// <value>
        /// The API scope.
        /// </value>
        public TableDefinition ApiScope { get; set; } = new TableDefinition("ApiScopes");

        /// <summary>
        /// Gets or sets the API claim table configuration.
        /// </summary>
        /// <value>
        /// The API claim.
        /// </value>
        public TableDefinition ApiClaim { get; set; } = new TableDefinition("ApiClaims");

        /// <summary>
        /// Gets or sets the API scope claim table configuration.
        /// </summary>
        /// <value>
        /// The API scope claim.
        /// </value>
        public TableDefinition ApiScopeClaim { get; set; } = new TableDefinition("ApiScopeClaims");

        /// <summary>
        /// Gets or sets the client table configuration.
        /// </summary>
        /// <value>
        /// The client.
        /// </value>
        public TableDefinition Client { get; set; } = new TableDefinition("Clients");

        /// <summary>
        /// Gets or sets the type of the client grant table configuration.
        /// </summary>
        /// <value>
        /// The type of the client grant.
        /// </value>
        public TableDefinition ClientGrantType { get; set; } = new TableDefinition("ClientGrantTypes");

        /// <summary>
        /// Gets or sets the client redirect URI table configuration.
        /// </summary>
        /// <value>
        /// The client redirect URI.
        /// </value>
        public TableDefinition ClientRedirectUri { get; set; } = new TableDefinition("ClientRedirectUris");

        /// <summary>
        /// Gets or sets the client post logout redirect URI table configuration.
        /// </summary>
        /// <value>
        /// The client post logout redirect URI.
        /// </value>
        public TableDefinition ClientPostLogoutRedirectUri { get; set; } = new TableDefinition("ClientPostLogoutRedirectUris");

        /// <summary>
        /// Gets or sets the client scopes table configuration.
        /// </summary>
        /// <value>
        /// The client scopes.
        /// </value>
        public TableDefinition ClientScopes { get; set; } = new TableDefinition("ClientScopes");

        /// <summary>
        /// Gets or sets the client secret table configuration.
        /// </summary>
        /// <value>
        /// The client secret.
        /// </value>
        public TableDefinition ClientSecret { get; set; } = new TableDefinition("ClientSecrets");

        /// <summary>
        /// Gets or sets the client claim table configuration.
        /// </summary>
        /// <value>
        /// The client claim.
        /// </value>
        public TableDefinition ClientClaim { get; set; } = new TableDefinition("ClientClaims");

        /// <summary>
        /// Gets or sets the client IdP restriction table configuration.
        /// </summary>
        /// <value>
        /// The client IdP restriction.
        /// </value>
        public TableDefinition ClientIdPRestriction { get; set; } = new TableDefinition("ClientIdPRestrictions");

        /// <summary>
        /// Gets or sets the client cors origin table configuration.
        /// </summary>
        /// <value>
        /// The client cors origin.
        /// </value>
        public TableDefinition ClientCorsOrigin { get; set; } = new TableDefinition("ClientCorsOrigins");

        /// <summary>
        /// Gets or sets the client property table configuration.
        /// </summary>
        /// <value>
        /// The client property.
        /// </value>
        public TableDefinition ClientProperty { get; set; } = new TableDefinition("ClientProperties");

        /// <summary>
        /// Gets or sets the API resource property table configuration.
        /// </summary>
        /// <value>
        /// The Api Resource property.
        /// </value>
        public TableDefinition ApiResourceProperty { get; set; } = new TableDefinition("ApiProperties");

        /// <summary>
        /// Gets or sets the identity resource property table configuration.
        /// </summary>
        /// <value>
        /// The Identity Resource property.
        /// </value>
        public TableDefinition IdentityResourceProperty { get; set; } = new TableDefinition("IdentityProperties");
    }
}
