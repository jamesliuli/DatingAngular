namespace DatingApp.API.Helps
{
    public class UserParams
    {
        public const int MaxPageSize = 10;
        public int CurrentPage { get; set; } = 0;
        private int pageSize = 5;
        public int PageSize { 
            get {return pageSize;}
            set { pageSize = value > MaxPageSize? MaxPageSize : value;}
        }
        public int UserId {get; set;}
    }
}