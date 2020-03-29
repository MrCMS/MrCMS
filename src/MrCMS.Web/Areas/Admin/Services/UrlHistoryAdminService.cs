using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MrCMS.Data;
using MrCMS.Entities.Documents.Web;
using MrCMS.Web.Areas.Admin.Models;

namespace MrCMS.Web.Areas.Admin.Services
{
    public class UrlHistoryAdminService : IUrlHistoryAdminService
    {
        private readonly IRepository<UrlHistory> _repository;
        private readonly IMapper _mapper;

        public UrlHistoryAdminService(IRepository<UrlHistory> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<UrlHistory> Delete(int id)
        {
            var urlHistory = await _repository.Load(id);
            if (urlHistory == null)
            {
                return null;
            }

            urlHistory.Webpage?.Urls.Remove(urlHistory);
            await _repository.Delete(urlHistory);
            return urlHistory;
        }

        public async Task Add(AddUrlHistoryModel model)
        {
            var urlHistory = _mapper.Map<UrlHistory>(model);
            urlHistory.Webpage?.Urls.Add(urlHistory);
            await _repository.Add(urlHistory);
        }

        public UrlHistory GetByUrlSegment(string url)
        {
            return _repository.Query().SingleOrDefault(x => EF.Functions.Like(x.UrlSegment, url));
        }

        public AddUrlHistoryModel GetUrlHistoryToAdd(int webpageId)
        {
            return new AddUrlHistoryModel
            {
                WebpageId = webpageId
            };
        }

        public Task<List<UrlHistory>> GetByWebpageId(int webpageId)
        {
            return _repository.Readonly().Where(x => x.WebpageId == webpageId).ToListAsync();
        }
    }
}