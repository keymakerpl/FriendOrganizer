using System;
using System.Windows;
using System.Windows.Threading;
using Autofac;
using FriendOrganizer.UI.Startup;

namespace FriendOrganizer.UI
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            //var mainWindowView = new MainWindow(new MainViewModel(new FriendRepository()));
            //mainWindowView.Show();

            var bootstrapper = new Bootstrapper();
            var container = bootstrapper.Bootstrap();

            var mainWindow = container.Resolve<MainWindow>();
            mainWindow.Show();
        }


        private void App_OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("Unexpected error occured."
                            + Environment.NewLine + e.Exception.Message 
                            + "Inner exception:"
                            + Environment.NewLine + e.Exception.InnerException != null ? e.Exception.InnerException.Message : ""
                                , "Unexpected Error");

            e.Handled = true;
        }
    }
}
