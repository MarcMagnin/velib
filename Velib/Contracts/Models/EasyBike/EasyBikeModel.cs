using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Velib.Contracts.Models.EasyBike
{
    [DataContract]
    public class EasyBikeModel
    {
        [DataMember(Name = "lat")]
        public double Latitude { get; set; }

        [DataMember(Name = "lng")]
        public double Longitude { get; set; }

        [DataMember(Name = "data")]
        public Data Data { get; set; }

        public int AvailableBikes { get; set; }

        public int AvailableDocks { get; set; }

    }
    [DataContract]
    public class Data
    { 
         [DataMember(Name = "desc")]
           public string Desc{ get; set; }
        [DataMember(Name = "text")]
         public string Text{ get; set; }

    }
}


