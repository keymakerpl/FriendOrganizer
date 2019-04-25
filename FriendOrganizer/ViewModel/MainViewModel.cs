using System;
using System.Collections.ObjectModel;
using System.Linq;
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

        public ObservableCollection<IDetailViewModel> DetailViewModels { get; }
        public INavigationViewModel NavigationViewModel { get; }
        public ICommand CreateNewDetailCommand { get; }
        public ICommand OpenSingleDetailViewCommand { get; }

        private IDetailViewModel _selectedDetailViewModel;
        public IDetailViewModel SelectedDetailViewModel
        {
            get => _selectedDetailViewModel;
            set
            {
                _selectedDetailViewModel = value;
                OnPropertyChanged();
            }
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
            DetailViewModels = new ObservableCollection<IDetailViewModel>(); //for tabbed ui
            NavigationViewModel = navigationViewModel;

            _detailViewModelCreator = detailViewModelCreator;
            _eventAggregator = eventAggregator;            
            _messageDialogService = messageService;            

            _eventAggregator.GetEvent<OpenDetailViewEvent>()
                .Subscribe(OnOpenDetailView);

            _eventAggregator.GetEvent<AfterDetailDeletedEvent>()
                .Subscribe(AfterDetailDeleted);

            _eventAggregator.GetEvent<AfterDetailClosedEvent>()
                .Subscribe(AfterDetailClosed);

            CreateNewDetailCommand = new DelegateCommand<Type>(OnCreateNewDetailExecute);
            OpenSingleDetailViewCommand = new DelegateCommand<Type>(OpenSingleDetailViewExecute);
        }

        private void OpenSingleDetailViewExecute(Type viewModelType)
        {
            OnOpenDetailView(new OpenDetailViewEventArgs() { Id = -1, ViewModelName = viewModelType.Name });
        }

        private void AfterDetailClosed(AfterDetailClosedEventArgs args)
        {
            RemoveDetailViewModel(args.Id, args.ViewModelName);
        }

        private void RemoveDetailViewModel(int id, string viewModelName)
        {
            var detailViewModel = DetailViewModels.SingleOrDefault(vm => vm.Id == id
                                                                         && vm.GetType().Name == viewModelName);

            if (detailViewModel != null)
            {
                DetailViewModels.Remove(detailViewModel);
            }
        }

        public void Load()
        {
            throw new NotImplementedException();
        }

        public async Task LoadAsync()
        {
            await NavigationViewModel.LoadAsync();
        }

        private int newNextId = 0;
        private void OnCreateNewDetailExecute(Type viewModelType)
        {
            //Korzystamy z tej samej metody lecz bez id w parametrze
            OnOpenDetailView(new OpenDetailViewEventArgs() { Id = newNextId, ViewModelName = viewModelType.Name });
        }

        private async void OnOpenDetailView(OpenDetailViewEventArgs args)
        {
            var detailViewModel = DetailViewModels.SingleOrDefault(vm => vm.Id == args.Id
                                                                         && vm.GetType().Name == args.ViewModelName);

            if (detailViewModel == null)
            {
                //Pobieramy teraz odpowiedni model po kluczu. Autofac to ogarnie.
                detailViewModel = _detailViewModelCreator[args.ViewModelName];
                try
                {
                    await detailViewModel.LoadAsync(args.Id);
                }
                catch
                {
                    _messageDialogService.ShowInfoDialog("Has been deleted");
                    await NavigationViewModel.LoadAsync();
                    return;
                }
                DetailViewModels.Add(detailViewModel);
            }

            SelectedDetailViewModel = detailViewModel;
        }

        private void AfterDetailDeleted(AfterDetailDeletedEventArgs args)
        {
            RemoveDetailViewModel(args.Id, args.ViewModelName);
        }

        
    }
}
