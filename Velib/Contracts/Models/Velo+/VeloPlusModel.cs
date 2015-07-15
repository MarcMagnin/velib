using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Velib.Contracts.Models.Velo
{
    [DataContract]
    public class VeloPlusModel
    {
        [DataMember(Name = "libelle")]
        public string Libelle { get; set; }

        [DataMember(Name = "gps")]
        public GPS GPS { get; set; }

        [DataMember(Name = "nb_bike")]
        public int AvailableBikes { get; set; }

        [DataMember(Name = "nb_spot")]
        public int AvailableDocks { get; set; }
    }

    [DataContract]
    public class GPS
    { 
        [DataMember(Name = "lat")]
        public double Latitude { get; set; }
        [DataMember(Name = "lng")]
        public double Longitude { get; set; }
    }
}


