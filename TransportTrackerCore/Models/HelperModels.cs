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
        public static List<StationObject> GetStations()
        {
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

        public void GetLinesFromStation(string StationAbbrev, Direction direction)
        {
            //Get full list from https://gtfsapi.metrarail.com/gtfs/schedule/stop_times, get trip_id's as list
            // 
            //Go to trips at https://gtfsapi.metrarail.com/gtfs/schedule/trips

            //Filter out:
            //// Service ID out of range
            //// Direction out of range
            //// Return List of Route_id's and trip'id's

            //Go back to stop_time list
            //// Filter out:
            //// trip_Id's not from list of params
            //// Stop_sequence greater, or less (depends on direction)
            //// Return list of stations
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
