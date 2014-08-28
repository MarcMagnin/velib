using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VelibContext;
using Windows.Web.Http;
using Velib.Common;
using Windows.UI.Popups;
using Windows.Devices.Geolocation;
using System.Runtime.Serialization;
using Windows.UI.Core;

namespace Velib.Contracts
{
    public class ContractJCDecauxVelib : Contract
    {
        [IgnoreDataMember]
        public string ApiUrl = "https://developer.jcdecaux.com/rest/vls/stations/{0}.json";
        private static string dataURL = "https://api.jcdecaux.com/vls/v1/stations/{0}?contract=Paris&apiKey=c3ae49d442f47c94ccfdb032328be969febe06ed";
        public override async void GetAvailableBikes(VelibModel velibModel, CoreDispatcher dispatcher)
        {
            var httpClient = new HttpClient();

            //Helpers.ScenarioStarted(StartButton, CancelButton, OutputField);
            //rootPage.NotifyUser("In progress", NotifyType.StatusMessage);
            bool failed = true;
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(new Uri(string.Format(dataURL, velibModel.Number)));//.AsTask(cts.Token);
                var responseBodyAsText = await response.Content.ReadAsStringAsync(); //.AsTask(cts.Token);
                var rootNode = responseBodyAsText.FromJsonString<VelibModel>();
                if ( MainPage.BikeMode && velibModel.AvailableBikes != rootNode.AvailableBikes)
                {
                    velibModel.Reload = true;
                    if(velibModel.AvailableBikes == -1)
                        velibModel.OnlyColorReload = true;
                    
                }
                if (!MainPage.BikeMode && velibModel.AvailableBikeStands != rootNode.AvailableBikeStands )
                {
                    velibModel.Reload = true;
                    if(velibModel.AvailableBikeStands == -1)
                        velibModel.OnlyColorReload = true;
                }

                
                velibModel.AvailableBikes = rootNode.AvailableBikes;
                velibModel.AvailableBikeStands = rootNode.AvailableBikeStands;

                if (velibModel.Reload)
                {
                    velibModel.Reload = false;
                    await dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
                    {
                        var control = velibModel.VelibControl;
                        if (control  != null)
                        {
                            //if (MainPage.BikeMode)
                            //{
                            //    velibModel.AvailableStr = velibModel.AvailableBikes.ToString();
                            //    control.ShowColor(velibModel.AvailableBikes);
                            //}
                            //else
                            //{
                            //    velibModel.AvailableStr = velibModel.AvailableBikeStands.ToString();
                            //    control.ShowColor(velibModel.AvailableBikeStands);
                            //}

                            //if(!velibModel.OnlyColorReload)
                                
                                // for contract like jcdecaux to prevent the loading animation to pass from grey color to colored one
                                if (velibModel.OnlyColorReload)
                                {
                                    control.ShowStationColor();
                                    velibModel.OnlyColorReload = false;
                                }
                                else
                                {
                                    control.ShowVelibStation();
                                    control.ShowStationColor();
                                }

                            //velibModel.OnlyColorReload = false;
                            
                        }

                    });
                }
                velibModel.Loaded = true;
                httpClient.Dispose();
                //cts.Token.ThrowIfCancellationRequested();
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
                //try
                //{
                //    //Uri dataUri = new Uri("ms-appx:///DataSample/Events.json");
                //    //StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(dataUri);
                //    //string jsonText = await FileIO.ReadTextAsync(file);
                //    //var rootNode = jsonText.FromJsonString<EventList>();
                //    //foreach (var evt in rootNode.Events)
                //    //{
                //    //    evt.Location = new Windows.Devices.Geolocation.Geopoint(new BasicGeoposition() { Latitude = evt.Latitude, Longitude = evt.Longitude });
                //    //    Events.Add(evt);
                //    //}
                //}
                //catch (Exception eee) { }
            }
        }

        public override async Task DownloadContract()
        {
            var httpClient = new HttpClient();
            var cts = new CancellationTokenSource();
            var velibs = new List<VelibModel>();
            Downloading = true;
            bool failed = false;
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(new Uri(string.Format(ApiUrl, Name))).AsTask(cts.Token);
                var responseBodyAsText = await response.Content.ReadAsStringAsync().AsTask(cts.Token);
                // require Velib.Common
                Velibs = responseBodyAsText.FromJsonString<List<VelibModel>>();
                VelibCounter = Velibs.Count.ToString() + " stations";
                foreach (var velib in Velibs)
                {
                    
                    velib.Location = new Windows.Devices.Geolocation.Geopoint(new BasicGeoposition()
                    {
                        Latitude = velib.Latitude,
                        Longitude = velib.Longitude
                    });
                    velib.Contract = this;
                    velib.AvailableBikes = -1;
                    velib.AvailableBikeStands = -1;
                }
                
                Downloaded = true;
                VelibDataSource.StaticVelibs.AddRange(Velibs);
                httpClient.Dispose();
                cts.Token.ThrowIfCancellationRequested();
            }
            catch (TaskCanceledException)
            {
                failed = true;
            }
            catch (Exception ex)
            {
                failed = true;
            }
            finally
            {
                Downloading = false;
                //  Helpers.ScenarioCompleted(StartButton, CancelButton);
            }
            if (failed)
            {
                var dialog = new MessageDialog("Sorry, you are currently not able to download " + Name);
                await dialog.ShowAsync();
            }
        }
        public override Contract GetSimpleContract()
        {
            return (ContractJCDecauxVelib)base.GetSimpleContract();
        }

        protected override Contract GetInstanceForSimpleClone()
        {
            return new ContractJCDecauxVelib();
        }
    }
}
