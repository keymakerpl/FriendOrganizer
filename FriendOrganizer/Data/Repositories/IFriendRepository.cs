using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FriendOrganizer.Model;

namespace FriendOrganizer.UI.Data.Repositories
{
    public interface IFriendRepository : IGenericRepository<Friend>
    {
        IEnumerable<Friend> GetAll();
        Task<List<Friend>> GetAllAsync();
        void RemovePhoneNumber(FriendPhoneNumber selectedNumberModel);
    }
}
