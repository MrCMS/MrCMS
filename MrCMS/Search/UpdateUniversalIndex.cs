using MrCMS.Entities;
using MrCMS.Website;

namespace MrCMS.Search
{
    public class UpdateUniversalIndex : EndRequestTask<SystemEntity>
    {
        public UpdateUniversalIndex(SystemEntity entity)
            : base(entity)
        {

        }
    }
}