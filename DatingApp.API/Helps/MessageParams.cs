namespace DatingApp.API.Helps
{
    public class MessageParams
    {
        public int UserId { get; set; }
        public string MessageContainer { get; set; } = "unread";
    }
}