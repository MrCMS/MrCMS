using FakeItEasy;
using MrCMS.Services;
using Ninject;

namespace MrCMS.Tests.Services
{
    public class MessageTemplateParserTests : MrCMSTest
    {
        private readonly IKernel _kernel;
        private MessageTemplateParser _messageTemplateParser;

        public MessageTemplateParserTests()
        {
            _kernel = A.Fake<IKernel>();
            _messageTemplateParser = new MessageTemplateParser(_kernel);
        }
    }
}