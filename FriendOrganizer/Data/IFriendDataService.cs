using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FriendOrganizer.Model;

namespace FriendOrganizer.UI.Data
{
    public interface IFriendDataService
    {
        IEnumerable<Friend> GetAll();
        Task<List<Friend>> GetAllAsync();
        Task<Friend> GetByIdAsync(int friendId);
        Task SaveAsync(Friend friend);
    }
}
