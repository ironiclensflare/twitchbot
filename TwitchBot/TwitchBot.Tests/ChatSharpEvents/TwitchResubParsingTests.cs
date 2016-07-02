using ChatSharp.Events;
using NUnit.Framework;

namespace TwitchBot.Tests.ChatSharpEvents
{
    [TestFixture]
    public class TwitchResubParsingTests
    {
        [Test]
        public void ResubsWithMessagesShouldBeCorrectlyParsed()
        {
            var rawMessage = @"@badges=staff/1,broadcaster/1,turbo/1;color=#008000;display-name=ironiclensflare;emotes=;mod=0;msg-id=resub;msg-param-months=6;room-id=1337;subscriber=1;system-msg=ironiclensflare\shas\ssubscribed\sfor\s6\smonths!;login=twitch_username;turbo=1;user-id=1337;user-type=staff :tmi.twitch.tv USERNOTICE #channel :Woo!";

            var eventArgs = new TwitchResubEventArgs(rawMessage);

            Assert.AreEqual("ironiclensflare", eventArgs.Username);
            Assert.AreEqual(6, eventArgs.Months);
            Assert.AreEqual("Woo!", eventArgs.Message);
        }

        [Test]
        public void ResubsWithoutMessagesShouldBeCorrectlyParsed()
        {
            var rawMessage = @"@badges=staff/1,broadcaster/1,turbo/1;color=#008000;display-name=ironiclensflare;emotes=;mod=0;msg-id=resub;msg-param-months=6;room-id=1337;subscriber=1;system-msg=ironiclensflare\shas\ssubscribed\sfor\s6\smonths!;login=twitch_username;turbo=1;user-id=1337;user-type=staff :tmi.twitch.tv USERNOTICE #channel";

            var eventArgs = new TwitchResubEventArgs(rawMessage);

            Assert.AreEqual("ironiclensflare", eventArgs.Username);
            Assert.AreEqual(6, eventArgs.Months);
            Assert.IsEmpty(eventArgs.Message);
        }
    }
}
