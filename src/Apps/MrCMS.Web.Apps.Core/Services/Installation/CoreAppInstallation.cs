using System.Threading.Tasks;
using MrCMS.Installation;
using MrCMS.Installation.Models;
using MrCMS.Installation.Services;

namespace MrCMS.Web.Apps.Core.Services.Installation
{
    public class CoreAppInstallation : IOnInstallation
    {
        private readonly ISetupCoreLayouts _setupCoreLayouts;
        private readonly ISetupCoreWebpages _setupCoreWebpages;
        private readonly ISetupCoreMedia _setupCoreMedia;

        public CoreAppInstallation(ISetupCoreLayouts setupCoreLayouts, ISetupCoreWebpages setupCoreWebpages, ISetupCoreMedia setupCoreMedia)
        {
            _setupCoreLayouts = setupCoreLayouts;
            _setupCoreWebpages = setupCoreWebpages;
            _setupCoreMedia = setupCoreMedia;
        }

        public int Priority
        {
            get { return -1; }
        }

        public async Task Install(InstallModel model)
        {
            //settings
            await _setupCoreLayouts.Setup();
            await _setupCoreMedia.Setup();
            await _setupCoreWebpages.Setup();
        }
    }

}