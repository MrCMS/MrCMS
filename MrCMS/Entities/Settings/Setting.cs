using MrCMS.Entities.Multisite;

namespace MrCMS.Entities.Settings
{
    /// <summary>
    /// Represents a setting
    /// </summary>
    public partial class Setting : SiteEntity
    {
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the value
        /// </summary>
        public virtual string Value { get; set; }
    }
}