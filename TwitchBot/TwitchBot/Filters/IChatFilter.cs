namespace TwitchBot.Filters
{
    public interface IChatFilter
    {
        string Name { get; set; }
        string Description { get; set; }
        bool Enabled { get; set; }
        bool ModExempt { get; set; }
        string[] PermittedUsers { get; set; }
        bool Match(string input);
    }
}
