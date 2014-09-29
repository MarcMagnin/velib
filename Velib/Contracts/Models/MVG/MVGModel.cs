using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Velib.Contracts.Models.MVG
{
    [DataContract]
    public class MVGModel
    {
       
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "status")]
        public string Status { get; set; }

        [DataMember(Name = "latitude")]
        public double Latitude { get; set; }

        [DataMember(Name = "longitude")]
        public double Longitude { get; set; }

        [DataMember(Name = "name")]
        public string Label { get; set; }

        [DataMember(Name = "blocked")]
        public bool Locked { get; set; }

        [DataMember(Name = "bikes_available")]
        public int AvailableBikes { get; set; }

        [DataMember(Name = "docks_available")]
        public int AvailableDocks { get; set; }

    }

}


