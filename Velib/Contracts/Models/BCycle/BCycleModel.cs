using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Velib.Contracts.Models.BCycle
{
    [DataContract]
    public class BCycleModel
    {
        [DataMember(Name = "Id")]
        public int Id { get; set; }

        [DataMember(Name = "Status")]
        public string Status { get; set; }

        
        [DataMember(Name = "Name")]
        public string Label { get; set; }

       // [DataMember(Name = "")]
        //public string StationAddress { get; set; }

        [DataMember(Name = "BikesAvailable")]
        public int AvailableBikes { get; set; }

        [DataMember(Name = "DocksAvailable")]
        public int AvailableDocks { get; set; }


        [DataMember(Name = "Location")]
        public Location Location { get; set; }
    }


    [DataContract]
    public class Location
    {
        [DataMember(Name = "Latitude")]
        public double Latitude { get; set; }

        [DataMember(Name = "Longitude")]
        public double Longitude { get; set; }

    }

}

