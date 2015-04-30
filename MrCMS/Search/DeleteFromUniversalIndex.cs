using MrCMS.Entities;
using MrCMS.Website;

namespace MrCMS.Search
{
    public class DeleteFromUniversalIndex : EndRequestTask<SystemEntity>
    {
        public DeleteFromUniversalIndex(SystemEntity entity)
            : base(entity)
        {
        }
    }
}