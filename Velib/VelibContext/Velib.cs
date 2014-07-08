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
        private static string dataURL = "https://api.jcdecaux.com/vls/v1/stations/{0}?contract=Paris&apiKey=c3ae49d442f47c94ccfdb032328be969febe06ed";
        private bool selected;

        public Point MapLocation;

        [IgnoreDataMember]
        public bool Selected
        {
            get
            {
                return this.selected;
            }

            set
            {

                if (value != this.selected)
                {
                    this.selected = value;
                    NotifyPropertyChanged();
                }
            }

        }

        [DataMember(Name = "available_bikes")]
        public int AvailableBikes { get; set; }

        private string availableBikesStr;
        public string AvailableBikesStr
        {
            get { return availableBikesStr; }
            set
            {
                if (value != this.availableBikesStr)
                {
                    this.availableBikesStr = value;
                    NotifyPropertyChanged("AvailableBikesStr");
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


        public override int GetHashCode()
        {
         return Number.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            return this.Number == ((obj as VelibModel).Number) ;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        public async Task GetAvailableBikes(CoreDispatcher dispatcher,CancellationToken token)
        {
            var httpClient = new HttpClient();
            var cts = new CancellationTokenSource();

            //Helpers.ScenarioStarted(StartButton, CancelButton, OutputField);
            //rootPage.NotifyUser("In progress", NotifyType.StatusMessage);
            bool failed = true;
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(new Uri(string.Format(dataURL, Number))).AsTask(cts.Token);
                var responseBodyAsText = await response.Content.ReadAsStringAsync().AsTask(cts.Token);
                var rootNode = responseBodyAsText.FromJsonString<VelibModel>();
                await dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
            {
                if (this.VelibControl != null)
                {
                    this.VelibControl.ShowVelibColor(rootNode.AvailableBikes);
                    this.AvailableBikesStr = rootNode.AvailableBikes.ToString();
                }
                //Velibs.Add(evt);
            });
                httpClient.Dispose();
                cts.Token.ThrowIfCancellationRequested();
            }
            catch (TaskCanceledException)
            {

            }
            catch (Exception ex)
            {
                failed = true;
            }
            finally
            {
                //  Helpers.ScenarioCompleted(StartButton, CancelButton);
            }
            if (failed)
            {
                // load local sample data
                try
                {
                    //Uri dataUri = new Uri("ms-appx:///DataSample/Events.json");
                    //StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(dataUri);
                    //string jsonText = await FileIO.ReadTextAsync(file);
                    //var rootNode = jsonText.FromJsonString<EventList>();
                    //foreach (var evt in rootNode.Events)
                    //{
                    //    evt.Location = new Windows.Devices.Geolocation.Geopoint(new BasicGeoposition() { Latitude = evt.Latitude, Longitude = evt.Longitude });
                    //    Events.Add(evt);
                    //}
                }
                catch (Exception eee) { }
            }
        }
        private static async Task GetDataAsync()
        {

            
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