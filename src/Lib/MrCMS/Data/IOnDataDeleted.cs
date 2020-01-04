using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Entities;

namespace MrCMS.Data
{
    public abstract class OnDataDeleted<T> : OnDataSaved where T : IHaveId
    {
        public abstract Task Execute(EntityData data);
    }
}