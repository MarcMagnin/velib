using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Velib.Contracts.Models.US
{
    [DataContract]
    public class CitiBikeModel
    {

        [DataMember(Name = "ok")]
        public bool Ok { get; set; }

        [DataMember(Name = "meta")]
        public object[] Meta { get; set; }

        [DataMember(Name = "results")]
        public Result[] Results { get; set; }

        [DataMember(Name = "activeStations")]
        public int ActiveStations { get; set; }

        [DataMember(Name = "totalStations")]
        public int TotalStations { get; set; }

        [DataMember(Name = "lastUpdate")]
        public int LastUpdate { get; set; }
    }


    [DataContract]
          public class NearbyStation
    {

        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "distance")]
        public double Distance { get; set; }
    }

    [DataContract]
    public class Result
    {

        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "status")]
        public string Status { get; set; }

        [DataMember(Name = "latitude")]
        public double Latitude { get; set; }

        [DataMember(Name = "longitude")]
        public double Longitude { get; set; }

        [DataMember(Name = "label")]
        public string Label { get; set; }

        [DataMember(Name = "stationAddress")]
        public string StationAddress { get; set; }

        [DataMember(Name = "availableBikes")]
        public int AvailableBikes { get; set; }

        [DataMember(Name = "availableDocks")]
        public int AvailableDocks { get; set; }

        [DataMember(Name = "nearbyStations")]
        public NearbyStation[] NearbyStations { get; set; }
    }

}


