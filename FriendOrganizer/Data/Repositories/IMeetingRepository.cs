using System.Collections.Generic;
using System.Threading.Tasks;
using FriendOrganizer.Model;
using FriendOrganizer.UI.Data.Repositories;

namespace FriendOrganizer.Data.Repositories
{
    public interface IMeetingRepository : IGenericRepository<Meeting>
    {
        Task<Meeting> GetByIdAsync(int id);
        Task<List<Friend>> GetAllFriendsAsync();
        Task ReloadFriendsAsync(int friendId);
    }
}