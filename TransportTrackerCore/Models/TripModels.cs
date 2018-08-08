using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using static TransportTrackerCore.Models.AlertModels;
using static TransportTrackerCore.Models.AuxModels;
using j = TransportTrackerCore.Models.JSON_Models;
using h = TransportTrackerCore.Models.HelperModels;
using System.Collections.Concurrent;

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
        public static string GetServicePeriod()
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

        public List<StopOnTrip> GetScheduledTimes(string FromStationID, string ToStationID, Direction direction)
        { 
            //h.GetListAsync().ContinueWith(h.OnMyAsyncMethodFailed, TaskContinuationOptions.OnlyOnFaulted);

            List<StopOnTrip> stopsList = (h.StopList == null) ? h.GetStopTimes() : h.StopList;
             
            //Get Metra routes that are running today, in the direction specified
            //direction ID = 0: inbound; 1: outbound
            string serviceID = GetServicePeriod();

            if (string.IsNullOrEmpty(serviceID)) return stopsList;

            List<Trip> tripsList = (h.TripsList == null) ? h.GetTrips() : h.TripsList;

            //Get all inbound/outbound routes running for a specific day
            tripsList = tripsList
                .Where(
                (x) =>
                (x.service_id.Equals(serviceID))
                && (x.direction_id.Equals((int)direction))
                ).ToList();

             
            var results = new ConcurrentBag<string>();

            Parallel.For(0, tripsList.Count, i =>
            { 
                var myroute = tripsList[i];

                var fromFoute = stopsList.FirstOrDefault(x => x.stop_id.Equals(FromStationID) && x.trip_id.Equals(myroute.trip_id));

                var toRoute = stopsList.FirstOrDefault(x => x.stop_id.Equals(ToStationID) && x.trip_id.Equals(myroute.trip_id));

                if ((fromFoute != null) && (toRoute != null))
                {
                    results.Add(myroute.trip_id);
                } 
            });

            var matches = results.ToList().Distinct().ToList();

            stopsList = stopsList
                .Where(
                (x) =>
                ((x.stop_id.Equals(FromStationID))
                //|| (x.stop_id.Equals(ToStationID))
                )
                && (matches.Contains(x.trip_id))
                ).ToList();

            //Remove routes that are already in the past
            for (int i = stopsList.Count - 1; i >= 0; i--)
            {
                int hour = int.Parse(stopsList[i].arrival_time.Split(':')[0]);
                int minute = int.Parse(stopsList[i].arrival_time.Split(':')[1]);

                int currentHour = DateTime.Now.TimeOfDay.Hours;
                int currentMinute = DateTime.Now.TimeOfDay.Minutes;

                if (hour.CompareTo(currentHour) < 0)
                {
                    stopsList.RemoveAt(i);
                }
                else if (hour.CompareTo(currentHour) == 0)
                {
                    if (minute.CompareTo(currentMinute) < 0)
                    {
                        stopsList.RemoveAt(i);
                    }
                }
            }

            stopsList = stopsList.OrderBy(x => x.arrival_time).ToList();

            tripsList = tripsList.Where(x => matches.Contains(x.trip_id)).ToList();
            
            return stopsList;
        }

        public static object GetPropValue(object src, string propName)
        {
            return src.GetType().GetProperty(propName).GetValue(src, null);
        }
    }
}
