using System;
using System.Threading.Tasks;
using DatingApp.API.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class AuthRepository: IAuthRepository
    {
        private readonly DataContext _context;

        public AuthRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<User> Register(User user, string password)
        {
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> Login(string username, string password)
        {
                var user = await _context.Users.FirstOrDefaultAsync( x=> x.Username == username);
                
                if (user == null){
                    return null;
                }

                if (!VerifyPassword(user, password))
                    return null;
                
                return user;
        }

        private bool VerifyPassword(User user, string password)
        {
            using( var hamc = new System.Security.Cryptography.HMACSHA512(user.PasswordSalt))
            {
                    var computedHash = hamc.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                    for( int i =0; i< computedHash.Length; i++)
                    {
                        if (computedHash[i] != user.PasswordHash[i])
                            return false;
                    }
            }
            return true;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using( var hamc = new System.Security.Cryptography.HMACSHA512())
            {
                    passwordSalt = hamc.Key;
                    passwordHash = hamc.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public async Task<bool> UserExists(string username)
        {
            if (!await _context.Users.AnyAsync(x => x.Username == username))
                 return false;

            return true;
        }
    }
}