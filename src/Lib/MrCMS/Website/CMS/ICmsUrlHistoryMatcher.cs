namespace MrCMS.Website.CMS
{
    public interface ICmsUrlHistoryMatcher
    {
        UrlHistoryLookupResult TryMatch(string path);
    }
}