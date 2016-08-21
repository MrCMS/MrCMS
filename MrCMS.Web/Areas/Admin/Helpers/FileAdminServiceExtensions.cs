using System;
using MrCMS.Entities.Documents.Media;
using MrCMS.Web.Areas.Admin.Models;
using NHibernate;

namespace MrCMS.Web.Areas.Admin.Helpers
{
    public static class FileAdminServiceExtensions
    {
        public static IQueryOver<MediaFile, MediaFile> OrderBy(this IQueryOver<MediaFile, MediaFile> query,
            MediaCategorySortMethod sortBy)
        {
            switch (sortBy)
            {
                case MediaCategorySortMethod.CreatedOnDesc:
                    return query.OrderBy(file => file.CreatedOn).Desc;
                case MediaCategorySortMethod.CreatedOn:
                    return query.OrderBy(file => file.CreatedOn).Asc;
                case MediaCategorySortMethod.Name:
                    return query.OrderBy(file => file.FileName).Asc;
                case MediaCategorySortMethod.NameDesc:
                    return query.OrderBy(file => file.FileName).Desc;
                case MediaCategorySortMethod.DisplayOrderDesc:
                    return query.OrderBy(file => file.DisplayOrder).Desc;
                case MediaCategorySortMethod.DisplayOrder:
                    return query.OrderBy(file => file.DisplayOrder).Asc;
                default:
                    throw new ArgumentOutOfRangeException("sortBy");
            }
        }

        public static IQueryOver<MediaCategory, MediaCategory> OrderBy(
            this IQueryOver<MediaCategory, MediaCategory> query,
            MediaCategorySortMethod sortBy)
        {
            switch (sortBy)
            {
                case MediaCategorySortMethod.CreatedOnDesc:
                    return query.OrderBy(category => category.CreatedOn).Desc;
                case MediaCategorySortMethod.CreatedOn:
                    return query.OrderBy(category => category.CreatedOn).Asc;
                case MediaCategorySortMethod.Name:
                    return query.OrderBy(category => category.Name).Asc;
                case MediaCategorySortMethod.NameDesc:
                    return query.OrderBy(category => category.Name).Desc;
                case MediaCategorySortMethod.DisplayOrderDesc:
                    return query.OrderBy(category => category.DisplayOrder).Desc;
                case MediaCategorySortMethod.DisplayOrder:
                    return query.OrderBy(category => category.DisplayOrder).Asc;
                default:
                    throw new ArgumentOutOfRangeException("sortBy");
            }
        }
    }
}