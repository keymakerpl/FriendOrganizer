using System;
using System.Threading.Tasks;

namespace FriendOrganizer.UI.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public INavigationViewModel NavigationViewModel { get; }
        public IFriendDetailViewModel FriendDetailViewModel { get; }

        /// <summary>
        /// Główne okno programu, przyjmuje jako parametr Nawigator
        /// </summary>
        /// <param name="navigationViewModel"></param>
        public MainViewModel(INavigationViewModel navigationViewModel, IFriendDetailViewModel friendDetailViewModel)
        {
            NavigationViewModel = navigationViewModel;
            FriendDetailViewModel = friendDetailViewModel;
        }

        public void Load()
        {
            throw new NotImplementedException();
        }

        public async Task LoadAsync()
        {
            await NavigationViewModel.LoadAsync();
        }
            
    }
}
