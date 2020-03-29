using AutoMapper;
using MrCMS.Indexing.Management;
using MrCMS.Web.Areas.Admin.Models;

namespace MrCMS.Web.Areas.Admin.Mapping
{
    public class LuceneFieldBoostAdminProfile : Profile
    {
        public LuceneFieldBoostAdminProfile()
        {
            CreateMap<LuceneFieldBoost, UpdateLuceneFieldBoostModel>().ReverseMap();
        }
    }
}