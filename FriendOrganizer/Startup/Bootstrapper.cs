using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using FriendOrganizer.DataAcces;
using FriendOrganizer.UI.Data;
using FriendOrganizer.UI.ViewModel;
using Prism.Events;

namespace FriendOrganizer.UI.Startup
{
    public class Bootstrapper
    {
        /// <summary>
        /// Kontener do wiązania widok-model widoku. Depency Injection
        /// </summary>
        /// <returns></returns>
        public IContainer Bootstrap()
        {
            var builder = new ContainerBuilder();

            //Prism Agregator zdarzeń, publikuje/subskrybuje eventy pomiędzy modelami
            builder.RegisterType<EventAggregator>().As<IEventAggregator>().SingleInstance();

            //Baza danych DbContext, inicjalizacja, zarzadzanie itd.
            builder.RegisterType<FriendOrganizerDbContext>().AsSelf();

            //ViewModel - View, tworzy instancje modeli i widoku
            builder.RegisterType<MainWindow>().AsSelf();
            builder.RegisterType<MainViewModel>().AsSelf();
            builder.RegisterType<NavigationViewModel>().As<INavigationViewModel>();
            builder.RegisterType<FriendDetailViewModel>().As<IFriendDetailViewModel>();

            //Usługa przeglądania itemów na listach
            builder.RegisterType<LookupDataService>().AsImplementedInterfaces();

            //Usługi bazodanowe, wrapper
            builder.RegisterType<FriendDataService>().As<IFriendDataService>();

            return builder.Build();
        }
    }
}
