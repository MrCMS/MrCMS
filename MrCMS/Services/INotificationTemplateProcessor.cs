namespace MrCMS.Services
{
    public interface INotificationTemplateProcessor
    {
        string ReplaceTokensAndMethods<T>(T tokenProvider, string template);
    }
}