using FakeItEasy;
using MrCMS.Services;
using MrCMS.Website.Controllers;

namespace MrCMS.Tests.Website.Controllers.Builders
{
    public class LogoutControllerBuilder
    {
        private IAuthorisationService _authorisationService = A.Fake<IAuthorisationService>();

        public LogoutController Build()
        {
            return new LogoutController(_authorisationService);
        }

        public LogoutControllerBuilder WithAuthorisationService(IAuthorisationService authorisationService)
        {
            _authorisationService = authorisationService;
            return this;
        }
    }
}