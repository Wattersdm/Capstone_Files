using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TossAway2.Classes
{
    public class Event
    {
        #region Properties

        private int calendarEventId { get; set; }
        public int CalendarEventId
        {
            get => calendarEventId;
            set
            {
                calendarEventId = value;
            }
        }
        private string description { get; set; }
        public string Description
        {
            get => description;
            set
            {
                description = value;
            }
        }

        private DateTime eventStart { get; set; }
        public DateTime EventStart
        {
            get => eventStart;
            set
            {
                eventStart = value;
            }
        }

        private DateTime eventEnd { get; set; }
        public DateTime EventEnd
        {
            get => eventEnd;
            set
            {
                eventEnd = value;
            }
        }

        private string location { get; set; }
        public string Location
        {
            get => location;
            set
            {
                location = value;
            }
        }

        private int responsibleUserId { get; set; }
        public int ResponsibleUserId
        {
            get => responsibleUserId;
            set
            {
                responsibleUserId = value;
            }
        }

        private int childUserId { get; set; }
        public int ChildUserId
        {
            get => childUserId;
            set
            {
                childUserId = value;
            }
        }

        #endregion

        #region Constructors

        public Event() { } // default

        public Event(Dictionary<string, string> eventData)
        {
            calendarEventId = Convert.ToInt32(eventData["calendarEventId"]);
            description = eventData["description"];
            eventStart = Convert.ToDateTime(eventData["eventStart"]);
            eventEnd = Convert.ToDateTime(eventData["eventEnd"]);
            location = eventData["location"];
            responsibleUserId = Convert.ToInt32(eventData["responsibleUserId"]);
            childUserId = Convert.ToInt32(eventData["childUserId"]);
        }

        #endregion

        #region Methods

        public bool updateEvent(Dictionary<string, string> eventData)
        {
            bool isUpdated = false;

            connect Connect = new connect();
            if (Connect.UpdateEventData(eventData))
            {
                // update object data
                description = eventData["description"];
                eventStart = Convert.ToDateTime(eventData["eventStart"]);
                eventEnd = Convert.ToDateTime(eventData["eventEnd"]);
                location = eventData["location"];
                responsibleUserId = Convert.ToInt32(eventData["responsibleUserId"]);
                childUserId = Convert.ToInt32(eventData["childUserId"]);

                isUpdated = true;
            }

            return isUpdated;
        }

        #endregion
    }
}
