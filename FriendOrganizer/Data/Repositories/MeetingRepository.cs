using System.Threading.Tasks;
using System.Data.Entity;
using FriendOrganizer.DataAcces;
using FriendOrganizer.Model;
using FriendOrganizer.UI.Data.Repositories;

namespace FriendOrganizer.Data.Repositories
{
    public class MeetingRepository : GenericRepository<Meeting, FriendOrganizerDbContext>, IMeetingRepository
    {
        public MeetingRepository(FriendOrganizerDbContext context) : base(context)
        {
            
        }

        public async override Task<Meeting> GetByIdAsync(int id)
        {
            return await Context.Meetings.Include(m => m.Friends).SingleAsync(m => m.Id == id);
        }
    }
}
