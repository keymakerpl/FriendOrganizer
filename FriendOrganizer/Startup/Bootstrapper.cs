using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using FriendOrganizer.DataAcces;
using FriendOrganizer.UI.Data;
using FriendOrganizer.UI.ViewModel;

namespace FriendOrganizer.UI.Startup
{
    public class Bootstrapper
    {
        /// <summary>
        /// Kontener do wiązania widok-model widoku. DI.
        /// </summary>
        /// <returns></returns>
        public IContainer Bootstrap()
        {
            var builder = new ContainerBuilder();

            //Baza danych DbContext, inicjalizacja, zarzadzanie itd.
            builder.RegisterType<FriendOrganizerDbContext>().AsSelf();

            //ViewModel - View
            builder.RegisterType<MainWindow>().AsSelf();
            builder.RegisterType<MainViewModel>().AsSelf();
            builder.RegisterType<NavigationViewModel>().As<INavigationViewModel>(); //nawigacja
            
            //Usługa przeglądania itemów na listach
            builder.RegisterType<LookupDataService>().AsImplementedInterfaces();

            //Usługi bazodanowe
            builder.RegisterType<FriendDataService>().As<IFriendDataService>();

            return builder.Build();
        }
    }
}
