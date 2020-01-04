using System.Collections.Generic;
using System.Threading.Tasks;
using MrCMS.Entities;

namespace MrCMS.Data
{
    public abstract class OnDataUpdated<T> : OnDataSaved where T : IHaveId
    {
        public abstract Task Execute(ChangeInfo data);
    }
}