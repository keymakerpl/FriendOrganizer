using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using FriendOrganizer.Model;
using FriendOrganizer.UI.Data.Lookups;
using FriendOrganizer.UI.Data.Repositories;
using FriendOrganizer.View.UI.Services;
using FriendOrganizer.Wrapper;
using Prism.Commands;
using Prism.Events;

namespace FriendOrganizer.UI.ViewModel
{
    public class ProgrammingLanguageDetailViewModel : DetailViewModelBase
    {
        private IProgrammingLanguageRepository _repository;

        private ProgrammingLanguageWrapper _selectedProgrammingLanguage;
        public ProgrammingLanguageWrapper SelectedProgrammingLanguage
        {
            get { return _selectedProgrammingLanguage; }
            set
            {
                _selectedProgrammingLanguage = value;
                OnPropertyChanged();
                ((DelegateCommand)RemoveCommand).RaiseCanExecuteChanged();
            }
        }
        public ICommand AddCommand { get; }
        public ICommand RemoveCommand { get; }

        public ProgrammingLanguageDetailViewModel(IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService,
            IProgrammingLanguageRepository repository)
            : base(eventAggregator,
                messageDialogService)
        {
            _repository = repository;

            Title = "Programming Language";
            ProgrammingLanguages = new ObservableCollection<ProgrammingLanguageWrapper>();

            AddCommand = new DelegateCommand(OnAddCommand);
            RemoveCommand = new DelegateCommand(OnRemoveCommand, OnCanRemoveCommand);
        }

        private bool OnCanRemoveCommand()
        {
            return SelectedProgrammingLanguage != null;
        }

        private async void OnRemoveCommand()
        {
            var isReferenced = await _repository.IsReferencedByFriendAsync(SelectedProgrammingLanguage.Id);
            if (isReferenced)
            {
                MessageDialogService.ShowInfoDialog($"Cannot delete referenced language: {SelectedProgrammingLanguage.Name}");
                return;
            }

            SelectedProgrammingLanguage.PropertyChanged -= Wrapper_PropertyChanged; //odłącz eventy od usuwanego elementu
            _repository.Remove(SelectedProgrammingLanguage.Model); //wywal z repo
            ProgrammingLanguages.Remove(SelectedProgrammingLanguage); //wywal z listy 
            SelectedProgrammingLanguage = null; //wybrany teraz jest null 
            HasChanges = _repository.HasChanges(); // w repo zaszły zmiany więc ustawiamy flagę w modelu
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged(); // odpalamy event i sprawdzamy czy Save może być aktywny
        }

        private void OnAddCommand()
        {
            var wrapper = new ProgrammingLanguageWrapper(new ProgrammingLanguage()); //utwórz nowy element, wrapper+model
            wrapper.PropertyChanged += Wrapper_PropertyChanged; //podłącz eventy
            _repository.Add(wrapper.Model); // dodaj nowy element do repo
            ProgrammingLanguages.Add(wrapper); //dodaj do listy

            wrapper.Name = ""; // tacznij triger walidacji
        }

        public ObservableCollection<ProgrammingLanguageWrapper> ProgrammingLanguages { get; set; }

        public async override Task LoadAsync(int id)
        {
            // Przydziel id modelu do detailmodelview
            Id = id;

            //odłącz eventy
            foreach (var languageWrapper in ProgrammingLanguages)
            {
                languageWrapper.PropertyChanged -= Wrapper_PropertyChanged;
            }

            //wyczyść listę języków
            ProgrammingLanguages.Clear();

            //wypełnij ponownie i podepnij eventy
            var languages = await _repository.GetAllAsync();
            foreach (var programmingLanguage in languages)
            {
                var wrapper = new ProgrammingLanguageWrapper(programmingLanguage);
                wrapper.PropertyChanged += Wrapper_PropertyChanged;
                ProgrammingLanguages.Add(wrapper);
            }

        }

        private void Wrapper_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!HasChanges) //odśwerzamy z repo czy już zaszły jakieś zmiany, nie odpalamy jeśli już jest True
            {
                HasChanges = _repository.HasChanges();
            }

            if (e.PropertyName == nameof(ProgrammingLanguageWrapper.HasErrors)) //sprawdzamy czy możemy sejwować
            {
                ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            }
        }

        protected async override void OnSaveExecute()
        {
            await _repository.SaveAsync(); //zapisujemy przez repo
            HasChanges = _repository.HasChanges();
            RaiseCollectionSavedEvent();
        }

        protected override bool OnSaveCanExecute()
        {
            return HasChanges && ProgrammingLanguages.All(p => !p.HasErrors);            
        }

        protected override void OnDeleteExecute()
        {
            throw new System.NotImplementedException(); //nie implementuje, nie chcemy kasować detalu języków programowania
        }
    }
}
