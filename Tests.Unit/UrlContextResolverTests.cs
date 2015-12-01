using Collinson.Logging.WebApi;
using NUnit.Framework;

namespace Tests.Unit
{
    public class UrlContextResolverTests
    {
        private UrlContextResolver contextResolver;

        [SetUp]
        public void SetUp()
        {
            contextResolver = new UrlContextResolver();
        }

        [Test]
        public void WhenMethodMappedAction_ShouldReturnControllerAndMethod()
        {
            var urlLocalPath = "/eventoutcome";
            var result = contextResolver.Resolve(urlLocalPath, "get");

            Assert.That(result, Is.EqualTo("eventoutcome.get"));
        }

        [Test]
        public void WhenEntityInstanceParentCollectionUrl_ShouldReturnControllerAndAction()
        {
            var urlLocalPath = "/entityinstance/member";
            var result = contextResolver.Resolve(urlLocalPath, "get");

            Assert.That(result, Is.EqualTo("entityinstance.member"));
        }

        [Test]
        public void WhenMixedCaseUrl_ShouldReturnLowered()
        {
            var urlLocalPath = "/EntityInstance/Member";
            var result = contextResolver.Resolve(urlLocalPath, "get");

            Assert.That(result, Is.EqualTo("entityinstance.member"));
        }

        [Test]
        public void WhenEntityInstanceResourceUrl_AllLevelsNonNumeric_ShouldReturnControllerAndAction()
        {
            var urlLocalPath = "/entityinstance/member/19";
            var result = contextResolver.Resolve(urlLocalPath, "get");

            Assert.That(result, Is.EqualTo("entityinstance.member"));
        }

        [Test]
        public void WhenFixedMeasureUrl_ThirdAndFourthLevelNonNumeric_ShouldReturnMeasureAndAllocationType()
        {
            var urlLocalPath = "/member/19/fixedmeasure/tier";
            var result = contextResolver.Resolve(urlLocalPath, "get");

            Assert.That(result, Is.EqualTo("fixedmeasure.tier"));
        }

        [Test]
        public void WhenEntityInstanceParentChildUrl_ThirdLevelIsNumeric_ShouldReturnControllerAndChildEntity()
        {
            var urlLocalPath = "/entityinstance/member/19/awardpoints";
            var result = contextResolver.Resolve(urlLocalPath, "get");

            Assert.That(result, Is.EqualTo("entityinstance.awardpoints"));
        }
    }
}
