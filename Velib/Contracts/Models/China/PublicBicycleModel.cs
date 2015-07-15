using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Velib.Contracts.Models.China
{
    [DataContract]
    public class PublicBicycleModel
    {
        [DataMember(Name = "lat")]
        public double Latitude { get; set; }

        [DataMember(Name = "lng")]
        public double Longitude { get; set; }

        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "capacity")]
        public int Capacity { get; set; }

        [DataMember(Name = "availBike")]
        public int AvailableBikes { get; set; }
    }
}
