using AutoFixture;
using AutoFixture.AutoFakeItEasy;
using AutoFixture.Xunit;


namespace MrCMS.Tests
{
    public class AutoFakeItEasyDataAttribute : AutoDataAttribute
    {
        public AutoFakeItEasyDataAttribute() : base(new Fixture().Customize(new AutoFakeItEasyCustomization()))
        {
        }
    }
}