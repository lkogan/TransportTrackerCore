using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using static TransportTrackerCore.Models.AuxModels;
using j = TransportTrackerCore.Models.JSON_Models;
using h = TransportTrackerCore.Models.HelperModels;
using System.Collections.Concurrent;
using static TransportTrackerCore.Models.HelperModels;
using static TransportTrackerCore.Models.TripUpdateModels;

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
             
            List<ServicePeriod> servicePeriodList = (h.ServicePeriodList == null) ? h.GetServicePeriodList() : h.ServicePeriodList;

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

        public static string EmptyIfNull(string value)
        {
            return string.IsNullOrEmpty(value) ? string.Empty : value;
        }

        public List<TrainArrival> GetScheduledTimes(string FromStationID, string ToStationID, Direction direction)
        { 
            List<TrainArrival> lstTrainArrivals = new List<TrainArrival>();

            List<StopOnTrip> stopsList = (h.StopList == null) ? h.GetStopTimes() : h.StopList;
             
            //Get Metra routes that are running today, in the direction specified
            //direction ID = 0: inbound; 1: outbound
            string serviceID = GetServicePeriod();

            if (string.IsNullOrEmpty(serviceID)) return lstTrainArrivals;
             
            var fromStopAlertData = h.AlertsList.Where(x => EmptyIfNull(x.alert.informed_entity[0].stop_id).Equals(FromStationID)).FirstOrDefault();

            if (fromStopAlertData != null)
            {
                string alertText = fromStopAlertData.alert.header_text.translation[0].text;

                string alertMemo = fromStopAlertData.alert.description_text.translation[0].text;
            }


            var toStopAlertData = h.AlertsList.Where(x => EmptyIfNull(x.alert.informed_entity[0].stop_id).Equals(ToStationID)).FirstOrDefault();

            if (toStopAlertData != null)
            {
                string alertText = toStopAlertData.alert.header_text.translation[0].text;

                string alertMemo = toStopAlertData.alert.description_text.translation[0].text;
            }

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
                || (x.stop_id.Equals(ToStationID))
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
            /**/

            tripsList = tripsList.Where(x => matches.Contains(x.trip_id)).ToList();

            for (int i = 0; i < tripsList.Count; i++)
            {
                string tripID = tripsList[i].trip_id;

                string Delay = string.Empty;

                TripUpdateCollection tuc = h.TripUpdatesList.Where(x => x.id.Equals(tripID.Replace("_V1_A", "_V1_B"))).FirstOrDefault();
                if (tuc != null)
                {
                    //string lastStationAbbr = tuc.trip_update.stop_time_update[0].stop_id;
                    //if (lastStationAbbr.Equals("CUS")) continue;

                    int delayInSeconds = tuc.trip_update.stop_time_update[0].arrival.delay;
                     
                    TimeSpan ts = new TimeSpan(0, 0, 0);

                    if (delayInSeconds > 0)
                    {
                        ts = TimeSpan.FromSeconds(delayInSeconds);
                        Delay = (int)ts.TotalMinutes + " min late" + Environment.NewLine;
                    }
                } 

                var currentStops = stopsList.Where(x => x.trip_id.Equals(tripsList[i].trip_id)).ToList();

                var tripAlertData = h.AlertsList.Where(x => EmptyIfNull(x.alert.informed_entity[0].trip).Equals(tripID)).FirstOrDefault();

                string alertText = "On Time";
                string alertMemo = string.Empty;

                if (tripAlertData != null)
                {
                    alertText = tripAlertData.alert.header_text.translation[0].text;

                    alertMemo = tripAlertData.alert.description_text.translation[0].text;
                }

                if (!string.IsNullOrEmpty(Delay) && alertText.Equals("On Time"))
                {
                    alertText = Delay;
                }

                if (currentStops.Count == 2)
                {
                    string origin_departure_time = currentStops[0].departure_time;

                    string dest_arrival_time = currentStops[1].arrival_time;

                    TrainArrival tr = new TrainArrival
                    {
                        origin_departure_time = origin_departure_time.Remove(origin_departure_time.Length - 3),

                        origin_name = StationsList.Where(x => x.value.Equals(currentStops[0].stop_id)).First().label,

                        dest_arrival_time = dest_arrival_time.Remove(dest_arrival_time.Length - 3),

                        dest_name = StationsList.Where(x => x.value.Equals(currentStops[1].stop_id)).First().label,

                        arrives_in_min = "",

                        time_on_train = "",

                        late_by_min = "",

                        alert = alertText,

                        route_id = tripsList[i].route_id,

                        trip_id = tripsList[i].trip_id,

                        tripURL = "https://metrarail.com/maps-schedules/train-lines/" + tripsList[i].route_id +
                    "/trip/" + tripsList[i].trip_id,
                        
                        description = "",
                    };

                    lstTrainArrivals.Add(tr);
                }

                lstTrainArrivals = lstTrainArrivals.OrderBy(x => x.origin_departure_time).ToList();
            } 

            return lstTrainArrivals;
        }

        public static object GetPropValue(object src, string propName)
        {
            return src.GetType().GetProperty(propName).GetValue(src, null);
        }
    }
}
