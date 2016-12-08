namespace MrCMS.Entities.Settings
{
    /// <summary>
    ///     Represents a setting
    /// </summary>
    public class Setting : SiteEntity
    {
        /// <summary>
        ///     Gets or sets the type
        /// </summary>
        public virtual string SettingType { get; set; }

        /// <summary>
        ///     Gets or sets the property name
        /// </summary>
        public virtual string PropertyName { get; set; }

        ///// <summary>
        /////     Gets or sets the name
        ///// </summary>
        //public virtual string Name { get; set; }

        /// <summary>
        ///     Gets or sets the value
        /// </summary>
        public virtual string Value { get; set; }
    }
}