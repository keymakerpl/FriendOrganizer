using System.Threading.Tasks;
using System.Windows.Input;
using FriendOrganizer.Event;
using FriendOrganizer.UI.Data.Repositories;
using FriendOrganizer.UI.Wrapper;
using Prism.Commands;
using Prism.Events;

namespace FriendOrganizer.UI.ViewModel
{
    public class FriendDetailViewModel : ViewModelBase, IFriendDetailViewModel
    {
        private IFriendRepository _repository;
        private FriendWrapper _friend;
        private IEventAggregator _eventAggregator;
        private bool _hasChanges;
        public ICommand SaveCommand { get; }

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
        public FriendDetailViewModel(IFriendRepository repository, IEventAggregator eventAggregator)
        {
            _repository = repository;
            _eventAggregator = eventAggregator;

            SaveCommand = new DelegateCommand(OnSaveExecute,
                OnSaveCanExecute);
        }

        public async Task LoadAsync(int friendId)
        {
            //Pobiera model z repo
            var friend = await _repository.GetByIdAsync(friendId);

            //Opakowanie modelu detala
            Friend = new FriendWrapper(friend);

            //Po załadowaniu detala i każdej zmianie propertisa sprawdzamy CanExecute Sejwa
            Friend.PropertyChanged += ((sender, args) =>
            {
                if (!HasChanges)
                {
                    HasChanges = _repository.HasChanges();
                }
                if (args.PropertyName == nameof(Friend.HasErrors))
                {
                    ((DelegateCommand) SaveCommand).RaiseCanExecuteChanged(); 
                }
            });
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Właściwośc pomocnicza do przechowania zmiany z repo
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
            HasChanges = _repository.HasChanges();

            //Powiadom agregator eventów, że zapisano
            _eventAggregator.GetEvent<AfterFriendSavedEvent>()
                .Publish(new AfterFriendSavedEventArgs
                {
                    Id = Friend.Id,
                    DisplayMember = $"{Friend.FirstName} {Friend.LastName}"
                });
        }             
    }
}
