using System.Collections.Generic;
using System.Linq;
using MrCMS.Helpers;
using MrCMS.Services;
using MrCMS.Settings;
using Ninject;
using Ninject.Modules;
using Ninject.Web.Common;

namespace MrCMS.IoC.Modules
{
    public class FileSystemModule : NinjectModule
    {
        public override void Load()
        {
            // Allowing IFileSystem implementation to be set in the site settings
            Kernel.Rebind<IFileSystem>().ToMethod(context =>
            {
                string storageType = context.Kernel.Get<FileSystemSettings>().StorageType;
                if (!string.IsNullOrWhiteSpace(storageType))
                    return context.Kernel.Get(TypeHelper.GetTypeByName(storageType)) as IFileSystem;
                return context.Kernel.Get<FileSystem>();
            }).InRequestScope();
            Kernel.Rebind<IEnumerable<IFileSystem>>().ToMethod(context => TypeHelper
                .GetAllTypesAssignableFrom<IFileSystem>()
                .Select(
                    type =>
                        context.Kernel.Get(type) as
                            IFileSystem)).InRequestScope();

        }
    }
}