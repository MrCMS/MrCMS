using System;
using MrCMS.Entities.Multisite;

namespace MrCMS.Entities.Settings
{
    /// <summary>
    /// Represents a setting
    /// </summary>
    //[Obsolete("Settings are now stored as JSON in App_Data, this class is left in to allow auto-migration from the DB based " +
    //          "settings of previous versions, but will be removed in a later version.")]
    public class Setting : SiteEntity
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