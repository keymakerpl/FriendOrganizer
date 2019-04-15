using System;
using FriendOrganizer.Model;
using FriendOrganizer.UI.Wrapper;

namespace FriendOrganizer.Wrapper
{
    public class MeetingWrapper : ModelWrapper<Meeting>
    {
        public MeetingWrapper(Meeting model) : base(model)
        {
            
        }

        public int Id
        {
            get { return Model.Id; }
        }

        public string Name
        {
            get { return GetValue<string>(); }
            set
            {
                SetValue(value);
            }
        }

        public DateTime DateFrom
        {
            get { return GetValue<DateTime>(); }
            set { SetValue(value);}
        }

        public DateTime DateTo
        {
            get { return GetValue<DateTime>(); }
            set { SetValue(value); }
        }
    }
}
