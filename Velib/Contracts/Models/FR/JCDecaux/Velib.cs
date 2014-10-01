using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Web.Http;
using Velib.Common;
using Windows.Foundation;
using Velib.VelibContext;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Core;
using Velib;
using Velib.Contracts;

namespace VelibContext
{
   

    [DataContract]
    public class Position
    {
        [DataMember(Name = "lng")]
        public double Longitude { get; set; }
        [DataMember(Name = "lat")]
        public double Latitude { get; set; }
    }

    public class VelibAddRemoveCollection
    {
        public List<VelibModel> ToAdd{ get; set; }
        public List<VelibModel> ToRemove { get; set; }
    }

    [DataContract]
    public class VelibModel : INotifyPropertyChanged
    {
        public bool Reload;
        public bool OnlyColorReload;
       
        public Point MapLocation;

        public Contract Contract{ get; set; }

        [IgnoreDataMember]
        public bool DownloadingAvailability{get;set;}
       

        public bool Loaded { get; set; }

        [DataMember(Name = "available_bike_stands")]
        public int? AvailableBikeStands { get; set; }


        [DataMember(Name = "available_bikes")]
        public int AvailableBikes { get; set; }



        private string availableStr;
        public string AvailableStr
        {
            get { return availableStr; }
            set
            {
                if (value != this.availableStr)
                {
                    this.availableStr = value;
                    NotifyPropertyChanged("AvailableStr");
                }
            }
        }
        //[{"number":31705,"name":"31705 - CHAMPEAUX (BAGNOLET)","address":"RUE DES CHAMPEAUX (PRES DE LA GARE ROUTIERE) - 93170 BAGNOLET","latitude":48.8645278209514,"longitude":2.416170724425901}

        [DataMember(Name = "latitude")]
        public double Latitude { get; set; }

        [DataMember(Name = "longitude")]
        public double Longitude { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "address")]
        public string Address { get; set; }

      [DataMember(Name = "number")]
        public int Number{ get; set; }

        public Geopoint Location { get; set; }

        [DataMember(Name = "position")]
        public Position Position { get; set; }

        public bool Locked{ get; set; }


        public override int GetHashCode()
        {
          return (Longitude+Latitude).GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            return this.Longitude == ((obj as VelibModel).Longitude) && this.Latitude == ((obj as VelibModel).Latitude);
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        public async Task GetAvailableBikes(CoreDispatcher dispatcher)
        {
            this.Contract.GetAvailableBikes(this, dispatcher);
            
        }

        // store the offsetLocation in order to reuse it for each draw cycle
        public Point OffsetLocation;
        public void SetOffsetLocation(MapControl _map)
        {
            _map.GetOffsetFromLocation(this.Location, out OffsetLocation);
        }
        public VelibControl VelibControl;

    }
}