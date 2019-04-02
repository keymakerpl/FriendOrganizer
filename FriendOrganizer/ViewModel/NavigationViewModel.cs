using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FriendOrganizer.UI.Data;
using FriendOrganizer.Model;

namespace FriendOrganizer.UI.ViewModel
{
    public class NavigationViewModel : INavigationViewModel
    {
        private IFriendLookupDataService _friendLookupDataService;
        public ObservableCollection<LookupItem> Friends { get; }

        public NavigationViewModel(IFriendLookupDataService friendLookupDataService)
        {
            _friendLookupDataService = friendLookupDataService;
            Friends = new ObservableCollection<LookupItem>();
        }

        public async Task LoadAsync()
        {
            var lookup = await _friendLookupDataService.GetFriendLookupAsync();
            Friends.Clear();
            foreach (var lookupItem in lookup)
            {
                Friends.Add(lookupItem);
            }
        }
    }
}
