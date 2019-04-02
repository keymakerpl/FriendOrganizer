using FriendOrganizer.Model;

namespace FriendOrganizer.DataAcces.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<FriendOrganizerDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(FriendOrganizerDbContext context)
        {
            context.Friends.AddOrUpdate(e => e.FirstName,
                new Friend() { FirstName = "Jan", LastName = "Nowak"},
                new Friend(){ FirstName = "Marek", LastName = "Kawa³ek"},
                new Friend(){FirstName = "Anna", LastName = "Nowak"});
        }
    }
}
