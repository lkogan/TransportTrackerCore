using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TransportTrackerCore.Models
{
    public class TripUpdateModels
    {

        public class Trip
        {
            public string trip_id { get; set; }
            public string route_id { get; set; }
            public object direction_id { get; set; }
            public string start_time { get; set; }
            public string start_date { get; set; }
            public int schedule_relationship { get; set; }
        }

        public class Vehicle
        {
            public string id { get; set; }
            public string label { get; set; }
            public object license_plate { get; set; }
        }

        public class Time
        {
            public DateTime low { get; set; }
            public int high { get; set; }
            public bool unsigned { get; set; }
        }

        public class Arrival
        {
            public int delay { get; set; }
            public Time time { get; set; }
            public int uncertainty { get; set; }
        }

        public class Time2
        {
            public DateTime low { get; set; }
            public int high { get; set; }
            public bool unsigned { get; set; }
        }

        public class Departure
        {
            public int delay { get; set; }
            public Time2 time { get; set; }
            public int uncertainty { get; set; }
        }

        public class StopTimeUpdate
        {
            public int stop_sequence { get; set; }
            public string stop_id { get; set; }
            public Arrival arrival { get; set; }
            public Departure departure { get; set; }
            public int schedule_relationship { get; set; }
        }

        public class Timestamp
        {
            public DateTime low { get; set; }
            public int high { get; set; }
            public bool unsigned { get; set; }
        }

        public class Trip2
        {
            public string trip_id { get; set; }
            public string route_id { get; set; }
            public object direction_id { get; set; }
            public string start_time { get; set; }
            public string start_date { get; set; }
            public int schedule_relationship { get; set; }
        }

        public class Vehicle3
        {
            public string id { get; set; }
            public string label { get; set; }
            public object license_plate { get; set; }
        }

        public class Position2
        {
            public double latitude { get; set; }
            public double longitude { get; set; }
            public object bearing { get; set; }
            public object odometer { get; set; }
            public object speed { get; set; }
        }

        public class Timestamp2
        {
            public DateTime low { get; set; }
            public int high { get; set; }
            public bool unsigned { get; set; }
        }

        public class Vehicle2
        {
            public Trip2 trip { get; set; }
            public Vehicle3 vehicle { get; set; }
            public Position2 position { get; set; }
            public object current_stop_sequence { get; set; }
            public object stop_id { get; set; }
            public int current_status { get; set; }
            public Timestamp2 timestamp { get; set; }
            public object congestion_level { get; set; }
            public object occupancy_status { get; set; }
        }

        public class Position
        {
            public string id { get; set; }
            public bool is_deleted { get; set; }
            public object trip_update { get; set; }
            public Vehicle2 vehicle { get; set; }
            public object alert { get; set; }
        }

        public class TripUpdate
        {
            public Trip trip { get; set; }
            public Vehicle vehicle { get; set; }
            public List<StopTimeUpdate> stop_time_update { get; set; }
            public Timestamp timestamp { get; set; }
            public object delay { get; set; }
            public Position position { get; set; }
        }

        public class TripUpdateCollection
        {
            public string id { get; set; }
            public bool is_deleted { get; set; }
            public TripUpdate trip_update { get; set; }
            public object vehicle { get; set; }
            public object alert { get; set; }

            //public DateTime __invalid_name__metra-publish-tstamp { get; set; }
    }
}
}