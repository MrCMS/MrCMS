namespace MrCMS.Installation
{
    public interface IOnInstallation
    {
        /// <summary>
        /// The order the installation occurs in (lower is earlier)
        /// </summary>
        int Priority { get; }
        void Install(InstallModel model);
    }
}