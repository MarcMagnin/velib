using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Velib.Contracts.Models.BCycle
{
 [DataContract]
    public class PhiladelphiaTempModel
    {
        [DataMember(Name = "features")]
        public List<Feature> Features { get; set; }
        
    }

 [DataContract]
 public class Feature
 {

     [DataMember(Name = "geometry")]
     public Geometry Geometry { get; set; }

     [DataMember(Name = "id")]
     public int Id { get; set; }

     [DataMember(Name = "properties")]
     public Properties Properties { get; set; }

 }
 [DataContract]
 public class Properties
 {
     [DataMember(Name = "location_name")]
     public string LocationName{ get; set; }
 }
 [DataContract]
 public class Geometry
 {
     [DataMember(Name = "coordinates")]
     public double[] Location{ get; set; }

 }

}

