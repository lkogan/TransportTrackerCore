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

        [HttpGet("GetStations")]
        public IEnumerable<StationObject> GetStations()
        {
            IEnumerable<StationObject> lst = (StationsList == null) ? HelperModels.GetStations() : StationsList;

            return lst;
        }

        [HttpGet("GetAccessibleStations")]
        public IEnumerable<StationObject> GetAccessibleStations(string StationAbbrev, int IsOutbound)
        {
            Direction direction = (IsOutbound.Equals(1)) ? Direction.Outbound : Direction.Inbound;

            List<StationObject> lst = HelperModels.GetLinesFromStation(StationAbbrev, direction);

            return lst;
        }

        [HttpGet("GetScheduleData")]
        public IEnumerable<TrainArrival> GetScheduleData(string FromStationAbbrev, string ToStationAbbrev, int Direction)
        {
            TripModels _tm = new TripModels();

            Direction direction = (Direction)Direction;

            IEnumerable<TrainArrival> schedules = _tm.GetScheduledTimes(FromStationAbbrev, ToStationAbbrev, direction);

            return schedules;
        }
    }
}
