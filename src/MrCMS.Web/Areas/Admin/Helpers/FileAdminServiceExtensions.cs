using System;
using System.Linq;
using MrCMS.Entities.Documents.Media;
using MrCMS.Web.Areas.Admin.Models;

namespace MrCMS.Web.Areas.Admin.Helpers
{
    public static class FileAdminServiceExtensions
    {
        public static IQueryable<MediaFile> OrderBy(this IQueryable<MediaFile> query,
            MediaCategorySortMethod sortBy)
        {
            switch (sortBy)
            {
                case MediaCategorySortMethod.CreatedOnDesc:
                    return query.OrderByDescending(file => file.CreatedOn);
                case MediaCategorySortMethod.CreatedOn:
                    return query.OrderBy(file => file.CreatedOn);
                case MediaCategorySortMethod.Name:
                    return query.OrderBy(file => file.FileName);
                case MediaCategorySortMethod.NameDesc:
                    return query.OrderByDescending(file => file.FileName);
                case MediaCategorySortMethod.DisplayOrderDesc:
                    return query.OrderByDescending(file => file.DisplayOrder);
                case MediaCategorySortMethod.DisplayOrder:
                    return query.OrderBy(file => file.DisplayOrder);
                default:
                    throw new ArgumentOutOfRangeException("sortBy");
            }
        }

        public static IQueryable<MediaCategory> OrderBy(
            this IQueryable<MediaCategory> query,
            MediaCategorySortMethod sortBy)
        {
            switch (sortBy)
            {
                case MediaCategorySortMethod.CreatedOnDesc:
                    return query.OrderByDescending(category => category.CreatedOn);
                case MediaCategorySortMethod.CreatedOn:
                    return query.OrderBy(category => category.CreatedOn);
                case MediaCategorySortMethod.Name:
                    return query.OrderBy(category => category.Name);
                case MediaCategorySortMethod.NameDesc:
                    return query.OrderByDescending(category => category.Name);
                case MediaCategorySortMethod.DisplayOrderDesc:
                    return query.OrderByDescending(category => category.DisplayOrder);
                case MediaCategorySortMethod.DisplayOrder:
                    return query.OrderBy(category => category.DisplayOrder);
                default:
                    throw new ArgumentOutOfRangeException("sortBy");
            }
        }
    }
}