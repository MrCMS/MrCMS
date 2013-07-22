namespace MrCMS.Services
{
    public interface IMessageTemplateProcessor
    {
        string ReplaceTokensAndMethods<T>(T tokenProvider, string template);
    }
}