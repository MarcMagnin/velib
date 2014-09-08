using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Velib.Contracts.Models.CH.PubliBike
{
    [DataContract]
    public class PubliBikeModel
    {
        //[DataMember(Name = "groups")]
        //public List<Group> Groups { get; set; }
        [DataMember(Name = "terminals")]
        public List<Station> Stations { get; set; }

    }
    //[DataContract]
    //public class Group
    //{

    //    [DataMember(Name = "name")]
    //    public string name { get; set; }
    //}


    [DataContract]
    public class Bike
    {

        [DataMember(Name = "available")]
        public int Available { get; set; }
    }


    [DataContract]
    public class BikeHolder
    {

        [DataMember(Name = "holdersfree")]
        public int HoldersFree { get; set; }
    }
  
    [DataContract]
    public class Station
    {

        [DataMember(Name = "terminalid")]
        public int Id { get; set; }

        [DataMember(Name = "status")]
        public string Status { get; set; }

        [DataMember(Name = "lat")]
        public double Latitude { get; set; }

        [DataMember(Name = "lng")]
        public double Longitude { get; set; }

        [DataMember(Name = "name")]
        public string Label { get; set; }

        [DataMember(Name = "street")]
        public string StationAddress { get; set; }

        [DataMember(Name = "bikes")]
        public List<Bike> AvailableBikes { get; set; }

        [DataMember(Name = "bikeholders")]
        public List<BikeHolder> AvailableDocks { get; set; }

    }

}


