using System.Globalization;
using MrCMS.Entities.People;

namespace MrCMS.Services.Resources
{
    public interface IGetUserCultureInfo
    {
        CultureInfo Get(string userUiCulture);
        string GetInfoString(string userUiCulture);
    }
}