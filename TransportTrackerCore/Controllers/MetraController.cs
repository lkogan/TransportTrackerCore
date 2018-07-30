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
using static TransportTrackerCore.Models.HelperModels;

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
            List<StationObject> lst = HelperModels.GetStations();
             
            return lst;
        }
    } 
}
