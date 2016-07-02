using System.Linq;
using System.Text.RegularExpressions;

namespace TwitchBot.Filters
{
    public class LinkFilter : IChatFilter
    {
        public LinkFilter()
        {
            Name = "Link Filter";
            Description = "Detects links posted in messages.";
            Enabled = true;
            ModExempt = false;
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public bool Enabled { get; set; }
        public bool ModExempt { get; set; }
        public string[] PermittedUsers { get; set; }

        public bool Match(string input)
        {
            var urlRegex =
                new Regex("[-a-zA-Z0-9@:%_\\+.~#?&\\/\\/=]{2,256}\\.[a-z]{2,4}\\b(\\/[-a-zA-Z0-9@:%_\\+.~#?&\\/\\/=]*)?");

            var extensionRegex = new Regex(@"\.(jpg|jpeg|gif|exe|avi|mp4)");

            var result = urlRegex.Match(input);
            var matchedString = result.ToString();

            // Make a concession for silly things like "nope.jpg"
            if (result.Success && matchedString.Count(r => r == '.') < 2 && extensionRegex.IsMatch(matchedString))
                return false;
            
            return result.Success;
        }
    }
}
