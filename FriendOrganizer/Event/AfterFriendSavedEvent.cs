using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Events;

namespace FriendOrganizer.Event
{
    public class AfterFriendSavedEvent : PubSubEvent<AfterFriendSavedEventArgs>
    {

    }

    public class AfterFriendSavedEventArgs
    {
        public int Id { get; set; }
        public string DisplayMember { get; set; }
    }
}
