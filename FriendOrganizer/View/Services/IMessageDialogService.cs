namespace FriendOrganizer.View.UI.Services
{
    public interface IMessageDialogService
    {
        MessageDialogRessult ShowOkCancelDialog(string text, string title);
    }
}