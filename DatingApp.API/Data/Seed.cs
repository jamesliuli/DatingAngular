using System.Collections.Generic;
using System.Linq;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace DatingApp.API.Data
{
    public class Seed
    {
        public static void SeedUsers(UserManager<User> userManager, RoleManager<Role> roleManager){
            if (!userManager.Users.Any())
            {
               var userData = System.IO.File.ReadAllText("Data/UserSeedData.json");
               var users = JsonConvert.DeserializeObject<List<User>>(userData);
            //    foreach( var user in users)
            //    {
            //        userManager.CreateAsync(user, "password").Wait();
            //    }

               //create roles

               var roles = new List<Role>()
               {
                   new Role(){ Name = "Member" },
                   new Role(){ Name = "Moderator" },
                   new Role(){ Name = "Admin" },
                   new Role(){ Name = "VIP" }
               };

               foreach(var role in roles)
               {
                   roleManager.CreateAsync(role).Wait();
               }

               //assign role to users
               foreach(var user in users)
               {
                   userManager.CreateAsync(user, "password").Wait();
                   userManager.AddToRoleAsync(user, "Member");
               }
               //add admin user
               var adminUser = new User(){
                   UserName = "Admin"
               };

               var result = userManager.CreateAsync(adminUser, "password").Result;
               if (result.Succeeded)
               {
                   var admin = userManager.FindByNameAsync("Admin").Result;
                   userManager.AddToRolesAsync(admin, new string[]{"Admin","Moderator" });
               }
            }
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using( var hamc = new System.Security.Cryptography.HMACSHA512())
            {
                    passwordSalt = hamc.Key;
                    passwordHash = hamc.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
    }
}