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
            //Filter positions to contain only routes that stop at Western Avenue
            List<string> lstRoutesFilter = new List<string>();
            lstRoutesFilter.Add("MD-N");
            lstRoutesFilter.Add("MD-W");
            lstRoutesFilter.Add("NCS");
             
            TripModels _tm = new TripModels();

            IEnumerable<StopOnTrip> schedules = _tm.GetScheduledTimes("CUS", Direction.Outbound, lstRoutesFilter);



            //var trips1 = _tm.GetScheduledTimes("WESTERNAVE", Direction.Inbound, lstRoutesFilter);

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
}
