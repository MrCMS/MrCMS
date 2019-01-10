namespace MrCMS.Website.CMS
{
    public interface ICmsMethodTester
    {
        bool IsRoutable(string httpMethod);
    }
}