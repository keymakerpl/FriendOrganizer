using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FriendOrganizer.DataAcces;
using FriendOrganizer.Model;

namespace FriendOrganizer.UI.Data.Repositories
{
    public class FriendRepository : IFriendRepository
    {
        private FriendOrganizerDbContext _context;

        /// <summary>
        /// Autofec zajmie się wstrzykiwaniem zależności
        /// </summary>
        /// <param name="context"></param>
        public FriendRepository(FriendOrganizerDbContext context)
        {
            _context = context;
        } 
        
        public IEnumerable<Friend> GetAll()
        {
                return _context.Friends.AsNoTracking().ToList();           
        }

        public async Task<Friend> GetByIdAsync(int friendId)
        {
            return await _context.Friends.SingleAsync(f => f.Id == friendId);
        }

        public async Task SaveAsync()
        {            
                await _context.SaveChangesAsync();            
        }

        public bool HasChanges()
        {
            return _context.ChangeTracker.HasChanges();
        }

        public async Task<List<Friend>> GetAllAsync()
        {
                return await _context.Friends.AsNoTracking().ToListAsync();           
        }

        public void Add(Friend friend)
        {
            _context.Friends.Add(friend);
        }

        public void Remove(Friend friendModel)
        {
            _context.Friends.Remove(friendModel);
        }
    }
}
