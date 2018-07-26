using MrCMS.Batching.Entities;

namespace MrCMS.Services.FileMigration
{
    public class MigrateFilesBatchJob : BatchJob
    {
        public override string Name
        {
            get { return "Migrate Files"; }
        }
    }
}