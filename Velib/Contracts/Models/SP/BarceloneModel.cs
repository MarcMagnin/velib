using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Velib.Contracts.Models.SP
{
    [DataContract]
    public class BarceloneModel
    {
        [DataMember(Name = "id")]
        public string id { get; set; }

        [DataMember(Name = "bikes")]
        public int AvailableBikes { get; set; }

        [DataMember(Name = "slots")]
        public int AvailableDocks { get; set; }

        [DataMember(Name = "lat")]
        public double Latitude { get; set; }

        [DataMember(Name = "lon")]
        public double Longitude { get; set; }
    }
}

