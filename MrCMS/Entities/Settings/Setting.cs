using MrCMS.Helpers;

namespace MrCMS.Entities.Settings
{
    /// <summary>
    /// Represents a setting
    /// </summary>
    public partial class Setting : BaseEntity
    {
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets the value
        /// </summary>
        public virtual string Value { get; set; }


        public virtual T ValueAs<T>()
        {
            return Value.ConvertTo<T>();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}