using FakeItEasy;
using MrCMS.Services;
using MrCMS.Services.Auth;
using MrCMS.Website.Controllers;

namespace MrCMS.Tests.Website.Controllers.Builders
{
    public class LogoutControllerBuilder
    {
        private ISignInManager _signInManager = A.Fake<ISignInManager>();
        private IEventContext _eventContext = A.Fake<IEventContext>();

        public LogoutController Build()
        {
            return new LogoutController(_signInManager, _eventContext);
        }

        public LogoutControllerBuilder WithSignInManager(ISignInManager signInManager)
        {
            _signInManager = signInManager;
            return this;
        }
    }
}