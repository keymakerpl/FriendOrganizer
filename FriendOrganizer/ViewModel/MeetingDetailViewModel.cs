using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using FriendOrganizer.Data.Repositories;
using FriendOrganizer.Event;
using FriendOrganizer.Model;
using FriendOrganizer.View.UI.Services;
using FriendOrganizer.UI.Wrapper;
using Prism.Commands;
using Prism.Events;

namespace FriendOrganizer.UI.ViewModel
{
    public class MeetingDetailViewModel : DetailViewModelBase, IMeetingDetailViewModel
    {
        private IMeetingRepository _repository;
        private MeetingWrapper _meeting;
        private Friend _selectedAvailableFriend;
        private Friend _selectedAddedFriend;
        private List<Friend> _allFriends;

        public MeetingDetailViewModel(IEventAggregator eventAggregator,
            IMessageDialogService dialogService,
            IMeetingRepository meetingRepository) : base(eventAggregator, dialogService)
        {
            _repository = meetingRepository;

            eventAggregator.GetEvent<AfterDetailSavedEvent>().Subscribe(AfterDetailSaved);

            AddedFriends = new ObservableCollection<Friend>();
            AvailableFriends = new ObservableCollection<Friend>();
            
            AddFriendCommand = new DelegateCommand(OnAddFriendExecute, OnAddFriendCanExecute);
            RemoveFriendCommand = new DelegateCommand(OnRemoveFriendExecute, OnRemoveFriendCanExecute);
        }

        private async void AfterDetailSaved(AfterDetailSavedEventArgs args)
        {
            if (args.ViewModelName == nameof(FriendDetailViewModel))
            {
                await _repository.ReloadFriendsAsync(args.Id);
                _allFriends = await _repository.GetAllFriendsAsync();
                SetUpFriendsPickList();
            }
        }

        private bool OnAddFriendCanExecute()
        {
            return SelectedAvailableFriend != null;
        }

        private bool OnRemoveFriendCanExecute()
        {
            return SelectedAddedFriend != null;
        }

        private void OnRemoveFriendExecute()
        {
            var friendToRemove = SelectedAddedFriend;

            Meeting.Model.Friends.Remove(friendToRemove);

            AddedFriends.Remove(friendToRemove);
            AvailableFriends.Add(friendToRemove);

            HasChanges = _repository.HasChanges();
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        }

        private void OnAddFriendExecute()
        {
            var friendToAdd = SelectedAvailableFriend;

            Meeting.Model.Friends.Add(friendToAdd);

            AddedFriends.Add(friendToAdd);
            AvailableFriends.Remove(friendToAdd);

            HasChanges = _repository.HasChanges();
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        }

        public ICommand RemoveFriendCommand { get; set; }

        public ICommand AddFriendCommand { get; set; }

        public Friend SelectedAvailableFriend
        {
            get { return _selectedAvailableFriend;}
            set
            {
                _selectedAvailableFriend = value;
                OnPropertyChanged();
                ((DelegateCommand)AddFriendCommand).RaiseCanExecuteChanged();
            }
        }

        public Friend SelectedAddedFriend
        {
            get { return _selectedAddedFriend; }
            set
            {
                _selectedAddedFriend = value; 
                OnPropertyChanged();
                ((DelegateCommand)RemoveFriendCommand).RaiseCanExecuteChanged();
            }
        }

        public ObservableCollection<Friend> AvailableFriends { get; set; }

        public ObservableCollection<Friend> AddedFriends { get; set; }

        public MeetingWrapper Meeting
        {
            get { return _meeting; }
            private set
            {
                _meeting = value; 
                OnPropertyChanged();
            }
        }

        public override async Task LoadAsync(int meetingId)
        {
            //jeśli nie ma twórz nowy
            var meeting = meetingId > 0 ? await _repository.GetByIdAsync(meetingId) : CreateNewMeeting();
            Id = meetingId; //ustaw dla modelu detala

            InitMeeting(meeting);
            _allFriends = await _repository.GetAllFriendsAsync();

            SetUpFriendsPickList(); //ustaw listę friendsów do dodania
        }

        private void SetUpFriendsPickList()
        {
            var meetingFriendsIds = Meeting.Model.Friends.Select(f => f.Id).ToList(); //sa już w meetingu
            var addedFriends = _allFriends.Where(f => meetingFriendsIds.Contains(f.Id)).OrderBy(f => f.FirstName); //
            var availableFriends = _allFriends.Except(addedFriends).OrderBy(f => f.FirstName);

            AddedFriends.Clear();
            AvailableFriends.Clear();

            foreach (var addedFriend in addedFriends)
            {
                AddedFriends.Add(addedFriend);
            }
            foreach (var avaiableFriend in availableFriends)
            {
                AvailableFriends.Add(avaiableFriend);
            }
        }

        private void InitMeeting(Meeting meeting)
        {
            Meeting = new MeetingWrapper(meeting);
            Meeting.PropertyChanged += (sender, args) =>
            {
                if (!HasChanges)
                {
                    HasChanges = _repository.HasChanges();
                }

                if (args.PropertyName == nameof(Meeting.HasErrors))
                {
                    ((DelegateCommand) SaveCommand).RaiseCanExecuteChanged();
                }

                ((DelegateCommand) SaveCommand).RaiseCanExecuteChanged();
                if (args.PropertyName == nameof(Meeting.Name))
                {
                    SetTitle();
                }
            };

            if (Meeting.Id == 0) Meeting.Name = "";

            SetTitle();
        }

        private void SetTitle()
        {
            Title = $"{Meeting.Name}";
        }

        private Meeting CreateNewMeeting()
        {
            var newMeeting = new Meeting();
            _repository.Add(newMeeting);

            return newMeeting;
        }

        protected override async void OnSaveExecute()
        {
            await _repository.SaveAsync();
            HasChanges = _repository.HasChanges(); // Po zapisie ustawiamy flagę na false bo nie ma już zmian w repo

            Meeting.Id = Id; //odśwież meetingId

            //Powiadom agregator eventów, że zapisano
            RaiseDetailSavedEvent(Meeting.Id, Meeting.Name);
        }

        protected override bool OnSaveCanExecute()
        {
            return Meeting != null && !Meeting.HasErrors && HasChanges;
        }

        protected async override void OnDeleteExecute()
        {
            var result = await MessageDialogService.ShowOkCancelDialog("Delete?", "Confirm");
            if (result == MessageDialogResult.Cancel) return;

            _repository.Remove(Meeting.Model);
            await _repository.SaveAsync();
            RaiseDetailDeletedEvent(Meeting.Id);
        }
    }
}
