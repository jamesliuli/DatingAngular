using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Helps;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class DatingRepository : IDatingRepository
    {
        public DataContext _context { get; set; }
        public DatingRepository(DataContext context)
        {
            _context = context;
        }
        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public async Task<User> GetUser(int id)
        {
            var user = await _context.Users.Include(p=> p.Photos).FirstOrDefaultAsync( u => u.Id == id);
            return user;
        }
        public async Task<IEnumerable<User>> GetUsers(UserParams userParams = null)
        {
            if (userParams == null)
               userParams = new UserParams();
            var loginUser = await this.GetUser(userParams.UserId);
            var users = _context.Users.Include( p => p.Photos).AsQueryable();
            users = users.Where( u => u.Gender != loginUser.Gender);
            return await users.Skip(userParams.CurrentPage> 0? (userParams.CurrentPage - 1) * userParams.PageSize : 0)
                              .Take(userParams.PageSize).ToListAsync();
        }
        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Photo> GetPhoto(int id)
        {
            return await _context.Photos.FirstOrDefaultAsync( p => p.Id == id);
        }

        public async Task<Like> GetLike(int userId, int recipientId)
        {
            return await _context.Likes.FirstOrDefaultAsync(u =>
                u.LikerId == userId && u.LikeeId == recipientId);
        }
        public async Task<IEnumerable<int>> GetUserLikes(int id, bool likers)
        {
            var user = await _context.Users.Include( u => u.Likers)
                      .Include(u => u.Likees).FirstOrDefaultAsync(u => u.Id == id);
            if (likers)
            {
                return user.Likers.Where(u => u.LikeeId == id).Select(i => i.LikerId);
            }
            else
            {
                return user.Likees.Where(u => u.LikerId == id).Select(i => i.LikeeId);
            }
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages.FirstOrDefaultAsync( m => m.Id == id);
        }

        public async Task<IEnumerable<Message>> GetMessagesForUser(MessageParams messageParams)
        {
            var messages = _context.Messages.Include( u => u.Sender).ThenInclude( p => p.Photos)
            .Include( u => u.Recipient).ThenInclude( p => p.Photos)
            .AsQueryable();

            switch (messageParams.MessageContainer)
             {
                 case "Inbox":
                      messages = messages.Where( u => u.RecipientId == messageParams.UserId);
                      break;
                 case "Outbox":
                      messages = messages.Where( u => u.SenderId == messageParams.UserId);
                      break;
                 default:
                      messages = messages.Where( u => u.RecipientId == messageParams.UserId &&
                                                      u.IsRead == false);
                      break;
             }
             messages = messages.OrderByDescending( m => m.MessageSent);
             return await messages.ToListAsync();
        }

        public async Task<IEnumerable<Message>> GetMessageThread(int userId, int recipientId)
        {
            var messages = await _context.Messages.Include( u => u.Sender).ThenInclude( p => p.Photos)
                                            .Include( u => u.Recipient).ThenInclude( p => p.Photos)
                                            .Where( u => u.RecipientId == userId && u.SenderId == recipientId ||
                                                    u.SenderId == userId && u.RecipientId == recipientId)
                                            .OrderByDescending( m => m.MessageSent)
                                            .ToListAsync();
            return messages;                                                
        }
    }
}