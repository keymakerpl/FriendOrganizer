using System;
using System.Threading.Tasks;
using System.Windows.Input;
using FriendOrganizer.Event;
using FriendOrganizer.UI.Event;
using Prism.Events;
using FriendOrganizer.View.UI.Services;
using Prism.Commands;

namespace FriendOrganizer.UI.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public INavigationViewModel NavigationViewModel { get; }
        private Func<IFriendDetailViewModel> _friendDetailViewModelCreator { get; }
        private IFriendDetailViewModel _friendDetailViewModel;
        private IEventAggregator _eventAggregator;
        private IMessageDialogService _messageDialogService;
        public IFriendDetailViewModel FriendDetailViewModel
        {
            get => _friendDetailViewModel;
            private set { _friendDetailViewModel = value; OnPropertyChanged();}
        }
        public ICommand CreateNewFriendCommand { get; }


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
            _eventAggregator.GetEvent<AfterFriendDeletedEvent>().Subscribe(AfterFriendDeleted);

            CreateNewFriendCommand = new DelegateCommand(OnCreateNewFriendExecute);

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

        private async void OnOpenFriendDetailView(int? friendId)
        {
            if (FriendDetailViewModel != null && FriendDetailViewModel.HasChanges)
            {
                var result = _messageDialogService.ShowOkCancelDialog("You made changes. Continue?", "Has been changed");
                if (result == MessageDialogRessult.Cancel) return;
            }

            FriendDetailViewModel = _friendDetailViewModelCreator();
            await FriendDetailViewModel.LoadAsync(friendId);
        }

        private void AfterFriendDeleted(int friendId)
        {
            FriendDetailViewModel = null;
        }

        private void OnCreateNewFriendExecute()
        {
            //Korzystamy z tej samej metody lecz bez id w parametrze
            OnOpenFriendDetailView(null);
        }
    }
}
