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

            //TO DO: speed up - slow (110K entries!). 
            List<StopOnTrip> stopsOnTrips = (StopList == null) ? GetStopTimes() : StopList;

            //Get trips that pass through the station (IB/OB)
            var filteredStopsOnTrips1 = stopsOnTrips.Where(x => x.stop_id.Equals(StationAbbrev)).ToList();

            var tripsFromStation = filteredStopsOnTrips1.Select(x => x.trip_id).ToList();

            tripsFromStation = tripsFromStation.Distinct().ToList();

            //Get routes (MD-W, MD-N, NCS, etc).
            var routes = GetLinesFromStationList(filteredStopsOnTrips1.Select(x => x.trip_id).ToList());


            string serviceID = TripModels.GetServicePeriod();

            //Get trips passing through the station, in the direction desired, along the routes needed (replace with trips?)
            filteredTrips = filteredTrips
                .Where(
                (x) =>
                (x.service_id.Equals(serviceID))
                && (x.direction_id.Equals((int)direction))
                && (routes.Any(b => x.route_id.Equals(b)))
                ).ToList();
            
            var filteredTripIDs = filteredTrips.Select(x => x.trip_id).ToList();

            List<string> allStationsRemaining = new List<string>();

            //For each of trips, get stations from the origin station, in the direction specified, and add to list
            for (int i = 0; i < filteredTripIDs.Count; i++)
            {
                string tripID = filteredTripIDs[i];

                var stopsPerTrip = stopsOnTrips.Where(x => x.trip_id.Equals(tripID));

                var myOriginStation = stopsPerTrip.FirstOrDefault(x => x.stop_id.Equals(StationAbbrev));

                if (myOriginStation == null) continue; //Train is passing without stops through the station

                var sequenceID = myOriginStation.stop_sequence;

                IEnumerable<StopOnTrip> stopsOnTrip = new List<StopOnTrip>();

                if (direction.Equals((int)Direction.Inbound))
                {
                    stopsOnTrip = stopsPerTrip.Where(x => x.stop_sequence > sequenceID);
                }
                else if (direction.Equals((int)Direction.Outbound))
                {
                    stopsOnTrip = stopsPerTrip.Where(x => x.stop_sequence > sequenceID);
                }

                List<string> stationsRemaining = stopsOnTrip.Select(x => x.stop_id).ToList();

                allStationsRemaining.AddRange(stationsRemaining);
            }

            //Remove duplicates
            allStationsRemaining = allStationsRemaining.Distinct().ToList();

            List<StationObject> result = new List<StationObject>();

            for (int i = 0; i < allStationsRemaining.Count; i++)
            {
                result.Add(filteredStations.FirstOrDefault(x => x.value.Equals(allStationsRemaining[i])));
            }
             
            result = result.OrderBy(x => x.label).ToList();

            return result; 
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
