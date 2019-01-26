using System.Globalization;
using MrCMS.Entities.People;

namespace MrCMS.Services.Resources
{
    public interface IGetUserCultureInfo
    {
        CultureInfo Get(User user);
        string GetInfoString(User user);
    }
}