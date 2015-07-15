using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Velib.Contracts.Models.China
{
    public class HuiminOperateModel
    {
        [JsonProperty("lat")]
        public double Latitude { get; set; }

        [JsonProperty("lng")]
        public double Longitude { get; set; }

        [JsonProperty("DQCSZ")]
        public int AvailableBikes { get; set; }

        [JsonProperty("kzcs")]
        public int AvailableSlots { get; set; }
    }
}
