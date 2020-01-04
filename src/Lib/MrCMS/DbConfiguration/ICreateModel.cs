using Microsoft.EntityFrameworkCore;

namespace MrCMS.DbConfiguration
{
    public interface ICreateModel
    {
        void Create(ModelBuilder builder);
    }
}