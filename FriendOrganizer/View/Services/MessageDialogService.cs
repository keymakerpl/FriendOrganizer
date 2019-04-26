using System.Threading.Tasks;
using FriendOrganizer.UI;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace FriendOrganizer.View.UI.Services
{
    public class MessageDialogService : IMessageDialogService
    {
        private MetroWindow MetroWindow => (MetroWindow)App.Current.MainWindow;
        public async Task<MessageDialogResult> ShowOkCancelDialog(string text, string title)
        {
            var result = await MetroWindow.ShowMessageAsync(title, text, MessageDialogStyle.AffirmativeAndNegative);
            
            return result == MahApps.Metro.Controls.Dialogs.MessageDialogResult.Affirmative
                ? MessageDialogResult.OK
                : MessageDialogResult.Cancel;
        }

        public void ShowInfoDialog(string text)
        {
            MetroWindow.ShowMessageAsync(text, "Info");
        }
    }    

    public enum MessageDialogResult
    {
        OK,
        Cancel
    }
}
