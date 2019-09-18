using System;

namespace MrCMS.Web.IdentityServer.NHibernate.Storage.Options
{  
    /// <summary>
    /// Class to control a table definition (name and schema).
    /// </summary>
    public class TableDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableDefinition"/> class.
        /// </summary>
        /// <param name="name">The table name.</param>
        public TableDefinition(string name)
            : this(name, string.Empty)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TableDefinition"/> class.
        /// </summary>
        /// <param name="name">The table name.</param>
        /// <param name="schema">The table's schema name.</param>
        public TableDefinition(string name, string schema)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }
            Name = name;
            Schema = schema;
        }

        /// <summary>
        /// Gets the table name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the table's schema name.
        /// </summary>
        public string Schema { get; private set; }
    }
}