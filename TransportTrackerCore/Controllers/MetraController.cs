using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using TransportTrackerCore.Models;
using static TransportTrackerCore.Models.AuxModels;

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
    }
}
