using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Velib.Contracts.Models.Bixi
{
    [DataContract]
    public class BixxiModel
    {

        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "locked")]
        public string Status { get; set; }

        [DataMember(Name = "lat")]
        public double Latitude { get; set; }

        [DataMember(Name = "long")]
        public double Longitude { get; set; }

        [DataMember(Name = "name")]
        public string Label { get; set; }

        [DataMember(Name = "nbBikes")]
        public int AvailableBikes { get; set; }

        [DataMember(Name = "nbEmptyDocks")]
        public int AvailableDocks { get; set; }

    }

}



