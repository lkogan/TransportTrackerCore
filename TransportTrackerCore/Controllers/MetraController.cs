using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using TransportTrackerCore.Models;
using static TransportTrackerCore.Models.AuxModels;
using j = TransportTrackerCore.Models.JSON_Models;

namespace TransportTrackerCore.Controllers
{
    [Route("api/[controller]")]
    public class MetraController : Controller
    { 
        [HttpGet("GetRoutesData")]
        public IEnumerable<RoutesData> GetRoutesData()
        {  
            WesternAvenueModels _model = new WesternAvenueModels();

            IEnumerable<RoutesData> routes = _model.GetRoutesOnTheWayToWesternAvenue();

            return routes;
        }

        [HttpGet("GetScheduleData")]
        public IEnumerable<StopOnTrip> GetScheduleData()
        {
            TripModels _tm = new TripModels();
            
            IEnumerable<StopOnTrip> schedules = _tm.GetScheduledTimes("WESTERNAVE", "CUS", Direction.Inbound);

            return schedules;
        }


        [HttpGet("GetStations")]
        public IEnumerable<StationObject> GetStations()
        {
            //Create dictionary to resolve station abbrevs to names
            string stationsJSON = j.Get_GTFS_Response(j.METRA_API_URL + "schedule/stops");
            List<Station> stationsList = JsonConvert.DeserializeObject<List<Station>>(stationsJSON);

            stationsList = stationsList.OrderBy(x => x.stop_name).ToList();

            List<StationObject> lst = new List<StationObject>();

            foreach (Station s in stationsList)
            {
                if (s.stop_name.Equals("Western Ave"))
                {
                    s.stop_name = "Western Ave/18th Place (BNSF)";
                }

                lst.Add(new StationObject { value = s.stop_id, label = s.stop_name });
            }
            return lst;
        }

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
