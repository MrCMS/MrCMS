using System;
using System.Collections.Generic;
using MrCMS.Entities.Documents;
using MrCMS.Entities.Documents.Metadata;
using MrCMS.Web.Apps.Core.Pages;

namespace MrCMS.Web.Apps.Core.Metadata
{
    public class LoginPageMetadata : DocumentMetadataMap<LoginPage>
    {
        public override string IconClass
        {
            get { return "glyphicon glyphicon-user"; }
        }
        public override string WebGetController
        {
            get { return "Login"; }
        }
        public override string WebPostController
        {
            get { return "Login"; }
        }
        public override ChildrenListType ChildrenListType
        {
            get { return ChildrenListType.WhiteList; }
        }

        public override IEnumerable<Type> ChildrenList
        {
            get
            {
                yield return typeof(ForgottenPasswordPage);
                yield return typeof(ResetPasswordPage);
                yield return typeof(TwoFactorCodePage);
            }
        }
    }
}