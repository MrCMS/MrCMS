using System.Globalization;

namespace MrCMS.Services.Resources
{
    public interface IGetCurrentUserCultureInfo
    {
        CultureInfo Get();
        string GetInfoString();
    }
}