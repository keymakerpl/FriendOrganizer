using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FriendOrganizer.Model;
using FriendOrganizer.UI.Data;
using FriendOrganizer.UI.ViewModel;

namespace FriendOrganizer.UI.ViewModel
{
    public class FriendDetailViewModel : ViewModelBase, IFriendDetailViewModel
    {
        private IFriendDataService _dataService;
        private Friend _friend;
        public Friend Friend
        {
            get { return _friend; }
            set { _friend = value; OnPropertyChanged(); }
        }

        public FriendDetailViewModel(IFriendDataService dataService)
        {
            _dataService = dataService;
        }

        public async Task LoadAsync(int friendId)
        {
            Friend = await _dataService.GetByIdAsync(friendId);
        }
    }
}
