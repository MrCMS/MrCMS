using System.Collections.Generic;

namespace MrCMS.DbConfiguration
{
    public interface IGetModelCreators
    {
        IEnumerable<ICreateModel> GetCreators();
    }
}