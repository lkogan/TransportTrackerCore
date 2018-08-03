﻿using Microsoft.AspNetCore.Mvc;
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
        [HttpGet("LoadInitialData")]
        public void LoadInitialData()
        {
            HelperModels.LoadInitialData();
        }

        [HttpGet("GetRoutesData")]
        public IEnumerable<RoutesData> GetRoutesData()
        {  
            WesternAvenueModels _model = new WesternAvenueModels();

            IEnumerable<RoutesData> routes = _model.GetRoutesOnTheWayToWesternAvenue();

            return routes;
        }

        [HttpGet("GetScheduleData")]
        public IEnumerable<StopOnTrip> GetScheduleData(string FromStationAbbrev, string ToStationAbbrev, int Direction)
        { 
            TripModels _tm = new TripModels();
            
            IEnumerable<StopOnTrip> schedules = _tm.GetScheduledTimes(FromStationAbbrev, ToStationAbbrev, (Direction)Direction);

            return schedules;
        }
         

        [HttpGet("GetStations")]
        public IEnumerable<StationObject> GetStations()
        {
            IEnumerable<StationObject> lst = (StationsList == null) ? HelperModels.GetStations() : StationsList;

            return lst;
        }

        [HttpGet("GetAccessibleStations")]
        public IEnumerable<StationObject> GetAccessibleStations(string StationAbbrev, bool IsOutbound)
        {
            int direction = (IsOutbound == true) ? 1 : 0;
            List<StationObject> lst = HelperModels.GetLinesFromStation(StationAbbrev, direction);

            return lst;
        }

    }
}
