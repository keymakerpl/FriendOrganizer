﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FriendOrganizer.Model;

namespace FriendOrganizer.UI.Data.Repositories
{
    public interface IFriendRepository
    {
        IEnumerable<Friend> GetAll();
        Task<List<Friend>> GetAllAsync();
        Task<Friend> GetByIdAsync(int friendId);
        Task SaveAsync();
        bool HasChanges();
        void Add(Friend friend);
        void Remove(Friend friendModel);
        void RemovePhoneNumber(FriendPhoneNumber selectedNumberModel);
    }
}
