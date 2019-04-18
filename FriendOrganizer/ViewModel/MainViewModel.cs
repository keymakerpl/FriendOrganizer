using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Autofac.Features.Indexed;
using FriendOrganizer.Event;
using FriendOrganizer.UI.Event;
using Prism.Events;
using FriendOrganizer.View.UI.Services;
using Prism.Commands;

namespace FriendOrganizer.UI.ViewModel
{
    public class MainViewModel : ViewModelBase
    {       
        private IEventAggregator _eventAggregator;
        private IMessageDialogService _messageDialogService;
        private IIndex<string, IDetailViewModel> _detailViewModelCreator;

        public INavigationViewModel NavigationViewModel { get; }
        public ICommand CreateNewDetailCommand { get; }

        private IDetailViewModel _detailViewModel;
        public IDetailViewModel DetailViewModel
        {
            get => _detailViewModel;
            private set { _detailViewModel = value; OnPropertyChanged();}
        }

        /// <summary>
        /// Główne okno programu, przyjmuje jako parametr Nawigator
        /// </summary>
        /// <param name="navigationViewModel"></param>
        /// <param name="friendDetailViewModelCreator"></param>
        /// <param name="eventAggregator"></param>
        public MainViewModel(INavigationViewModel navigationViewModel,
            IIndex<string, IDetailViewModel> detailViewModelCreator,
            IEventAggregator eventAggregator,
            IMessageDialogService messageService)
        {
            NavigationViewModel = navigationViewModel;
            _detailViewModelCreator = detailViewModelCreator;
            _eventAggregator = eventAggregator;            
            _messageDialogService = messageService;            

            _eventAggregator.GetEvent<OpenDetailViewEvent>()
                .Subscribe(OnOpenDetailView);

            _eventAggregator.GetEvent<AfterDetailDeletedEvent>()
                .Subscribe(AfterDetailDeleted);

            CreateNewDetailCommand = new DelegateCommand<Type>(OnCreateNewDetailExecute);
        }        

        public void Load()
        {
            throw new NotImplementedException();
        }

        public async Task LoadAsync()
        {
            await NavigationViewModel.LoadAsync();
        }

        private void OnCreateNewDetailExecute(Type viewModelType)
        {
            //Korzystamy z tej samej metody lecz bez id w parametrze
            OnOpenDetailView(new OpenDetailViewEventArgs() { ViewModelName = viewModelType.Name });
        }

        private async void OnOpenDetailView(OpenDetailViewEventArgs args)
        {
            if (DetailViewModel != null && DetailViewModel.HasChanges)
            {
                var result = _messageDialogService.ShowOkCancelDialog("You made changes. Continue?", "Has been changed");
                if (result == MessageDialogRessult.Cancel) return;
            }

            //ustawiamy odpowieni model
            //switch (args.ViewModelName)
            //{
            //    case nameof(FriendDetailViewModel):
            //        DetailViewModel = _friendDetailViewModelCreator();
            //        break;

            //    case nameof(MeetingDetailViewModel):
            //        DetailViewModel = _meetingDetailViewModelCreator();
            //        break;

            //    default:
            //        throw new Exception($"ViewModel {args.ViewModelName} not exists");
            //}

            //Pobieramy teraz odpowiedni model po kluczu. Autofac to ogarnie.
            DetailViewModel = _detailViewModelCreator[args.ViewModelName];

            await DetailViewModel.LoadAsync(args.Id);
        }

        private void AfterDetailDeleted(AfterDetailDeletedEventArgs args)
        {
            DetailViewModel = null;
        }

        
    }
}
