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
            Meetings = new Collection<Meeting>();
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

        #region Relacje

        //Ulubiony język
        public ProgrammingLanguage FavoriteLanguage { get; set; }

        //Numery telefonów
        public ICollection<FriendPhoneNumber> PhoneNumbers { get; set; }

        //Spotkania
        public ICollection<Meeting> Meetings { get; set; }

        #endregion
    }
}
