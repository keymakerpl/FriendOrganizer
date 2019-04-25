using Autofac;
using FriendOrganizer.Data.Repositories;
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
        /// Kontener do inicjalizacji instancji. Depency Injection zamiast new
        /// </summary>
        /// <returns></returns>
        public IContainer Bootstrap()
        {
            var builder = new ContainerBuilder();

            //Prism Agregator zdarzeń, publikuje/subskrybuje eventy pomiędzy modelami
            builder.RegisterType<EventAggregator>().As<IEventAggregator>().SingleInstance();

            //Baza danych DbContext, inicjalizacja, zarzadzanie itd.
            builder.RegisterType<FriendOrganizerDbContext>().AsSelf();

            //ViewModel - View, tworzy instancje modeli dla widoku
            builder.RegisterType<MainWindow>().AsSelf();
            builder.RegisterType<MainViewModel>().AsSelf();
            builder.RegisterType<NavigationViewModel>().As<INavigationViewModel>();

            //Autofac wrzuci odpowiedni model do konstruktora mainviewmodel po kluczu
            builder.RegisterType<FriendDetailViewModel>()
                .Keyed<IDetailViewModel>(nameof(FriendDetailViewModel));
            builder.RegisterType<MeetingDetailViewModel>()
                .Keyed<IDetailViewModel>(nameof(MeetingDetailViewModel));
            builder.RegisterType<ProgrammingLanguageDetailViewModel>()
                .Keyed<IDetailViewModel>(nameof(ProgrammingLanguageDetailViewModel));            

            //Serwisy
            builder.RegisterType<MessageDialogService>().As<IMessageDialogService>(); //MessageBox service

            //Usługa przeglądania itemów na listach
            builder.RegisterType<LookupDataService>().AsImplementedInterfaces(); // Skorzysta z interfejsu który jest wymagany, AsImplemented

            //Repozytoria
            builder.RegisterType<FriendRepository>().As<IFriendRepository>();
            builder.RegisterType<MeetingRepository>().As<IMeetingRepository>();
            builder.RegisterType<ProgrammingLanguageRepository>().As<IProgrammingLanguageRepository>();

            return builder.Build();
        }
    }
}
