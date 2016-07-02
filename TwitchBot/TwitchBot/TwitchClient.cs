using System;
using ChatSharp;
using ChatSharp.Events;
using log4net;

namespace TwitchBot
{
    public class TwitchClient
    {
        private readonly IrcClient _irc;
        private readonly string _botName;
        private readonly string _channelName;
        private readonly ILog _log;

        public TwitchClient(string server, string botName, string password, string channelName)
        {
            _irc = new IrcClient(server, new IrcUser(botName, botName, password));
            _botName = botName;
            _channelName = channelName.StartsWith("#") ? channelName : "#" + channelName;
            _log = LogManager.GetLogger("TwitchClient");
        }

        public TwitchClient(IrcClient irc, string channelName)
        {
            _irc = irc;
            _botName = irc.User.Nick;
            _channelName = channelName.StartsWith("#") ? channelName : "#" + channelName;
            _log = LogManager.GetLogger("TwitchClient");
        }

        /// <summary>
        /// Indicates whether the bot is active and will handle chat messages.
        /// </summary>
        public bool Active { get; set; }
        
        /// <summary>
        /// Connects the bot to IRC.
        /// </summary>
        public void Connect()
        {
            _irc.ConnectionComplete += (s, e) =>
            {
                _log.Info("Connected successfully!");
                _irc.SendRawMessage("CAP REQ :twitch.tv/membership");
                _irc.SendRawMessage("CAP REQ :twitch.tv/tags");
                _irc.SendRawMessage("CAP REQ :twitch.tv/commands");

                _log.Debug($"Attempting to join {_channelName}...");
                _irc.JoinChannel(_channelName);

                _log.Debug($"Successfully joined {_channelName}.");
                OnBotListening();
            };

            _irc.RawMessageRecieved += (sender, args) =>
            {
                _log.Debug(args.Message);
            };

            _irc.ModeChanged += ModeChanged;
            _irc.TwitchMessageReceived += TwitchMessageReceived;
            _irc.TwitchResubReceived += TwitchResubReceived;
            _irc.TwitchSubReceived += TwitchSubReceived;

            _log.Info($"Connecting to {_irc.ServerAddress}...");
            _irc.ConnectAsync();
        }

        private void TwitchSubReceived(object sender, TwitchSubEventArgs e)
        {
            OnUserSubbed(e);
        }

        private void TwitchResubReceived(object sender, TwitchResubEventArgs e)
        {
            OnUserResubbed(e);
        }

        private void TwitchMessageReceived(object sender, TwitchMessageEventArgs e)
        {
            OnMessageReceived(e);
        }

        /// <summary>
        /// Sends a message to the channel.
        /// </summary>
        /// <param name="message"></param>
        public void SendMessage(string message)
        {
            _irc.SendMessage(message, _channelName);
        }

        private void ModeChanged(object sender, ModeChangeEventArgs e)
        {
            _log.Debug($"Mode change: {e.Change}");
            var regex = new System.Text.RegularExpressions.Regex(@"([\+-]\w)\s(.*)");
            var match = regex.Match(e.Change);
            if (!match.Success) return;
            var operation = match.Groups[1].Value;
            var username = match.Groups[2].Value;
            if (operation.Equals("+o") && username.Equals(_botName)) OnBotOpped();
            else if (operation.Equals("-o") && username.Equals(_botName)) OnBotDeopped();
        }

        /// <summary>
        /// Triggered when the bot has successfully connected to a channel
        /// and is ready to start moderating.
        /// </summary>
        public event EventHandler BotReady;

        internal void OnBotReady()
        {
            BotReady?.Invoke(null, null);
        }

        /// <summary>
        /// Triggered when the bot is ready to perform read-only operations.
        /// </summary>
        public event EventHandler BotListening;
        internal void OnBotListening()
        {
            BotListening?.Invoke(null, null);
        }

        /// <summary>
        /// Triggered when the bot gains operator status in the channel.
        /// </summary>
        public event EventHandler BotOpped;

        internal void OnBotOpped()
        {
            BotOpped?.Invoke(null, null);
            if (!Active) BotReady?.Invoke(null, null);
            Active = true;
        }

        /// <summary>
        /// Triggered when the bot loses its operator status in the channel.
        /// </summary>
        public event EventHandler BotDeopped;

        internal void OnBotDeopped()
        {
            BotDeopped?.Invoke(null, null);
            Active = false;
        }

        /// <summary>
        /// Triggered when a new message is received in the channel.
        /// </summary>
        public event EventHandler<MessageEventArgs> NewMessage;

        internal void OnMessageReceived(TwitchMessageEventArgs e)
        {
            var messageArgs = new MessageEventArgs(e.Username, e.Message);
            messageArgs.IsMod = e.Mod;
            NewMessage?.Invoke(null, messageArgs);
        }

        public event EventHandler<TwitchResubEventArgs> UserResubbed;

        internal void OnUserResubbed(TwitchResubEventArgs e)
        {
            UserResubbed?.Invoke(this, e);
        }

        public event EventHandler<TwitchSubEventArgs> UserSubbed;

        internal void OnUserSubbed(TwitchSubEventArgs e)
        {
            UserSubbed?.Invoke(this, e);
        }
    }
}
