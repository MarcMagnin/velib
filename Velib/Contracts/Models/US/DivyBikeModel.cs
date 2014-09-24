using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Velib.Contracts.Models.US
{
    [DataContract]
    public class DivyBikeModel
    {

        [DataMember(Name = "stationBeanList")]
        public Station[] Stations { get; set; }

    }

    [DataContract]
    public class Station
    {

        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "statusKey")]
        public string Status { get; set; }

        [DataMember(Name = "latitude")]
        public double Latitude { get; set; }

        [DataMember(Name = "longitude")]
        public double Longitude { get; set; }

        [DataMember(Name = "stationName")]
        public string Label { get; set; }

        [DataMember(Name = "stAddress1")]
        public string StationAddress { get; set; }

        [DataMember(Name = "availableBikes")]
        public int AvailableBikes { get; set; }

        [DataMember(Name = "availableDocks")]
        public int AvailableDocks { get; set; }

    }

}


