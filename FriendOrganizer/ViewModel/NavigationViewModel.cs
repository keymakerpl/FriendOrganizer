using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using FriendOrganizer.Event;
using FriendOrganizer.UI.Data.Lookups;
using FriendOrganizer.UI.Event;
using FriendOrganizer.ViewModel;
using Prism.Events;

namespace FriendOrganizer.UI.ViewModel
{
    public class NavigationViewModel : ViewModelBase, INavigationViewModel
    {
        private IFriendLookupDataService _friendLookupDataService;
        private IEventAggregator _eventAggregator;
        public ObservableCollection<NavigationItemViewModel> Friends { get; }

        public NavigationViewModel(IFriendLookupDataService friendLookupDataService, IEventAggregator eventAggregator)
        {
            _friendLookupDataService = friendLookupDataService;
            _eventAggregator = eventAggregator;
            Friends = new ObservableCollection<NavigationItemViewModel>();
            _eventAggregator.GetEvent<AfterFriendSavedEvent>().Subscribe(AfterFriendSaved);
            _eventAggregator.GetEvent<AfterFriendDeletedEvent>().Subscribe(AfterFriendDeleted);
        }        

        public async Task LoadAsync()
        {
            var lookup = await _friendLookupDataService.GetFriendLookupAsync();
            Friends.Clear();
            foreach (var lookupItem in lookup)
            {
                Friends.Add(new NavigationItemViewModel(lookupItem.Id, lookupItem.DisplayMember, _eventAggregator));
            }
        }

        private void AfterFriendDeleted(int friendId)
        {
            var friend = Friends.SingleOrDefault(f => f.Id == friendId);
            if (friend != null) Friends.Remove(friend);
        }

        private void AfterFriendSaved(AfterFriendSavedEventArgs args)
        {
            var lookupItem = Friends.SingleOrDefault(i => i.Id == args.Id); //Wywali null jeśli na liście nie ma nowego detala
            if (lookupItem == null)
            {
                Friends.Add(new NavigationItemViewModel(args.Id, args.DisplayMember, _eventAggregator)); //dodajemy nowy detal do listy
            }
            else lookupItem.DisplayMember = args.DisplayMember;

        }

    }
}
