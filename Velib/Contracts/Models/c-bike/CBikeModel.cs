using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Velib.Contracts.Models.c_bike
{
    
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
public class BIKEStationData {
    
    [System.Xml.Serialization.XmlElementAttribute("BIKEStation", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public BIKEStationDataBIKEStation[] Items
    {
        get;
        set;
    }
}

[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
public class BIKEStationDataBIKEStation {
    
    [System.Xml.Serialization.XmlElementAttribute("Station", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public BIKEStationDataBIKEStationStation[] Stations {
        get;
        set;
    }
}

[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public  class BIKEStationDataBIKEStationStation
{
    [System.Xml.Serialization.XmlElementAttribute("StationID", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public int Id
    {
        get;
        set;
    }

    [System.Xml.Serialization.XmlElementAttribute("StationName", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string Name
    {
        get;
        set;
    }



    [System.Xml.Serialization.XmlElementAttribute("StationLat", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public double Latitude
    {
        get;
        set;
    }

    [System.Xml.Serialization.XmlElementAttribute("StationLon", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public double Longitude
    {
        get;
        set;
    }

    [System.Xml.Serialization.XmlElementAttribute("StationNums1", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public int AvailableBikes
    {
        get;
        set;
    }

    [System.Xml.Serialization.XmlElementAttribute("StationNums2", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public int AvailableDocks
    {
        get;
        set;
    }
}
}

//    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
//    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
//    public partial class BIKEStationData
//    {
//        private Items[] items;
//        [System.Xml.Serialization.XmlElementAttribute("BIKESstation", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
//        public Items[] Items
//        {
//            get { return items; }
//            set { items = value; }
//        }

//    }
//    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
//    public partial class Items
//    {
//        private Station[] stations;
//        [System.Xml.Serialization.XmlElementAttribute("Station", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
//        public Station[] Stations
//        {
//            get { return stations; }
//            set { stations = value; }
//        }
//    }

//    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
//    public partial class Station
//    {

//        [System.Xml.Serialization.XmlElementAttribute("StationID", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
//        public int Id
//        {
//            get;
//            set;
//        }

//        /// <remarks/>
//        [System.Xml.Serialization.XmlElementAttribute("StationName", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
//        public string Name
//        {
//            get;
//            set;
//        }



//        /// <remarks/>
//        [System.Xml.Serialization.XmlElementAttribute("StationLat",Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
//        public double Latitude
//        {
//            get;
//            set;
//        }

//        /// <remarks/>
//        [System.Xml.Serialization.XmlElementAttribute("StationLon",Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
//        public double Longitude
//        {
//            get;
//            set;
//        }

//        [System.Xml.Serialization.XmlElementAttribute("StationNums1", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
//        public int AvailableBikes
//        {
//            get;
//            set;
//        }

//        [System.Xml.Serialization.XmlElementAttribute("StationNums2", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
//        public int AvailableDocks
//        {
//            get;
//            set;
//        }

//    }

  
//}
