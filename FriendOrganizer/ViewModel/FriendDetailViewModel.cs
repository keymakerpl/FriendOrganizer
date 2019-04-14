using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using FriendOrganizer.Event;
using FriendOrganizer.Model;
using FriendOrganizer.UI.Data.Lookups;
using FriendOrganizer.UI.Data.Repositories;
using FriendOrganizer.UI.Wrapper;
using FriendOrganizer.View.UI.Services;
using FriendOrganizer.Wrapper;
using Prism.Commands;
using Prism.Events;

namespace FriendOrganizer.UI.ViewModel
{
    public class FriendDetailViewModel : DetailViewModelBase, IFriendDetailViewModel
    {
        public ICommand AddPhoneNumberCommand { get; }
        public ICommand RemovePhoneNumberCommand { get; }

        private IFriendRepository _repository;
        private IMessageDialogService _dialogService;
        private IProgrammingLanguageLookupDataService _programmingLanguagesLookupService;

        private FriendPhoneNumberWrapper _selectedPhoneNumber;
        public FriendPhoneNumberWrapper SelectedNumber
        {
            get { return _selectedPhoneNumber; }
            set
            {
                _selectedPhoneNumber = value;
                OnPropertyChanged();
                ((DelegateCommand)RemovePhoneNumberCommand).RaiseCanExecuteChanged();
            }
        }

        private FriendWrapper _friend;
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
        public FriendDetailViewModel(IFriendRepository repository,
            IEventAggregator eventAggregator,
            IMessageDialogService dialogService,
            IProgrammingLanguageLookupDataService programmingLanguageLookupDataService) : base(eventAggregator)
        {
            _repository = repository;
            _dialogService = dialogService;
            _programmingLanguagesLookupService = programmingLanguageLookupDataService;            

            AddPhoneNumberCommand = new DelegateCommand(OnAddPhoneNumberExecute);
            RemovePhoneNumberCommand = new DelegateCommand(OnRemovePhoneNumberExecute, OnRemovePhoneNumberCanExecute);

            //Właściwość do przechowania lookup itemów w liście
            ProgrammingLanguages = new ObservableCollection<LookupItem>();
            PhoneNumbers = new ObservableCollection<FriendPhoneNumberWrapper>();
        }

        public override async Task LoadAsync(int? friendId)
        {
            //Pobiera model z repo
            var friend = friendId.HasValue ?
                await _repository.GetByIdAsync(friendId.Value) : CreateNewFriend();

            InitFriend(friend);

            InitializeFriendPhoneNumbers(friend.PhoneNumbers);

            await LoadProgrammingLanguagesAsync();
        }        

        public ObservableCollection<LookupItem> ProgrammingLanguages { get; }

        public ObservableCollection<FriendPhoneNumberWrapper> PhoneNumbers { get; }

        private void InitFriend(Friend friend)
        {
            int? friendId;
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
                    ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
                }
            });
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();

            if (Friend.Id == 0)
                Friend.FirstName = ""; // takie se, trzeba tacznąć propertisa aby zadziałała walidacja nowego detalu
        }

        private void InitializeFriendPhoneNumbers(ICollection<FriendPhoneNumber> friendPhoneNumbers)
        {
            foreach (var number in PhoneNumbers)
            {
                number.PropertyChanged -= FriendPhoneNumberWrapper_PropertyChanged;
            }

            PhoneNumbers.Clear();

            foreach (var number in friendPhoneNumbers)
            {
                var wrapper = new FriendPhoneNumberWrapper(number);
                PhoneNumbers.Add(wrapper);
                wrapper.PropertyChanged += FriendPhoneNumberWrapper_PropertyChanged;
            }
        }

        private void FriendPhoneNumberWrapper_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!HasChanges)
            {
                HasChanges = _repository.HasChanges();
            }

            if (e.PropertyName == nameof(FriendPhoneNumberWrapper.HasErrors))
            {
                ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            }
        }        

        private async Task LoadProgrammingLanguagesAsync()
        {
            //wypełniamy listę lookapami
            ProgrammingLanguages.Clear();
            ProgrammingLanguages.Add(new NullLookupItem());
            var lookup = await _programmingLanguagesLookupService.GetProgrammingLanguageLookupAsync();
            foreach (var lookupItem in lookup)
            {
                ProgrammingLanguages.Add(lookupItem);
            }
        }        

        /// <summary>
        /// Sprawdzamy czy model ma błędy walidacji i czy repozytorium zostało zmienione
        /// </summary>
        /// <returns></returns>
        protected override bool OnSaveCanExecute()
        {            
            return Friend != null && !Friend.HasErrors && HasChanges && PhoneNumbers.All(pn => !pn.HasErrors);
        }

        protected override async void OnSaveExecute()
        {
            await _repository.SaveAsync();
            HasChanges = _repository.HasChanges(); // Po zapisie ustawiamy flagę na false jeśli nie ma zmian w repo
            
            //Powiadom agregator eventów, że zapisano
            RaiseDetailSavedEvent(Friend.Id, $"{Friend.FirstName} {Friend.LastName}");            
        }

        protected override async void OnDeleteExecute()
        {
            var result = _dialogService.ShowOkCancelDialog("Delete?", "Confirm");
            if (result == MessageDialogRessult.Cancel) return;

            _repository.Remove(Friend.Model);
            await _repository.SaveAsync();
            RaiseDetailDeletedEvent(Friend.Id);            
        }

        private bool OnRemovePhoneNumberCanExecute()
        {
            return SelectedNumber != null;
        }

        private void OnRemovePhoneNumberExecute()
        {
            SelectedNumber.PropertyChanged -= FriendPhoneNumberWrapper_PropertyChanged;
            _repository.RemovePhoneNumber(SelectedNumber.Model);
            PhoneNumbers.Remove(SelectedNumber);
            SelectedNumber = null;
            HasChanges = _repository.HasChanges();
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        }

        private void OnAddPhoneNumberExecute()
        {
            var newNumber = new FriendPhoneNumberWrapper(new FriendPhoneNumber());
            newNumber.PropertyChanged += FriendPhoneNumberWrapper_PropertyChanged;
            PhoneNumbers.Add(newNumber);
            Friend.Model.PhoneNumbers.Add(newNumber.Model);
            newNumber.Number = "";
        }

        private Friend CreateNewFriend()
        {
            var friend = new Friend();
            _repository.Add(friend);

            return friend;
        }
    }
}
