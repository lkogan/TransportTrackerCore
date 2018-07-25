using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using TransportTrackerCore.Models;

namespace TransportTrackerCore.Controllers
{
    [Route("api/[controller]")]
    public class MetraController : Controller
    { 
        [HttpGet("GetRoutesData")]
        public IEnumerable<RoutesData> GetRoutesData()
        { 
            //Filter positions to contain only routes that stop at Western Avenue
            List<string> lstRoutesFilter = new List<string>();
            lstRoutesFilter.Add("MD-N");
            lstRoutesFilter.Add("MD-W");
            lstRoutesFilter.Add("NCS");

            TripModels _tm = new TripModels();
            var trips = _tm.GetTrips(Direction.Outbound, lstRoutesFilter);
             


            WesternAvenueModels _model = new WesternAvenueModels();

            IEnumerable<RoutesData> routes = _model.GetRoutesOnTheWayToWesternAvenue();

            return routes;
        } 
    }
}
