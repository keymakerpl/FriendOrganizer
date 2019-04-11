using System.Threading.Tasks;
using System.Windows.Input;
using FriendOrganizer.Event;
using FriendOrganizer.Model;
using FriendOrganizer.UI.Data.Repositories;
using FriendOrganizer.UI.Wrapper;
using FriendOrganizer.View.UI.Services;
using Prism.Commands;
using Prism.Events;

namespace FriendOrganizer.UI.ViewModel
{
    public class FriendDetailViewModel : ViewModelBase, IFriendDetailViewModel
    {
        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get; }
        private IFriendRepository _repository;
        private FriendWrapper _friend;
        private IEventAggregator _eventAggregator;
        private bool _hasChanges;
        private IMessageDialogService _dialogService;

        public FriendWrapper Friend
        {
            get { return _friend; }
            set
            {
                _friend = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="repository">Repo</param>
        /// <param name="eventAggregator">Agregator eventów</param>
        public FriendDetailViewModel(IFriendRepository repository, IEventAggregator eventAggregator, IMessageDialogService dialogService)
        {
            _repository = repository;
            _eventAggregator = eventAggregator;
            _dialogService = dialogService;

            //TODO: move to init
            SaveCommand = new DelegateCommand(OnSaveExecute,
                OnSaveCanExecute);

            DeleteCommand = new DelegateCommand(OnDeleteExecute);
        }

        private async void OnDeleteExecute()
        {
            var result = _dialogService.ShowOkCancelDialog("Delete?", "Confirm");
            if (result == MessageDialogRessult.Cancel) return;
            _repository.Remove(Friend.Model);
            await _repository.SaveAsync();
            _eventAggregator.GetEvent<AfterFriendDeletedEvent>().Publish(Friend.Id);
        }

        public async Task LoadAsync(int? friendId)
        {
            //Pobiera model z repo
            var friend = friendId.HasValue ?
                await _repository.GetByIdAsync(friendId.Value) : CreateNewFriend();

            //Opakowanie modelu detala w ModelWrapper aby korzystał z walidacji propertisów
            Friend = new FriendWrapper(friend);

            //Po załadowaniu detala i każdej zmianie propertisa sprawdzamy CanExecute Sejwa
            Friend.PropertyChanged += ((sender, args) =>
            {
                if (!HasChanges)
                {
                    HasChanges = _repository.HasChanges();
                }

                //sprawdzamy czy zmieniony propert w modelu ma błędy i ustawiamy SaveButton
                if (args.PropertyName == nameof(Friend.HasErrors))
                {
                    ((DelegateCommand) SaveCommand).RaiseCanExecuteChanged(); 
                }
            });
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();

            if (!friendId.HasValue) Friend.FirstName = ""; // takie se, trzeba tacznąć propertisa aby zadziałała walidacja nowego detalu
        }

        /// <summary>
        /// Właściwośc pomocnicza do przechowania zmiany z repo, odpala even jeśli w repo zaszły  zmiany
        /// </summary>
        public bool HasChanges
        {
            get =>
                _hasChanges;
            set
            {
                if (_hasChanges != value)
                {
                    _hasChanges = value;
                    OnPropertyChanged();
                    ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
                }
            }
        }

        /// <summary>
        /// Sprawdzamy czy model ma błędy walidacji i czy repozytorium zostało zmienione
        /// </summary>
        /// <returns></returns>
        private bool OnSaveCanExecute()
        {            
            return Friend != null && !Friend.HasErrors && HasChanges;
        }

        private async void OnSaveExecute()
        {
            await _repository.SaveAsync();
            HasChanges = _repository.HasChanges(); // Po zapisie ustawiamy flagę na false jeśli nie ma zmian w repo

            //Powiadom agregator eventów, że zapisano
            _eventAggregator.GetEvent<AfterFriendSavedEvent>()
                .Publish(new AfterFriendSavedEventArgs
                {
                    Id = Friend.Id,
                    DisplayMember = $"{Friend.FirstName} {Friend.LastName}"
                });
        }

        private Friend CreateNewFriend()
        {
            var friend = new Friend();
            _repository.Add(friend);

            return friend;
        }
    }
}
