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
            WesternAvenueModels _model = new WesternAvenueModels();

            IEnumerable<RoutesData> routes = _model.GetRoutesOnTheWayToWesternAvenue();

            ServicePeriodModels _sdm = new ServicePeriodModels();
            string serviceID = _sdm.GetServicePeriod();

            return routes;
        } 
    }
}
