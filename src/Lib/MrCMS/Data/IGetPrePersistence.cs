using System.Collections.Generic;
using MrCMS.Entities;

namespace MrCMS.Data
{
    public interface IGetPrePersistence
    {
        IEnumerable<OnDataAdding> GetOnAdding<T>() where T : class;
        IEnumerable<OnDataUpdating> GetOnUpdating<T>() where T : class;
        IEnumerable<OnDataDeleting> GetOnDeleting<T>() where T : class;
    }
}