using System.ComponentModel.DataAnnotations;

namespace DatingApp.API.Dtos
{
    public class UserForLoginDto
    {
        [Required]
        public string username {get; set;}

        [Required]
        [StringLength(8, MinimumLength=4, ErrorMessage="pls specify password >= 4 and <= 8")]
        public string password {get; set;}
    }
}