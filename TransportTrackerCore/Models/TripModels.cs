﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using static TransportTrackerCore.Models.AuxModels;
using j = TransportTrackerCore.Models.JSON_Models;

namespace TransportTrackerCore.Models
{
    public class ServicePeriod
    {
        [JsonProperty("service_id")]
        public string service_id { get; set; }

        [JsonProperty("monday")]
        public int monday { get; set; }

        [JsonProperty("tuesday")]
        public int tuesday { get; set; }

        [JsonProperty("wednesday")]
        public int wednesday { get; set; }

        [JsonProperty("thursday")]
        public int thursday { get; set; }

        [JsonProperty("friday")]
        public int friday { get; set; }

        [JsonProperty("saturday")]
        public int saturday { get; set; }

        [JsonProperty("sunday")]
        public int sunday { get; set; }

        [JsonProperty("start_date")]
        public string start_date { get; set; }

        [JsonProperty("end_date")]
        public string end_date { get; set; }
    }
     
    public class Trip
    {
        public string route_id { get; set; }
        public string service_id { get; set; }
        public string trip_id { get; set; }
        public string trip_headsign { get; set; }
        public string block_id { get; set; }
        public string shape_id { get; set; }
        public int direction_id { get; set; }
    }

    public class TripModels
    {
        public string GetServicePeriod()
        {
            string serviceID = string.Empty;

            List<ServicePeriod> lstRoutes = new List<ServicePeriod>();

            string servicePeriodJSON = j.Get_GTFS_Response(j.METRA_API_URL + "schedule/calendar");
            List<ServicePeriod> servicePeriodList = JsonConvert.DeserializeObject<List<ServicePeriod>>(servicePeriodJSON);
 
            ServicePeriod servicePeriod = servicePeriodList
                .Where(
                (x) =>
                (DateTime.Parse(x.start_date) <= DateTime.Today)
                && (DateTime.Parse(x.end_date) <= DateTime.Today)
                && ((GetPropValue(x, DateTime.Now.ToString("dddd").ToLower()) as int?).Equals(1))
                )
                .FirstOrDefault();

            serviceID = (servicePeriod == null) ? string.Empty : servicePeriod.service_id;

            return serviceID;
        }
         
        public List<List<StopOnTrip>> GetTrips(Direction direction, List<string> routesFilter)
        {
            //direction ID = 0: inbound; 1: outbound
            string serviceID = GetServicePeriod();

            List<List<StopOnTrip>> lstTrips = new List<List<StopOnTrip>>();

            if (string.IsNullOrEmpty(serviceID)) return lstTrips;
             
            string tripsJSON = j.Get_GTFS_Response(j.METRA_API_URL + "schedule/trips");
            List<Trip> tripsList = JsonConvert.DeserializeObject<List<Trip>>(tripsJSON);
             
            tripsList = tripsList
                .Where(
                (x) => 
                (x.service_id.Equals(serviceID))
                && (x.direction_id.Equals((int)direction))
                && (routesFilter.Any(b => x.route_id.Equals(b)))
                ).ToList();

            for (int i = 0; i < tripsList.Count; i++)
            {
                string stopTimesJSON = j.Get_GTFS_Response(j.METRA_API_URL + "schedule/stop_times/" + tripsList[i].trip_id);
                List<StopOnTrip> stopTimesList = JsonConvert.DeserializeObject<List<StopOnTrip>>(stopTimesJSON);

                //Need to solve problem with 24 hours (midnight), 25 hours (1am), etc. 
                if (
                    (!stopTimesList[0].arrival_time.Substring(0, 2).Equals("24"))
                    && (!stopTimesList[0].arrival_time.Substring(0, 2).Equals("25"))
                    && (!stopTimesList[0].arrival_time.Substring(0, 2).Equals("26"))
                    )
                {
                    if (DateTime.Parse(stopTimesList[0].arrival_time) >= default(DateTime).Add(DateTime.Now.TimeOfDay))
                    {
                        lstTrips.Add(stopTimesList);
                    }
                } 
                
            } 

            return lstTrips;
        }

        public static object GetPropValue(object src, string propName)
        {
            return src.GetType().GetProperty(propName).GetValue(src, null);
        }
    }
}
