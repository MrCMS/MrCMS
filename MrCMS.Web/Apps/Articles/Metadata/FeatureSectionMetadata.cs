using System;
using System.Collections.Generic;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Metadata;
using MrCMS.Web.Apps.Articles.Pages;

namespace MrCMS.Web.Apps.Articles.Metadata
{
    public class FeatureSectionMetadata : DocumentMetadataMap<FeatureSection>
    {
        public override string WebGetController
        {
            get { return "FeatureSection"; }
        }
        public override string IconClass
        {
            get { return "icon-briefcase"; }
        }
        
        public override ChildrenListType ChildrenListType
        {
            get { return ChildrenListType.WhiteList; }
        }

        public override IEnumerable<Type> ChildrenList
        {
            get
            {
                yield return typeof (Feature);
                yield return typeof (FeatureSection);
            }
        }

        public override string DefaultLayoutName
        {
            get { return "Two Column"; }
        }

        public override SortBy SortBy
        {
            get
            {
                return SortBy.PublishedOnDesc;
            }
        }

        public override int MaxChildNodes
        {
            get { return 25; }
        }
    }
}