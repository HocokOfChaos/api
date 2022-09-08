using AutoMapper;
using RoshdefAPI.AutoMapper;

namespace RoshdefAPI.Test
{
    [TestFixture]
    public class AutoMapperTests
    {
        MapperConfiguration _configuration;

        [SetUp]
        public void Setup()
        {
            _configuration = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new AutoMapperProfile());
            });
        }

        [Test]
        public void AssertConfigurationIsValid_ShouldNotThrow_WhenExecuted()
        {
            Assert.DoesNotThrow(() =>
            {
                _configuration.AssertConfigurationIsValid();
            });
        }
    }
}