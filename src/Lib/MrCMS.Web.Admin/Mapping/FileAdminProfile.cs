using Mapster;
using MrCMS.Entities.Documents.Media;
using MrCMS.Web.Admin.Models;

namespace MrCMS.Web.Admin.Mapping
{
    public class FileAdminProfile : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<MediaFile, UpdateFileSEOModel>();
            config.NewConfig<UpdateFileSEOModel, MediaFile>();
        }
    }
}