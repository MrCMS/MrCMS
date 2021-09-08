namespace MrCMS.Tests.Helpers
{
    public class ImplementationOfGenericInterface : GenericInterface<string>
    {
        public string Test => "Test";
    }
}