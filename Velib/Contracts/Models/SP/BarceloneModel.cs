using System.Runtime.Serialization;

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

