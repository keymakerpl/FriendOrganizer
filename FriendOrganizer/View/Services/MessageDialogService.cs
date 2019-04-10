using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FriendOrganizer.View.UI.Services
{
    public class MessageDialogService : IMessageDialogService
    {
        public MessageDialogRessult ShowOkCancelDialog(string text, string title)
        {
            var result = MessageBox.Show(text, title, MessageBoxButton.OKCancel);
            return result == MessageBoxResult.OK
                ? MessageDialogRessult.OK
                : MessageDialogRessult.Cancel;
        }
    }

    public enum MessageDialogRessult
    {
        OK,
        Cancel
    }
}
