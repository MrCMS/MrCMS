using Microsoft.Extensions.DependencyInjection;
using MrCMS.Services;
using MrCMS.Settings;

namespace MrCMS.Helpers
{
    public static class FileSystemServiceExtensions
    {
        public static IServiceCollection AddMrCMSFileSystem(this IServiceCollection services)
        {
            services.AddScoped<IFileSystem>(provider =>
            {
                var settings = provider.GetService<FileSystemSettings>();

                var storageType = settings.StorageType;
                if (string.IsNullOrWhiteSpace(storageType))
                {
                    return provider.GetService<FileSystem>();
                }

                var type = TypeHelper.GetTypeByName(storageType);
                if (type?.IsAssignableFrom(typeof(IFileSystem)) != true)
                {
                    return provider.GetService(type) as IFileSystem;
                }

                return provider.GetService(type) as IFileSystem;
            });

            return services;
        }

    }
}