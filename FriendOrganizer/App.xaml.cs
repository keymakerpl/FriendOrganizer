using System.Windows;
using Autofac;
using FriendOrganizer.UI.Startup;

namespace FriendOrganizer.UI
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            //var mainWindowView = new MainWindow(new MainViewModel(new FriendDataService()));
            //mainWindowView.Show();

            var bootstrapper = new Bootstrapper();
            var container = bootstrapper.Bootstrap();

            var mainWindow = container.Resolve<MainWindow>();
            mainWindow.Show();
        }
    }
}
