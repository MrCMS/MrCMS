using MrCMS.Entities.Documents.Media;
using MrCMS.Helpers;
using NHibernate;

namespace MrCMS.Services
{
    public class MediaCategoryService : IMediaCategoryService
    {
        private readonly ISession _session;

        public MediaCategoryService(ISession session)
        {
            _session = session;
        }

        public void SaveCategory(MediaCategory mediaCategory)
        {
            _session.Transact(session => session.SaveOrUpdate(mediaCategory));
        }

        public MediaCategory GetCategory(int id)
        {
            return _session.Get<MediaCategory>(id);
        }
    }
}