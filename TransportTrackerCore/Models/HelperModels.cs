using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static TransportTrackerCore.Models.AuxModels;
using j = TransportTrackerCore.Models.JSON_Models;

namespace TransportTrackerCore.Models
{
    public class HelperModels
    {
        public static List<ServicePeriod> ServicePeriodList;

        public static List<StopOnTrip> StopList;

        public static List<StationObject> StationsList;

        public static List<Trip> TripsList;


        public static void LoadInitialData()
        { 
            ServicePeriodList = GetServicePeriod();
             
            StopList = GetStopTimes();

            StationsList = GetStations();

            TripsList = GetTrips();
        }

        public static List<ServicePeriod> GetServicePeriod()
        {
            if (ServicePeriodList != null) return ServicePeriodList;

            string servicePeriodJSON = j.Get_GTFS_Response(j.METRA_API_URL + "schedule/calendar");
            ServicePeriodList = JsonConvert.DeserializeObject<List<ServicePeriod>>(servicePeriodJSON);

            return ServicePeriodList;
        }

        public static List<StopOnTrip> GetStopTimes()
        {
            if (StopList != null) return StopList;

            string stopTimesJSON = j.Get_GTFS_Response(j.METRA_API_URL + "schedule/stop_times");
            StopList = JsonConvert.DeserializeObject<List<StopOnTrip>>(stopTimesJSON);

            return StopList;
        }

        public static List<Trip> GetTrips()
        {
            if (TripsList != null) return TripsList;

            string tripsJSON = j.Get_GTFS_Response(j.METRA_API_URL + "schedule/trips");
            TripsList = JsonConvert.DeserializeObject<List<Trip>>(tripsJSON);

            return TripsList;
        }

        public static List<StationObject> GetStations()
        {
            if (StationsList != null) return StationsList;

            //Create dictionary to resolve station abbrevs to names
            string stationsJSON = j.Get_GTFS_Response(j.METRA_API_URL + "schedule/stops");
            List<Station> stationsList = JsonConvert.DeserializeObject<List<Station>>(stationsJSON);

            stationsList = stationsList.OrderBy(x => x.stop_name).ToList();

            List<StationObject> lst = new List<StationObject>();

            foreach (Station s in stationsList)
            {
                if (s.stop_id.Equals("BNWESTERN"))
                {
                    s.stop_name = "Western Ave/18th Place (BNSF)";
                }

                lst.Add(new StationObject { value = s.stop_id, label = s.stop_name });
            }

            StationsList = lst;

            return lst;
        }

        public static List<string> GetLinesFromStationList(List<string> listOfLines)
        {
            List<string> routes = new List<string>();

            foreach (var x in listOfLines)
            {
                string route = x.Substring(0, x.IndexOf('_'));

                if (!routes.Contains(route))
                {
                    routes.Add(route);
                }
            }

            return routes;
        }

        public static List<StationObject> GetLinesFromStation(string StationAbbrev, int direction)
        {
            List<StationObject> filteredStations = (StationsList == null) ? GetStations() : StationsList;

            List<Trip> filteredTrips = (TripsList == null) ? GetTrips() : TripsList;

            List<ServicePeriod> filteredServicePeriods = (ServicePeriodList == null) ? GetServicePeriod() : ServicePeriodList;
             
            List<StopOnTrip> filteredStopsOnTrips = (StopList == null) ? GetStopTimes() : StopList; //slow (110K entries!). 

            var filteredStopsOnTrips1 = filteredStopsOnTrips.Where(x => x.stop_id.Equals(StationAbbrev)).ToList();

            var tripsFromStation = filteredStopsOnTrips1.Select(x => x.trip_id).ToList();

            tripsFromStation = tripsFromStation.Distinct().ToList();

            var routes = GetLinesFromStationList(filteredStopsOnTrips1.Select(x => x.trip_id).ToList());



            string serviceID = TripModels.GetServicePeriod();
              
            filteredTrips = filteredTrips
                .Where(
                (x) =>
                (x.service_id.Equals(serviceID))
                && (x.direction_id.Equals((int)direction))
                && (routes.Any(b => x.route_id.Equals(b)))
                ).ToList();

            var filteredTripIDs = filteredTrips.Select(x => x.trip_id).ToList();

            var filteredStationAbbrevs = filteredStopsOnTrips
                .Where(
                (x) =>
                (filteredTripIDs.Any(b => x.trip_id.Equals(b)))
                && (!x.stop_id.Equals(StationAbbrev))
                ).Select(x => x.stop_id).Distinct().ToList();

            List<StationObject> result = new List<StationObject>();

            for (int i = 0; i < filteredStationAbbrevs.Count; i++)
            {
                result.Add(filteredStations.FirstOrDefault(x => x.value.Equals(filteredStationAbbrevs[i])));
            }

            result = result.OrderBy(x => x.label).ToList();

            return result;

            //Remaining:
            //1) Filter out stop_sequence greater than source station (for inbounds), or less than (for outbounds).
            //Keep in mind that stop_sequence will be different on each line

            //Speed up
        }

        public class StationObject
        {
            public string value { get; set; }
            public string label { get; set; }
        }

        public class TrainArrival
        {
            public string origin_departure_time { get; set; }
            public string origin_name { get; set; }
            public string dest_arrival_time { get; set; }
            public string arrives_in_min { get; set; }
            public string dest_name { get; set; }
            public string description { get; set; }






            //public string route_id { get; set; }
            //public string service_id { get; set; }
            //public string trip_id { get; set; }
            //public string trip_headsign { get; set; }

            //public int direction_id { get; set; }



            //public string trip_id { get; set; }
            //public string arrival_time { get; set; }
            //public string departure_time { get; set; }
            //public string stop_id { get; set; } 
        }
    }
}
