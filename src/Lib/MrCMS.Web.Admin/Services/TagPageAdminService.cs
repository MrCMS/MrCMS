using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MrCMS.Entities.Documents.Web;
using MrCMS.Helpers;
using MrCMS.Models;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Criterion.Lambda;
using NHibernate.Transform;
using X.PagedList;

namespace MrCMS.Web.Admin.Services
{
    public class TagPageAdminService : ITagPageAdminService
    {
        private readonly ISession _session;

        public TagPageAdminService(ISession session)
        {
            _session = session;
        }

        public async Task<IList<AutoCompleteResult>> Search(string term)
        {
            AutoCompleteResult alias = null;
            return await _session.QueryOver<TagPage>()
                    .Where(x => x.Name.IsInsensitiveLike(term, MatchMode.Start))
                    .OrderBy(tag => tag.Name).Asc
                    .SelectList(builder =>
                    {
                        builder.Select(tag => tag.Id).WithAlias(() => alias.id);
                        builder.Select(tag => tag.Name).WithAlias(() => alias.label);
                        builder.Select(tag => tag.Name).WithAlias(() => alias.value);
                        return builder;
                    })
                    .TransformUsing(Transformers.AliasToBean<AutoCompleteResult>())
                    .Take(10)
                    .ListAsync<AutoCompleteResult>();
        }

        public Task<IPagedList<Select2LookupResult>> SearchPaged(string term, int page)
        {
            return _session.QueryOver<TagPage>()
                .Where(x => x.Name.IsInsensitiveLike(term, MatchMode.Start))
                .OrderBy(tag => tag.Name).Asc
                .PagedMappedAsync<TagPage, Select2LookupResult>(page, GetSelect2ProjectionBuilder());
        }

        private static Func<QueryOverProjectionBuilder<TagPage>, QueryOverProjectionBuilder<TagPage>>
            GetSelect2ProjectionBuilder()
        {
            Select2LookupResult alias = null;
            return builder =>
            {
                builder.Select(tag => tag.Id).WithAlias(() => alias.id);
                builder.Select(tag => tag.Name).WithAlias(() => alias.text);
                return builder;
            };
        }

        public async Task<IList<Webpage>> GetWebpages(TagPage page)
        {
            var webpages = new List<Webpage>();
            foreach (var item in page.Documents)
                webpages.Add(await _session.GetAsync<Webpage>(item.Id));
            return webpages.OrderByDescending(x => x.PublishOn).ToList();
        }

        public async Task<IList<Webpage>> GetWebpages(int pageId)
        {
            return await GetWebpages(await _session.GetAsync<TagPage>(pageId));
        }

        public async Task<Select2LookupResult> GetInfo(int id)
        {
            return await _session.QueryOver<TagPage>()
                .Where(x => x.Id == id)
                .SelectList(GetSelect2ProjectionBuilder())
                .TransformUsing(Transformers.AliasToBean<Select2LookupResult>())
                .SingleOrDefaultAsync<Select2LookupResult>();
        }
    }
}