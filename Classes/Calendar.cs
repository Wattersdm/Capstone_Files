using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TossAway2.Classes
{
    public class Calendar
    {
        #region Properties

        private int childUserId { get; set; }
        public int ChildUserId
        {
            get => childUserId;
            set
            {
                childUserId = value;
            }
        }

        private List<Event> events { get; set; }
        public List<Event> Events
        {
            get => events;
            set
            {
                events = value;
            }
        }

        #endregion

        #region Constructors

        public Calendar() { } // default

        public Calendar(List<Event> newEvents, int userId)
        {
            events = newEvents;
            childUserId = userId;
        }
        #endregion

        #region Methods

        // ------------------------------------------------------------------------
        // Method: RemoveEventFromCalendar()
        // Purpose: Removes child as an attendee from a specific event
        // NOTE:    If child was the only attendee, the event will be made inactive
        //          in the DB
        // ------------------------------------------------------------------------
        public bool RemoveEventFromCalendar(int calendarEventId, int childUserId)
        {
            bool isRemoved = false;
            connect Connect = new connect();
            foreach (Event ev in events)
            {
                if (ev.CalendarEventId == calendarEventId)
                {
                    if (Connect.RemoveChildFromEvent(calendarEventId, childUserId))
                    {
                        events.Remove(ev);
                        isRemoved = true;
                        break;
                    }
                }
            }
            return isRemoved;
        }

        // ------------------------------------------------------------------------
        // Method: CancelEvent()
        // Purpose: Cancels event and removes ALL attendees
        // ------------------------------------------------------------------------
        public bool CancelEvent(int calendarEventId)
        {
            bool isCancelled = false;

            connect Connect = new connect();
            foreach (Event ev in events)
            {
                if (ev.CalendarEventId == calendarEventId)
                {
                    if (Connect.CancelEntireEvent(calendarEventId))
                    {
                        events.Remove(ev);
                        isCancelled = true;
                        break;
                    }
                }
            }

            return isCancelled;
        }

        #endregion
    }
}
