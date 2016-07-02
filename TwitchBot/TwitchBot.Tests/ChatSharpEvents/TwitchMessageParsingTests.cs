using ChatSharp.Events;
using NUnit.Framework;

namespace TwitchBot.Tests.ChatSharpEvents
{
    [TestFixture]
    public class TwitchMessageParsingTests
    {
        [Test]
        public void UserDetailsShouldBeCorrectlyParsed()
        {
            var rawMessage = "@badges=broadcaster/1,turbo/1;color=#C0FFEE;display-name=ironiclensflare;emotes=;id=8e3692e2-7c85-4e64-9050-605be43d9e94;mod=0;room-id=51138303;subscriber=0;turbo=1;user-id=51138303;user-type= :ironiclensflare!ironiclensflare@ironiclensflare.tmi.twitch.tv PRIVMSG #ironiclensflare :o/";

            var eventArgs = new TwitchMessageEventArgs(rawMessage);

            Assert.AreEqual("#C0FFEE", eventArgs.Color);
            Assert.AreEqual("ironiclensflare", eventArgs.Username);
            Assert.AreEqual("o/", eventArgs.Message);
            Assert.True(eventArgs.Mod);
            Assert.False(eventArgs.Subscriber);
            Assert.True(eventArgs.Turbo);
        }

        [Test]
        public void MessagesWithColonsShouldBeCorrectlyParsed()
        {
            var rawMessage = "@badges=broadcaster/1,turbo/1;color=#C0FFEE;display-name=ironiclensflare;emotes=3:0-1;id=09af1c2d-4631-41d5-a828-9ed0ff885e1c;mod=0;room-id=51138303;subscriber=0;turbo=1;user-id=51138303;user-type= :ironiclensflare!ironiclensflare@ironiclensflare.tmi.twitch.tv PRIVMSG #ironiclensflare ::D";

            var eventArgs = new TwitchMessageEventArgs(rawMessage);

            Assert.AreEqual("#C0FFEE", eventArgs.Color);
            Assert.AreEqual("ironiclensflare", eventArgs.Username);
            Assert.AreEqual(":D", eventArgs.Message);
            Assert.True(eventArgs.Mod);
            Assert.False(eventArgs.Subscriber);
            Assert.True(eventArgs.Turbo);
        }

        [Test]
        public void MessagesWithEmotesShouldBeCorrectlyParsed()
        {
            var rawMessage = "@badges=broadcaster/1,turbo/1;color=#C0FFEE;display-name=ironiclensflare;emotes=3:0-1;id=09af1c2d-4631-41d5-a828-9ed0ff885e1c;mod=0;room-id=51138303;subscriber=0;turbo=1;user-id=51138303;user-type= :ironiclensflare!ironiclensflare@ironiclensflare.tmi.twitch.tv PRIVMSG #ironiclensflare :Hey guys :D :P :)";

            var eventArgs = new TwitchMessageEventArgs(rawMessage);

            Assert.AreEqual("#C0FFEE", eventArgs.Color);
            Assert.AreEqual("ironiclensflare", eventArgs.Username);
            Assert.AreEqual("Hey guys :D :P :)", eventArgs.Message);
            Assert.True(eventArgs.Mod);
            Assert.False(eventArgs.Subscriber);
            Assert.True(eventArgs.Turbo);
        }
    }
}
