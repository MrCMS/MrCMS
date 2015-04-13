using System;
using System.Collections.Generic;
using System.Linq;
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
            var primarySearchTerms = _getMediaCategorySearchTerms.SelectMany(terms => terms.GetPrimary(entity)).ToHashSet();
            var secondarySearchTerms = _getMediaCategorySearchTerms.SelectMany(terms => terms.GetSecondary(entity)).ToHashSet();
            return GetUniversalSearchItem(entity, primarySearchTerms, secondarySearchTerms);
        }

        private UniversalSearchItem GetUniversalSearchItem(MediaCategory entity, HashSet<string> primarySearchTerms, HashSet<string> secondarySearchTerms)
        {
            return new UniversalSearchItem
            {
                DisplayName = entity.Name,
                Id = entity.Id,
                PrimarySearchTerms = primarySearchTerms,
                SecondarySearchTerms = secondarySearchTerms,
                SystemType = typeof (MediaCategory).FullName,
                ActionUrl = "/admin/mediacategory/show/"+entity.Id, // _urlHelper.Action("Show", "MediaCategory", new {id = entity.Id, area = "admin"}),
                CreatedOn = entity.CreatedOn
            };
        }

        public override HashSet<UniversalSearchItem> GetSearchItems(HashSet<MediaCategory> entities)
        {
            Dictionary<MediaCategory, HashSet<string>> primaryDictionary = new Dictionary<MediaCategory, HashSet<string>>();
            foreach (var getMediaCategorySearchTerms in _getMediaCategorySearchTerms)
            {
                var terms = getMediaCategorySearchTerms.GetPrimary(entities);
                foreach (var key in terms.Keys)
                {
                    if (primaryDictionary.ContainsKey(key))
                    {
                        primaryDictionary[key].AddRange(terms[key]);
                    }
                    primaryDictionary[key] = terms[key];
                }
            }
            Dictionary<MediaCategory, HashSet<string>> secondaryDictionary = new Dictionary<MediaCategory, HashSet<string>>();
            foreach (var getMediaCategorySearchTerms in _getMediaCategorySearchTerms)
            {
                var terms = getMediaCategorySearchTerms.GetSecondary(entities);
                foreach (var key in terms.Keys)
                {
                    if (secondaryDictionary.ContainsKey(key))
                    {
                        secondaryDictionary[key].AddRange(terms[key]);
                    }
                    secondaryDictionary[key] = terms[key];
                }
            }

            return
                entities.Select(category =>
                        GetUniversalSearchItem(category,
                            primaryDictionary.ContainsKey(category) ? primaryDictionary[category] : new HashSet<string>(),
                            secondaryDictionary.ContainsKey(category) ? secondaryDictionary[category] : new HashSet<string>()
                            )).ToHashSet();
        }
    }
}