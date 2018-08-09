using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TransportTrackerCore.Models
{
    public class AlertModels
    { 
        public class AlertData
        {
            public string id { get; set; }
            public bool is_deleted { get; set; }
            public object trip_update { get; set; }
            public object vehicle { get; set; }
            public Alert alert { get; set; }
        }
        public class Start
        {
            public DateTime low { get; set; }
            public int high { get; set; }
            public bool unsigned { get; set; }
        }

        public class End
        {
            public DateTime low { get; set; }
            public int high { get; set; }
            public bool unsigned { get; set; }
        }

        public class ActivePeriod
        {
            public Start start { get; set; }
            public End end { get; set; }
        }

        public class Trip
        {
            public string trip_id { get; set; }
            public string route_id { get; set; }
            public object direction_id { get; set; }
            public string start_time { get; set; }
            public string start_date { get; set; }
            public int schedule_relationship { get; set; }
        }

        public class InformedEntity
        {
            public object agency_id { get; set; }
            public string route_id { get; set; }
            public object route_type { get; set; }
            public string trip { get; set; }
            public string stop_id { get; set; }
        }

        public class Translation
        {
            public string text { get; set; }
            public string language { get; set; }
        }

        public class Url
        {
            public List<Translation> translation { get; set; }
        }

        public class Translation2
        {
            public string text { get; set; }
            public string language { get; set; }
        }

        public class HeaderText
        {
            public List<Translation2> translation { get; set; }
        }

        public class Translation3
        {
            public string text { get; set; }
            public string language { get; set; }
        }

        public class DescriptionText
        {
            public List<Translation3> translation { get; set; }
        }

        public class Alert
        {
            public List<ActivePeriod> active_period { get; set; }
            public List<InformedEntity> informed_entity { get; set; }
            public int cause { get; set; }
            public int effect { get; set; }
            public Url url { get; set; }
            public HeaderText header_text { get; set; }
            public DescriptionText description_text { get; set; }
        }
    }


   
}
