using NUnit.Framework;
using TwitchBot.Filters;

namespace TwitchBot.Tests.Filters
{
    [TestFixture]
    public class LinkFilterTests
    {
        [TestCase("http://www.google.com")]
        [TestCase("Check this out http://www.lolcats.net")]
        [TestCase("www.mysite.net")]
        [TestCase("Go to google.com and type this.")]
        [TestCase("http://www.dropbox.com/thing/flower.jpg")]
        [TestCase("https://www.ultrasecure.net/malware.exe")]
        public void ActualLinks_ShouldBeDetected(string input)
        {
            var filter = new LinkFilter();
            var matches = filter.Match(input);

            Assert.True(matches);
        }

        [TestCase("Awww.That's a shame")]
        [TestCase("I'm a .net developer")]
        [TestCase("fail.jpg")]
        [TestCase("nope.gif")]
        public void ThingsThatLookLikeLinks_ShouldNotBeDetected(string input)
        {
            var filter = new LinkFilter();
            var matches = filter.Match(input);

            Assert.False(matches);
        }
    }
}
