using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Velib.Contracts.Models.PL
{
   [DataContract]
    public class SzczecinModel
    {

        [DataMember(Name = "map")]
        public Station[] Stations { get; set; }

    }

    [DataContract]
    public class Station
    {

        [DataMember(Name = "Latitude")]
        public double Latitude { get; set; }

        [DataMember(Name = "Longitude")]
        public double Longitude { get; set; }

        [DataMember(Name = "AvailableBikesCount")]
        public int AvailableBikes { get; set; }

        [DataMember(Name = "FreeLocksCount")]
        public int AvailableDocks { get; set; }

    }

}


