using System;
using System.Threading.Tasks;
using FriendOrganizer.UI.Event;
using Prism.Events;
using FriendOrganizer.View.UI.Services;

namespace FriendOrganizer.UI.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private IEventAggregator _eventAggregator;
        private Func<IFriendDetailViewModel> _friendDetailViewModelCreator { get; }
        public INavigationViewModel NavigationViewModel { get; }
        private IFriendDetailViewModel _friendDetailViewModel;
        private IMessageDialogService _messageDialogService;

        public IFriendDetailViewModel FriendDetailViewModel
        {
            get => _friendDetailViewModel;
            private set { _friendDetailViewModel = value; OnPropertyChanged();}
        }

        /// <summary>
        /// Główne okno programu, przyjmuje jako parametr Nawigator
        /// </summary>
        /// <param name="navigationViewModel"></param>
        /// <param name="friendDetailViewModelCreator"></param>
        /// <param name="eventAggregator"></param>
        public MainViewModel(INavigationViewModel navigationViewModel, Func<IFriendDetailViewModel> friendDetailViewModelCreator,
            IEventAggregator eventAggregator,
            IMessageDialogService messageService)
        {
            _eventAggregator = eventAggregator;
            _friendDetailViewModelCreator = friendDetailViewModelCreator;
            _messageDialogService = messageService;

            _eventAggregator.GetEvent<OpenFriendDetailViewEvent>()
                .Subscribe(OnOpenFriendDetailView);

            NavigationViewModel = navigationViewModel;
        }

        public void Load()
        {
            throw new NotImplementedException();
        }

        public async Task LoadAsync()
        {
            await NavigationViewModel.LoadAsync();
        }

        private async void OnOpenFriendDetailView(int friendId)
        {
            //TODO: remove from view model and move as incjected from class
            if (FriendDetailViewModel != null && FriendDetailViewModel.HasChanges)
            {
                var result = _messageDialogService.ShowOkCancelDialog("You made changes. Continue?", "Has been changed");
                if (result == MessageDialogRessult.Cancel) return;
            }

            FriendDetailViewModel = _friendDetailViewModelCreator();
            await FriendDetailViewModel.LoadAsync(friendId);
        }
    }
}
