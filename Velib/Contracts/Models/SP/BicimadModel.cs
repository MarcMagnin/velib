using Newtonsoft.Json;
using System.Collections.Generic;

namespace Velib.Contracts.Models.SP
{
    public class BicimadModel
    {
        [JsonProperty(PropertyName = "estaciones")]
        public List<BicimadStation> Stations
        {
            get; set;
        }
    }

    public class BicimadStation
    {
        [JsonProperty(PropertyName = "bicis_enganchadas")]
        public int AvailableBikes { get; set; }

        [JsonProperty(PropertyName = "bases_libres")]
        public int? AvailableBikeStands { get; set; }

        public bool Banking { get; set; }

        [JsonProperty(PropertyName = "latitud")]
        public double Latitude { get; set; }

        [JsonProperty(PropertyName = "longitud")]
        public double Longitude { get; set; }

        [JsonProperty(PropertyName = "nombre")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "activo")]
        public string innerStatus { get; set; }

        [JsonIgnore]
        public bool Status { get; set; }
    }
}
