using Autofac;
using FriendOrganizer.DataAcces;
using FriendOrganizer.UI.Data.Lookups;
using FriendOrganizer.UI.Data.Repositories;
using FriendOrganizer.UI.ViewModel;
using FriendOrganizer.View.UI.Services;
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

            //Serwisy
            builder.RegisterType<MessageDialogService>().As<IMessageDialogService>(); //MessageBox service

            //Usługa przeglądania itemów na listach
            builder.RegisterType<LookupDataService>().AsImplementedInterfaces();

            //Usługi bazodanowe, wrapper
            builder.RegisterType<FriendRepository>().As<IFriendRepository>();

            return builder.Build();
        }
    }
}
