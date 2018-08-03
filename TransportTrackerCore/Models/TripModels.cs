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
            List<StopOnTrip> stopsList = (h.StopList == null) ? h.GetStopTimes() : h.StopList;
             
            var fromStationList = stopsList.Where(x => x.stop_id.Equals(FromStationID));

            var toStationList = stopsList.Where(x => x.stop_id.Equals(ToStationID));

            var matches = fromStationList.Select(a => a.trip_id).Intersect(toStationList.Select(b => b.trip_id)).ToList();
             
            List<string> routes = h.GetLinesFromStationList(matches);
             
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
                 && (routes.Any(b => x.route_id.Equals(b)))
                ).ToList();

            matches = tripsList.Select(x => x.trip_id).ToList();
             
            stopsList = stopsList
                .Where(
                (x) =>
                ((x.stop_id.Equals(FromStationID))
                //|| (x.stop_id.Equals(ToStationID))
                )
                && (matches.Contains(x.trip_id))
                ).ToList();

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
