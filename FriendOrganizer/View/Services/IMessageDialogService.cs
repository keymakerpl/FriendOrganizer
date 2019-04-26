using System.Threading.Tasks;

namespace FriendOrganizer.View.UI.Services
{
    public interface IMessageDialogService
    {
        Task<MessageDialogResult> ShowOkCancelDialog(string text, string title);
        void ShowInfoDialog(string text);
    }
}