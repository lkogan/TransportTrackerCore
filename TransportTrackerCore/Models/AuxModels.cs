using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TransportTrackerCore.Models
{
    public enum Direction
    {
        Outbound = 0,
        Inbound = 1
    }

    public class AuxModels
    { 
        public class StopOnTrip
        {
            public string trip_id { get; set; }
            public string arrival_time { get; set; }
            public string departure_time { get; set; }
            public string stop_id { get; set; }
            public int stop_sequence { get; set; }
            public int pickup_type { get; set; }
            public int drop_off_type { get; set; }
            public int center_boarding { get; set; }
            public int south_boarding { get; set; }
            public int bikes_allowed { get; set; }
            public int notice { get; set; }
        }

        public class Station
        {
            public string stop_id { get; set; }
            public string stop_name { get; set; }
            public string stop_desc { get; set; }
            public double stop_lat { get; set; }
            public double stop_lon { get; set; }
            public string zone_id { get; set; }
            public string stop_url { get; set; }
            public int wheelchair_boarding { get; set; }
        }

    }
}