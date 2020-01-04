using Microsoft.EntityFrameworkCore;

namespace MrCMS.Data
{
    public interface IMrCmsContextResolver
    {
        DbContext Resolve();
    }
}