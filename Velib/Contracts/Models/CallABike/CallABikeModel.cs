using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Velib.Contracts.Models.CallABike
{
    [DataContract]
    public class CallABikeModel
    {

        [DataMember(Name = "marker")]
        public Station[] stations { get; set; }

    }

    [DataContract]
    public class Station
    {
    

        [DataMember(Name = "lat")]
        public double Latitude { get; set; }

        [DataMember(Name = "lng")]
        public double Longitude { get; set; }

        [DataMember(Name = "hal2option")]
        public Details Details { get; set; }


    }

    [DataContract]
    public class Details
    {
        [DataMember(Name = "bikelist")]
        public string[] Bikelist { get; set; }

    }
}


