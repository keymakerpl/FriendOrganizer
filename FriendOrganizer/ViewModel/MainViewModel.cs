using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FriendOrganizer.Model;

namespace FriendOrganizer.ViewModel
{
    class MainViewModel
    {
        public MainViewModel()
        {
            
        }

        public void Load()
        {

        }

        public ObservableCollection<Friend> Friends { get; set; }
    }
}
