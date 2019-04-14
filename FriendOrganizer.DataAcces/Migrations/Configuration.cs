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

            context.ProgrammingLanguages.AddOrUpdate(pl => pl.Name,
                new ProgrammingLanguage(){Name = "C#"},
                new ProgrammingLanguage(){Name = "TypeScript"},
                new ProgrammingLanguage(){Name = "Java"},
                new ProgrammingLanguage(){Name = "Objective-C"});

            context.SaveChanges();

            context.FriendPhoneNumbers.AddOrUpdate(nm => nm.Number,
                new FriendPhoneNumber(){Number = "+48 12345678", FriendId = context.Friends.First().Id});
        }
    }
}
