using System.Collections.Generic;
using System.Web.Mvc;
using FakeItEasy;
using FluentAssertions;
using MrCMS.Shortcodes;
using MrCMS.Website;
using Ploeh.AutoFixture.Xunit;
using Xunit.Extensions;

namespace MrCMS.Tests.Shortcodes
{
    public class ShortcodeParserTests
    {
        [Theory]
        [AutoFakeItEasyData]
        public void Parse_NullContentReturnsEmptyString(
            ShortcodeParser sut
        )
        {
            sut.Parse(null, null).Should().Be(MvcHtmlString.Empty);
        }

        [Theory]
        [AutoFakeItEasyData]
        public void Parse_NoCodesShouldBeUnchanged(
            ShortcodeParser sut
        )
        {
            const string content =
                "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus eu faucibus leo. Morbi ligula tellus, convallis non lorem vel, ultricies viverra diam. Pellentesque eget ligula eleifend, venenatis purus nec, tincidunt ante. Maecenas nec pellentesque lectus, vel bibendum sem. Sed eget vulputate lacus, mollis convallis massa. Quisque in ante ut turpis euismod elementum interdum in purus. Phasellus viverra nec est ut scelerisque.";

            var result = sut.Parse(null, content);

            result.ToString().Should().Be(content);
        }

        [Theory]
        [AutoFakeItEasyData]
        public void Parse_MatchedContentIsReplaced(
            [Frozen] IRenderShortcode renderShortcode,
            ShortcodeParser sut,
            IHtmlHelper helper
        )
        {
            A.CallTo(() => renderShortcode.CanRender("test")).Returns(true);
            A.CallTo(() => renderShortcode.Render(helper, "test", A<Dictionary<string, string>>.Ignored))
                .Returns("Updated text");
            const string content = "[test]";

            var result = sut.Parse(helper, content);

            result.ToString().Should().Be("Updated text");
        }


        [Theory]
        [AutoFakeItEasyData]
        public void Parse_GetsAttributesFromShortcode(
            [Frozen] IRenderShortcode renderShortcode,
            ShortcodeParser sut,
            IHtmlHelper helper
        )
        {
            A.CallTo(() => renderShortcode.CanRender("test")).Returns(true);
            A.CallTo(() =>
                    renderShortcode.Render(helper, "test",
                        A<Dictionary<string, string>>.That.Matches(
                            dictionary => dictionary.ContainsKey("id") && dictionary["id"] == "123")))
                .Returns("Updated text");
            const string content = "[test id=\"123\"]";

            var result = sut.Parse(helper, content);

            result.ToString().Should().Be("Updated text");
        }

        [Theory]
        [AutoFakeItEasyData]
        public void Parse_ParsesMultipleAttributesSuccessfully(
            [Frozen] IRenderShortcode renderShortcode,
            ShortcodeParser sut,
            IHtmlHelper helper
        )
        {
            A.CallTo(() => renderShortcode.CanRender("test")).Returns(true);
            A.CallTo(() =>
                    renderShortcode.Render(helper, "test",
                        A<Dictionary<string, string>>.That.Matches(
                            dictionary =>
                                dictionary.ContainsKey("id") && dictionary["id"] == "123"
                                &&
                                dictionary.ContainsKey("more") && dictionary["more"] == "data"
                        )))
                .Returns("Updated text");
            const string content = "[test id=\"123\" more=\"data\"] Updated Text";

            var result = sut.Parse(helper, content);

            result.ToString().Should().Be(" Updated Text");
        }

        [Theory]
        [AutoFakeItEasyData]
        public void Parse_WithOutQuotes(
            [Frozen] IRenderShortcode renderShortcode,
            ShortcodeParser sut,
            IHtmlHelper helper
        )
        {
            A.CallTo(() => renderShortcode.CanRender("test")).Returns(true);
            A.CallTo(() =>
                renderShortcode.Render(helper, "test",
                    A<Dictionary<string, string>>.That.Matches(
                        dictionary =>
                            dictionary.ContainsKey("id") && dictionary["id"] == "123"
                            &&
                            !dictionary.ContainsKey("more")
                    )));


            const string content = "[test id=123] sdfs adfsdf sdaf sadf asdfsdaf sdf sdf  more=(*&)(&knkjdhnf data]";

            var result = sut.Parse(helper, content);

            result.ToString().Should().Be(" sdfs adfsdf sdaf sadf asdfsdaf sdf sdf  more=(*&)(&knkjdhnf data]");
        }

        [Theory]
        [AutoFakeItEasyData]
        public void Parse_WithQuotes(
            [Frozen] IRenderShortcode renderShortcode,
            ShortcodeParser sut,
            IHtmlHelper helper
        )
        {
            var content = "[test id=\"123\"] sdfs adfsdf sdaf sadf asdfsdaf sdf sdf  more=(*&)(&knkjdhnf data]";


            A.CallTo(() => renderShortcode.CanRender("test")).Returns(true);
            A.CallTo(() =>
                renderShortcode.Render(helper, "test ",
                    A<Dictionary<string, string>>.That.Matches(
                        dictionary =>
                            dictionary.ContainsKey("id") && dictionary["id"] == "123"
                            &&
                            !dictionary.ContainsKey("more")
                    )));

            var result = sut.Parse(helper, content);

            result.ToString().Should().Be(" sdfs adfsdf sdaf sadf asdfsdaf sdf sdf  more=(*&)(&knkjdhnf data]");
        }
    }
}