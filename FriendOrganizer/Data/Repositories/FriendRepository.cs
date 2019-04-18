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
    public class FriendRepository : GenericRepository<Friend, FriendOrganizerDbContext>, IFriendRepository
    {
        public FriendRepository(FriendOrganizerDbContext context) : base(context)
        {
            
        }

        public override async Task<Friend> GetByIdAsync(int id)
        {
            return await Context.Set<Friend>().Include(f => f.PhoneNumbers).SingleAsync(f => f.Id == id);
        }

        public IEnumerable<Friend> GetAll()
        {
            throw new NotImplementedException();
        }

        public async Task<List<Friend>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public void RemovePhoneNumber(FriendPhoneNumber selectedNumberModel)
        {
            Context.FriendPhoneNumbers.Remove(selectedNumberModel);
        }

        public async Task<bool> HasMeetingsAsync(int friendId)
        {
            return await Context.Meetings.AsNoTracking().Include(m => m.Friends)
                .AnyAsync(m => m.Friends.Any(f => f.Id == friendId));
        }
    }
}
