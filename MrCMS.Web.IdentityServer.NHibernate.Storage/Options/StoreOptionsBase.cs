namespace MrCMS.Web.IdentityServer.NHibernate.Storage.Options
{
    /// <summary>
    /// Base class for database configuration options.
    /// </summary>
    public abstract class StoreOptionsBase
    {
        /// <summary>
        /// Gets or sets the default schema for Configuration store's objects.
        /// </summary>
        /// <value>
        /// The default schema.
        /// </value>
        public string DefaultSchema { get; set; } = null;
    }
}
