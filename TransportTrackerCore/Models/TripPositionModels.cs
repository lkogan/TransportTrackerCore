using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TransportTrackerCore.Models
{
    public class TripPositionModels
    { 
        public class Trip
        {
            [JsonProperty("trip_id")]
            public string trip_id { get; set; }

            [JsonProperty("route_id")]
            public string route_id { get; set; }

            [JsonProperty("direction_id")]
            public object direction_id { get; set; }

            [JsonProperty("start_time")]
            public string start_time { get; set; }

            [JsonProperty("start_date")]
            public string start_date { get; set; }

            [JsonProperty("schedule_relationship")]
            public bool schedule_relationship { get; set; }
        }

        public class Vehicle2
        {
            [JsonProperty("id")]
            public string id { get; set; }

            [JsonProperty("label")]
            public string label { get; set; }

            [JsonProperty("license_plate")]
            public object license_plate { get; set; }
        }

        public class Position
        {
            [JsonProperty("latitude")]
            public double latitude { get; set; }

            [JsonProperty("longitude")]
            public double longitude { get; set; }

            [JsonProperty("bearing")]
            public object bearing { get; set; }

            [JsonProperty("odometer")]
            public object odometer { get; set; }

            [JsonProperty("speed")]
            public object speed { get; set; }
        }

        public class Timestamp
        {
            [JsonProperty("low")]
            public DateTime low { get; set; }

            [JsonProperty("high")]
            public int high { get; set; }

            [JsonProperty("unsigned")]
            public bool unsigned { get; set; }
        }

        public class Vehicle
        {
            [JsonProperty("trip")]
            public Trip trip { get; set; }

            [JsonProperty("vehicle")]
            public Vehicle2 vehicle { get; set; }

            [JsonProperty("position")]
            public Position position { get; set; }

            [JsonProperty("current_stop_sequence")]
            public object current_stop_sequence { get; set; }

            [JsonProperty("stop_id")]
            public object stop_id { get; set; }

            [JsonProperty("current_status")]
            public int current_status { get; set; }

            [JsonProperty("timestamp")]
            public Timestamp timestamp { get; set; }

            [JsonProperty("congestion_level")]
            public object congestion_level { get; set; }

            [JsonProperty("occupancy_status")]
            public object occupancy_status { get; set; }
        }

        public class TripPosition
        {
            [JsonProperty("id")]
            public string id { get; set; }

            [JsonProperty("is_deleted")]
            public bool is_deleted { get; set; }

            [JsonProperty("trip_update")]
            public object trip_update { get; set; }

            [JsonProperty("vehicle")]
            public Vehicle vehicle { get; set; }

            [JsonProperty("alert")]
            public object alert { get; set; }
        }
    }


}