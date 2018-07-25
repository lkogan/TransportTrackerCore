using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using j = TransportTrackerCore.Models.JSON_Models;

namespace TransportTrackerCore.Models
{
    public class ServicePeriod
    {
        [JsonProperty("service_id")]
        public string service_id { get; set; }

        [JsonProperty("monday")]
        public int monday { get; set; }

        [JsonProperty("tuesday")]
        public int tuesday { get; set; }

        [JsonProperty("wednesday")]
        public int wednesday { get; set; }

        [JsonProperty("thursday")]
        public int thursday { get; set; }

        [JsonProperty("friday")]
        public int friday { get; set; }

        [JsonProperty("saturday")]
        public int saturday { get; set; }

        [JsonProperty("sunday")]
        public int sunday { get; set; }

        [JsonProperty("start_date")]
        public string start_date { get; set; }

        [JsonProperty("end_date")]
        public string end_date { get; set; }
    }

    public class ServicePeriodModels
    {
        public string GetServicePeriod()
        {
            string serviceID = string.Empty;

            List<ServicePeriod> lstRoutes = new List<ServicePeriod>();

            string servicePeriodJSON = j.Get_GTFS_Response(j.METRA_API_URL + "schedule/calendar");
            List<ServicePeriod> servicePeriodList = JsonConvert.DeserializeObject<List<ServicePeriod>>(servicePeriodJSON);
 
            ServicePeriod servicePeriod = servicePeriodList
                .Where(
                (x) =>
                (DateTime.Parse(x.start_date) <= DateTime.Today)
                && (DateTime.Parse(x.end_date) <= DateTime.Today)
                && ((GetPropValue(x, DateTime.Now.ToString("dddd").ToLower()) as int?).Equals(1))
                )
                .FirstOrDefault();

            serviceID = (servicePeriod == null) ? string.Empty : servicePeriod.service_id;

            return serviceID;
            }

        public static object GetPropValue(object src, string propName)
        {
            return src.GetType().GetProperty(propName).GetValue(src, null);
        }
    }
}
