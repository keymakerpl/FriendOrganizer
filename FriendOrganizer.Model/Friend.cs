using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace FriendOrganizer.Model
{
    public class Friend
    {
        public Friend()
        {
            Init();
        }

        private void Init()
        {
            PhoneNumbers = new Collection<FriendPhoneNumber>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [StringLength(50)]
        public string LastName { get; set; }

        [StringLength(50)]
        [EmailAddress]
        public string Email { get; set; }

        /// <summary>
        /// Wybrany ulubiony język, zbindowane w widoku z lookup itemem
        /// </summary>
        public int? FavoriteLanguageId { get; set; }
        
        public ProgrammingLanguage FavoriteLanguage { get; set; }

        //kolekcja - numery telefonów
        public ICollection<FriendPhoneNumber> PhoneNumbers { get; set; }
    }
}
