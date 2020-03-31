using MrCMS.TestSupport;

namespace MrCMS.Web.Apps.Admin.Tests.Services
{
    public class TagAdminServiceTests : MrCMSTest
    {
        // todo - rewrite tests and refactor
        //[Fact]
        //public void TagAdminService_Search_ShouldReturnTagsStartingWithTerm()
        //{
        //    var tagService = new TagAdminService(Context);

        //    var tag1 = new Tag {Name = "tag-1", Site = CurrentSite};
        //    var tag2 = new Tag {Name = "tag-2", Site = CurrentSite};
        //    var tag3 = new Tag {Name = "not-the-same", Site = CurrentSite};

        //    Context.Transact(session =>
        //    {
        //        Context.SaveOrUpdate(tag1);
        //        Context.SaveOrUpdate(tag2);
        //        Context.SaveOrUpdate(tag3);
        //    });

        //    IEnumerable<AutoCompleteResult> tags = tagService.Search("tag");

        //    tags.Should().HaveCount(2);
        //    tags.First().label.Should().Be("tag-1");
        //    tags.Skip(1).First().label.Should().Be("tag-2");
        //}

        //public class FakeContainer : Document
        //{
        //    public virtual void SetTags(ISet<Tag> tags)
        //    {
        //        Tags = tags;
        //    }
        //}

        //public class FakeContainerItem : Document
        //{
        //}

    }
}