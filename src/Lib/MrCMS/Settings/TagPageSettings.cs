namespace MrCMS.Settings
{
    public class TagPageSettings : SiteSettingsBase
    {
        public override bool RenderInSettings => true;

        public TagPageSettings()
        {
            TagPagesURLPrefix = "";
            TagPagesName = "tag/";
        }
        public string TagPagesURLPrefix { get; set; }

        public string TagPagesName { get; set; }

        public string TagPrefix
        {
            get { return $"{TagPagesURLPrefix}{TagPagesName}"; }
        }
    }
}