using System.Collections.Generic;

namespace MrCMS.Data
{
    public interface IGetValidators
    {
        IEnumerable<DatabaseValidatorBase> GetDatabaseValidators<TSubtype>();
        IEnumerable<EntityValidatorBase> GetEntityValidators<TSubtype>();
    }
}