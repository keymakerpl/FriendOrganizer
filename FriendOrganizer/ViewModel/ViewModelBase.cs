using System.ComponentModel;
using System.Runtime.CompilerServices;
using FriendOrganizer.UI.Annotations;

namespace FriendOrganizer.UI.ViewModel
{
    /// <summary>
    /// Klasa bazowa modelu widoku, odpala event przy zmianie propertisa.
    /// </summary>
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}