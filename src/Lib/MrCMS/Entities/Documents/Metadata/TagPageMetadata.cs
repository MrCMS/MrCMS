using System;
using System.Collections.Generic;
using MrCMS.Entities.Documents.Web;

namespace MrCMS.Entities.Documents.Metadata
{
    public class TagPageMetadata : WebpageMetadataMap<TagPage>
    {
        public override string WebGetController => "TagPage";

        public override bool RevealInNavigation => false;
        public override bool RequiresParent => true;
        public override ChildrenListType ChildrenListType => ChildrenListType.WhiteList;
        public override bool AutoBlacklist => true;
        public override bool Sortable => false;
        public override IEnumerable<Type> ChildrenList
        {
            get { yield break; }
        }
    }
}