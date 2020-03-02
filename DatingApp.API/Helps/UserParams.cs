namespace DatingApp.API.Helps
{
    public class UserParams
    {
        public const int MaxPageSize = 10;
        public int CurrentPage { get; set; } = 0;
        public int PageSize { get; set; } = 5;
        public int UserId {get; set;}
        public UserParams()
        {
            if (PageSize > MaxPageSize)
                PageSize = MaxPageSize;
        }
    }
}