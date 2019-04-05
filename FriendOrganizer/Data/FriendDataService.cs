using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FriendOrganizer.DataAcces;
using FriendOrganizer.Model;

namespace FriendOrganizer.UI.Data
{
    public class FriendDataService : IFriendDataService
    {
        private Func<FriendOrganizerDbContext> _contextCreator;

        /// <summary>
        /// Autofec zajmie się wstrzykiwaniem zależności
        /// </summary>
        /// <param name="func"></param>
        public FriendDataService(Func<FriendOrganizerDbContext> contextCreatorFunc)
        {
            _contextCreator = contextCreatorFunc;
        } 
        
        public IEnumerable<Friend> GetAll()
        {
            using (var ctx = _contextCreator())
            {
                return ctx.Friends.AsNoTracking().ToList();
            }

        }


        public async Task<Friend> GetByIdAsync(int friendId)
        {
            using (var ctx = _contextCreator())
            {
                return await ctx.Friends.AsNoTracking().SingleAsync(f => f.Id == friendId);
            }

        }

        public async Task SaveAsync(Friend friend)
        {
            using (var ctx = _contextCreator())
            {
                ctx.Friends.Attach(friend);
                ctx.Entry(friend).State = EntityState.Modified;
                await ctx.SaveChangesAsync();
            }
        }

        public async Task<List<Friend>> GetAllAsync()
        {
            using (var ctx = _contextCreator())
            {
                return await ctx.Friends.AsNoTracking().ToListAsync();
            }

        }
    }
}
