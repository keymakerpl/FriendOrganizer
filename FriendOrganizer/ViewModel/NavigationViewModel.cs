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
        private IMeetingLookupDataService _meetingLookupService;

        public NavigationViewModel(IFriendLookupDataService friendLookupDataService,
            IEventAggregator eventAggregator,
            IMeetingLookupDataService meetingLookupDataService)
        {
            Friends = new ObservableCollection<NavigationItemViewModel>();
            Meetings  = new ObservableCollection<NavigationItemViewModel>();

            _friendLookupDataService = friendLookupDataService;
            _eventAggregator = eventAggregator;
            _meetingLookupService = meetingLookupDataService;

            _eventAggregator.GetEvent<AfterDetailSavedEvent>()
                .Subscribe(AfterDetailSaved);

            _eventAggregator.GetEvent<AfterDetailDeletedEvent>()
                .Subscribe(AfterDetailDeleted);
        }

        public ObservableCollection<NavigationItemViewModel> Friends { get; }

        public ObservableCollection<NavigationItemViewModel> Meetings { get; }

        public async Task LoadAsync()
        {
            var lookup = await _friendLookupDataService.GetFriendLookupAsync();
            Friends.Clear();
            foreach (var lookupItem in lookup)
            {
                Friends.Add(new NavigationItemViewModel(lookupItem.Id, lookupItem.DisplayMember, nameof(FriendDetailViewModel) ,_eventAggregator));
            }

            lookup = await _meetingLookupService.GetMeetingLookupAsync();
            Meetings.Clear();
            foreach (var lookupItem in lookup)
            {
                Meetings.Add(new NavigationItemViewModel(lookupItem.Id, lookupItem.DisplayMember, nameof(MeetingDetailViewModel), _eventAggregator));
            }
        }

        private void AfterDetailDeleted(AfterDetailDeletedEventArgs args)
        {
            switch (args.ViewModelName)
            {
                case nameof(FriendDetailViewModel):
                    AfterDetailDeleted(Friends, args);
                    break;

                case nameof(MeetingDetailViewModel):
                    AfterDetailDeleted(Meetings, args);
                    break;
            }
        }

        private void AfterDetailDeleted(ObservableCollection<NavigationItemViewModel> collection, AfterDetailDeletedEventArgs args)
        {
            var item = collection.SingleOrDefault(i => i.Id == args.Id);
            if (item != null) collection.Remove(item);
        }

        private void AfterDetailSaved(AfterDetailSavedEventArgs args)
        {
            switch (args.ViewModelName)
            {
                case nameof(FriendDetailViewModel):
                    AfterDetailSaved(Friends ,args);
                    break;
                case nameof(MeetingDetailViewModel):
                    AfterDetailSaved(Meetings, args);
                    break;
            }
        }

        private void AfterDetailSaved(ObservableCollection<NavigationItemViewModel> collection, AfterDetailSavedEventArgs args)
        {
            var lookupItem = collection.SingleOrDefault(i => i.Id == args.Id); //Wywali null jeśli na liście nie ma nowego detala
            if (lookupItem == null)
            {
                collection.Add(new NavigationItemViewModel(args.Id, args.DisplayMember, nameof(args.ViewModelName),
                    _eventAggregator)); //dodajemy nowy detal do listy
            }
            else lookupItem.DisplayMember = args.DisplayMember;
        }
    }
}
