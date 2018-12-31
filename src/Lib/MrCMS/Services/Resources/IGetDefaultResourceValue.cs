namespace MrCMS.Services.Resources
{
    public interface IGetDefaultResourceValue
    {
        string GetValue(MissingLocalisationInfo info);
    }
}