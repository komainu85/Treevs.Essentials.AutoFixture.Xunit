using System;

namespace Treevs.Essentials.AutoFixture.Xunit.Tests
{
    using global::Xunit;
    using global::Xunit.Extensions;

    using Ploeh.AutoFixture;

    using Treevs.Essentials.AutoFixture.Xunit.AutoSetup;

    public class AutoSetupAttributeSingleClassFixture
    {
        public static Action<IFixture> AutoSetup()
        {
            return (f) =>
                {
                    f.Customize<AutoSetupGlobalSut>(obj => obj.OmitAutoProperties().Do(sut => sut.Setup = true));

                    f.Customize<AutoSetupMethodSut>(obj => obj.OmitAutoProperties());
                    f.Customize<AutoSetupPropertySut>(obj => obj.OmitAutoProperties());
                    f.Customize<AutoSetupPlainMethodSut>(obj => obj.OmitAutoProperties());
                };
        }

        public static Action<IFixture> MethodSetup()
        {
            return (f) => f.Customize<AutoSetupMethodSut>(obj => obj
                .Do(sut => sut.Setup = true)
                .OmitAutoProperties());
        }

        public static Action<IFixture> PropertySetup
        {
            get
            {
                return (f) => f.Customize<AutoSetupPropertySut>(obj => obj
                    .Do(sut => sut.Setup = true)
                    .OmitAutoProperties());
            }
        }

        public static void PlainMethodSetup(IFixture fixture, AutoSetupGlobalSut globalSut, AutoSetupPlainMethodSut plainMethodSut)
        {
            Assert.True(globalSut.Setup);
            fixture.Customize<AutoSetupPlainMethodSut>(opt => opt.Do(sut => sut.Setup = true));
        }

        [Theory]
        [AutoSetup]
        public void ImplicitGlobalSetup(
            AutoSetupGlobalSut globalSut, 
            AutoSetupMethodSut methodSut, 
            AutoSetupPropertySut propertySut,
            AutoSetupPlainMethodSut plainMethodSut)
        {
            Assert.True(globalSut.Setup);
            Assert.False(methodSut.Setup);
            Assert.False(propertySut.Setup);
            Assert.False(plainMethodSut.Setup);
        }

        [Theory]
        [AutoSetup("AutoSetup")]
        public void ExplicitGlobalSetup(
            AutoSetupGlobalSut globalSut,
            AutoSetupMethodSut methodSut,
            AutoSetupPropertySut propertySut,
            AutoSetupPlainMethodSut plainMethodSut)
        {
            Assert.True(globalSut.Setup);
            Assert.False(methodSut.Setup);
            Assert.False(propertySut.Setup);
            Assert.False(plainMethodSut.Setup);
        }

        [Theory]
        [AutoSetup("AutoSetup", "MethodSetup")]
        public void ExplicitGlobalSetupWithMethodSetup(
            AutoSetupGlobalSut globalSut,
            AutoSetupMethodSut methodSut,
            AutoSetupPropertySut propertySut,
            AutoSetupPlainMethodSut plainMethodSut)
        {
            Assert.True(globalSut.Setup);
            Assert.True(methodSut.Setup);
            Assert.False(propertySut.Setup);
            Assert.False(plainMethodSut.Setup);
        }

        [Theory]
        [AutoSetup("MethodSetup")]
        public void GlobalSetupWithMethodSetup(
            AutoSetupGlobalSut globalSut,
            AutoSetupMethodSut methodSut,
            AutoSetupPropertySut propertySut,
            AutoSetupPlainMethodSut plainMethodSut)
        {
            Assert.True(globalSut.Setup);
            Assert.True(methodSut.Setup);
            Assert.False(propertySut.Setup);
            Assert.False(plainMethodSut.Setup);
        }

        [Theory]
        [AutoSetup("PropertySetup")]
        public void GlobalSetupWithPropertySetup(
            AutoSetupGlobalSut globalSut,
            AutoSetupMethodSut methodSut,
            AutoSetupPropertySut propertySut,
            AutoSetupPlainMethodSut plainMethodSut)
        {
            Assert.True(globalSut.Setup);
            Assert.False(methodSut.Setup);
            Assert.True(propertySut.Setup);
            Assert.False(plainMethodSut.Setup);
        }

        [Theory]
        [AutoSetup("PlainMethodSetup")]
        public void GlobalSetupWithPlainMethodSetup(
            AutoSetupGlobalSut globalSut,
            AutoSetupMethodSut methodSut,
            AutoSetupPropertySut propertySut,
            AutoSetupPlainMethodSut plainMethodSut)
        {
            Assert.True(globalSut.Setup);
            Assert.False(methodSut.Setup);
            Assert.False(propertySut.Setup);
            Assert.True(plainMethodSut.Setup);
        }

        [Theory]
        [AutoSetup("MethodSetup", "PropertySetup", "PlainMethodSetup")]
        public void GlobalSetupWithMultipleSetups(
            AutoSetupGlobalSut globalSut,
            AutoSetupMethodSut methodSut,
            AutoSetupPropertySut propertySut,
            AutoSetupPlainMethodSut plainMethodSut)
        {
            Assert.True(globalSut.Setup);
            Assert.True(methodSut.Setup);
            Assert.True(propertySut.Setup);
            Assert.True(plainMethodSut.Setup);
        }

        [Theory]
        [AutoSetup("MethodSetup", "PropertySetup", "PlainMethodSetup")]
        public void FixtureInstanceInjected(IFixture fixture)
        {
            Assert.True(fixture.Create<AutoSetupGlobalSut>().Setup);
            Assert.True(fixture.Create<AutoSetupMethodSut>().Setup);
            Assert.True(fixture.Create<AutoSetupPropertySut>().Setup);
            Assert.True(fixture.Create<AutoSetupPlainMethodSut>().Setup);
        }
    }
}
