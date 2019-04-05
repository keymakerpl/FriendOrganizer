using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FriendOrganizer.UI.ViewModel;

namespace FriendOrganizer.ViewModel
{
    public class NavigationItemViewModel : ViewModelBase
    {
        private string _displayMember;

        public NavigationItemViewModel(int id, string displayMember)
        {
            Id = id;
            DisplayMember = displayMember;
        }

        public string DisplayMember
        {
            get { return _displayMember; }
            set
            { _displayMember = value;
                OnPropertyChanged();
            }
        }

        public int Id { get; set; }
    }
}
