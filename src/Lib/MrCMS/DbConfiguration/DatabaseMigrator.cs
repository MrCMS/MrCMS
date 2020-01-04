using Microsoft.EntityFrameworkCore;

namespace MrCMS.DbConfiguration
{
    public class DatabaseMigrator : IDatabaseMigrator
    {
        private readonly ISystemDatabase _database;

        public DatabaseMigrator(ISystemDatabase database)
        {
            _database = database;
        }
        public void Migrate()
        {
            _database.Database.Migrate();
        }
    }
}