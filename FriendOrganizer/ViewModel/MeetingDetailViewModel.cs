using System.Threading.Tasks;
using FriendOrganizer.Data.Repositories;
using FriendOrganizer.Model;
using FriendOrganizer.View.UI.Services;
using FriendOrganizer.Wrapper;
using Prism.Commands;
using Prism.Events;

namespace FriendOrganizer.UI.ViewModel
{
    public class MeetingDetailViewModel : DetailViewModelBase, IMeetingDetailViewModel
    {
        private IMessageDialogService _dialogService;
        private IMeetingRepository _repository;
        private MeetingWrapper Meeting { get; set; }

        public MeetingDetailViewModel(IEventAggregator eventAggregator,
            IMessageDialogService dialogService,
            IMeetingRepository meetingRepository) : base(eventAggregator)
        {
            _dialogService = dialogService;
            _repository = meetingRepository;
        }

        public override async Task LoadAsync(int? id)
        {
            var meeting = id.HasValue ? await _repository.GetByIdAsync(id.Value) : CreateNewMeeting();

            InitMeeting(meeting);
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
            };
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

            //Powiadom agregator eventów, że zapisano
            RaiseDetailSavedEvent(Meeting.Id, Meeting.Name);
        }

        protected override bool OnSaveCanExecute()
        {
            return Meeting != null && !Meeting.HasErrors && HasChanges;
        }

        protected override void OnDeleteExecute()
        {
            var result = _dialogService.ShowOkCancelDialog("Delete?", "Confirm");
            if (result == MessageDialogRessult.Cancel) return;

            _repository.Remove(Meeting.Model);
            _repository.SaveAsync();
            RaiseDetailDeletedEvent(Meeting.Id);
        }
    }
}
