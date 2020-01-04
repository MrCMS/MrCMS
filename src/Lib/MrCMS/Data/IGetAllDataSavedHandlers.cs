using System.Collections.Generic;

namespace MrCMS.Data
{
    public interface IGetAllDataSavedHandlers
    {
        ICollection<OnDataSaved> GetHandlers();
    }
}