using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FriendOrganizer.Model;

namespace FriendOrganizer.Data
{
    public class FriendDataService
    {

        //TODO: Load from DB
        public IEnumerable<Friend> GetAll()
        {
            yield return new Friend() { FirstName = "Jan", LastName = "Nowak", Email = "nowak@em.pl"};
            yield return new Friend() { FirstName = "Anna", LastName = "Nowak", Email = "nowak2@em.pl" };
            yield return new Friend() { FirstName = "Marek", LastName = "Kawałek", Email = "32234@em.pl" };
            yield return new Friend() { FirstName = "Stefan", LastName = "Wolny", Email = "wlk@em.pl" };
            yield return new Friend() { FirstName = "Marta", LastName = "Faworek", Email = "fww2@em.pl" };

        }
    }
}
