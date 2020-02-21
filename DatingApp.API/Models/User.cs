using System;
using System.Collections.Generic;

namespace DatingApp.API.Models
{
    public class User
    {
        public int Id {set; get;}

        public string Username {set; get;}

        public byte[] PasswordHash { set; get;}

        public byte[] PasswordSalt {set; get;}
        public string Gender {get; set;}
        public DateTime DateOfBirth {get; set;}
        public string KnownAs {get; set;}
        public DateTime Created {get; set;}
        public DateTime LastActive {get; set;}
        public string Introduction {get; set;}
        public string LookingFor {get; set;}
        public string Interests {get; set;}
        public string City {get; set;}
        public string Country {get; set;}

        //in database, 'users' table doesn't have below fields below which relations exists in 'photos' and 'likes' table  
        //but with EntityFramework, they are inlcuding in 'User' model
        public ICollection<Photo> Photos {get; set;}
        public ICollection<Like> Likers {get; set;}
        public ICollection<Like> Likees {get; set;}

    }

}