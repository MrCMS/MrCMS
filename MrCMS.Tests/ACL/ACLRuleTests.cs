using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using MrCMS.ACL;
using MrCMS.Apps;
using MrCMS.Entities.People;
using MrCMS.Tests.Helpers;
using Xunit;

namespace MrCMS.Tests.ACL
{
    public class ACLRuleTests
    {
        [Fact]
        public void ACLRule_CanAccessUser_ShouldReturnTrueIfUserIsAdmin()
        {
            var testAclRule = new TestACLRule();

            testAclRule.CanAccess(new User { Roles = new List<UserRole> { new UserRole { Name = UserRole.Administrator } } },
                                  "test").Should().BeTrue();
        }

        [Fact]
        public void ACLRule_CanAccessRole_ShouldReturnTrueIfRoleIsAdmin()
        {
            var testAclRule = new TestACLRule();

            testAclRule.CanAccess(new UserRole { Name = UserRole.Administrator },
                                  "test").Should().BeTrue();
        }

        [Fact]
        public void ACLRule_AppName_IfAppIsNullShouldBeSystem()
        {
            var testAclRule = new TestACLRule();
            
            testAclRule.SetApp(null);

            testAclRule.AppName.Should().Be("System");
        }

        [Fact]
        public void ACLRule_AppName_IfTheAppIsSetShouldBeTheNameOfTheApp()
        {
            var testAclRule = new TestACLRule();
            
            testAclRule.SetApp(new TestApp());
            
            testAclRule.AppName.Should().Be("Test");
        }
    }

    public class TestACLRule : ACLRule
    {
        private MrCMSApp _app = null;

        public override string DisplayName
        {
            get { throw new NotImplementedException(); }
        }

        protected override List<string> GetOperations()
        {
            throw new NotImplementedException();
        }

        protected override MrCMSApp App { get { return _app; } }
        public void SetApp(MrCMSApp app)
        {
            _app = app;
        }
    }
}
