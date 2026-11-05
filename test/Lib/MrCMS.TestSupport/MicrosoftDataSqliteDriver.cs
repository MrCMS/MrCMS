using System.Data.Common;
using NHibernate.Driver;
using NHibernate.SqlTypes;

namespace MrCMS.TestSupport
{
    /// <summary>
    /// NHibernate driver for Microsoft.Data.Sqlite, which (unlike System.Data.SQLite) ships native
    /// SQLite builds for every platform the tests run on, including Apple Silicon.
    /// </summary>
    public class MicrosoftDataSqliteDriver : ReflectionBasedDriver
    {
        public MicrosoftDataSqliteDriver()
            : base("Microsoft.Data.Sqlite", "Microsoft.Data.Sqlite", "Microsoft.Data.Sqlite.SqliteConnection",
                "Microsoft.Data.Sqlite.SqliteCommand")
        {
        }

        public override bool UseNamedPrefixInSql => true;
        public override bool UseNamedPrefixInParameter => true;
        public override string NamedPrefix => "@";
        public override bool SupportsMultipleOpenReaders => false;
        public override bool SupportsMultipleQueries => true;

        protected override void InitializeParameter(DbParameter dbParam, string name, SqlType sqlType)
        {
            base.InitializeParameter(dbParam, name, sqlType);
            // NHibernate's SQLite dialect stores Guids as text, matching System.Data.SQLite's behaviour
            if (sqlType.DbType == System.Data.DbType.Guid)
                dbParam.DbType = System.Data.DbType.String;
        }
    }
}

namespace MrCMS.TestSupport
{
    /// <summary>
    /// SQLite dialect whose schema reader does not ask the connection for its 'DataTypes' collection,
    /// which Microsoft.Data.Sqlite does not provide. NHibernate's own SQLite keyword list is still used for auto-quoting.
    /// </summary>
    public class MicrosoftDataSqliteDialect : NHibernate.Dialect.SQLiteDialect
    {
        public MicrosoftDataSqliteDialect()
        {
            // the base SQLite dialect has no mapping for DateTimeOffset columns (stored as ISO-8601 text)
            RegisterColumnType(System.Data.DbType.DateTimeOffset, "DATETIME");
        }

        public override NHibernate.Dialect.Schema.IDataBaseSchema GetDataBaseSchema(DbConnection connection)
        {
            return new Schema(connection, this);
        }

        private class Schema : NHibernate.Dialect.Schema.SQLiteDataBaseMetaData
        {
            public Schema(DbConnection connection, NHibernate.Dialect.Dialect dialect) : base(connection, dialect)
            {
            }

            public override System.Collections.Generic.ISet<string> GetReservedWords()
            {
                return new System.Collections.Generic.HashSet<string>(System.StringComparer.OrdinalIgnoreCase);
            }
        }
    }
}
