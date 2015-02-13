using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MrCMS.Entities.Documents.Media;
using MrCMS.Helpers;
using MrCMS.Search.Models;

namespace MrCMS.Search.ItemCreation
{
    public class GetMediaCategorySearchItem : GetUniversalSearchItemBase<MediaCategory>
    {
        private readonly IEnumerable<IGetMediaCategorySearchTerms> _getMediaCategorySearchTerms;

        public GetMediaCategorySearchItem(IEnumerable<IGetMediaCategorySearchTerms> getMediaCategorySearchTerms)
        {
            _getMediaCategorySearchTerms = getMediaCategorySearchTerms;
        }

        public override UniversalSearchItem GetSearchItem(MediaCategory entity)
        {
            var searchTerms = _getMediaCategorySearchTerms.SelectMany(terms => terms.Get(entity)).ToHashSet();
            return GetUniversalSearchItem(entity, searchTerms);
        }

        private UniversalSearchItem GetUniversalSearchItem(MediaCategory entity, HashSet<string> searchTerms)
        {
            return new UniversalSearchItem
            {
                DisplayName = entity.Name,
                Id = entity.Id,
                SearchTerms = searchTerms,
                SystemType = typeof(MediaCategory).FullName,
                ActionUrl = "/admin/mediacategory/show/" + entity.Id // _urlHelper.Action("Show", "MediaCategory", new {id = entity.Id, area = "admin"}),
            };
        }

        public override HashSet<UniversalSearchItem> GetSearchItems(HashSet<MediaCategory> entities)
        {
            Dictionary<MediaCategory, HashSet<string>> dictionary = new Dictionary<MediaCategory, HashSet<string>>();
            foreach (var getMediaCategorySearchTerms in _getMediaCategorySearchTerms)
            {
                var terms = getMediaCategorySearchTerms.Get(entities);
                foreach (var key in terms.Keys)
                {
                    if (dictionary.ContainsKey(key))
                    {
                        dictionary[key].AddRange(terms[key]);
                    }
                    dictionary[key] = terms[key];
                }
            }

            return
                entities.Select(category =>
                        GetUniversalSearchItem(category,
                            dictionary.ContainsKey(category) ? dictionary[category] : new HashSet<string>())).ToHashSet();
        }
    }
}