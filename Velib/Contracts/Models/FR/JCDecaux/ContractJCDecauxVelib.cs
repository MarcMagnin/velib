using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VelibContext;
using Velib.Common;
using Windows.UI.Popups;
using Windows.Devices.Geolocation;
using System.Runtime.Serialization;
using Windows.UI.Core;
using Windows.Web.Http;

namespace Velib.Contracts
{
    public class ContractJCDecauxVelib : Contract
    {
        [IgnoreDataMember]
        private static string dataURL = "https://api.jcdecaux.com/vls/v1/stations/{0}?contract={1}&apiKey=c3ae49d442f47c94ccfdb032328be969febe06ed";

        public ContractJCDecauxVelib()
        {
            this.ServiceProvider = "JCDecaux";
            ApiUrl = "https://developer.jcdecaux.com/rest/vls/stations/{0}.json";
        }

        public override async void GetAvailableBikes(VelibModel velibModel, CoreDispatcher dispatcher)
        {
            var httpClient = new HttpClient();

            //Helpers.ScenarioStarted(StartButton, CancelButton, OutputField);
            //rootPage.NotifyUser("In progress", NotifyType.StatusMessage);
            bool failed = true;
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(new Uri(string.Format(dataURL, velibModel.Number,this.Name)));//.AsTask(cts.Token);
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

        public override async Task InnerDownloadContract()
        {
            HttpResponseMessage response = await downloadContractHttpClient.GetAsync(new Uri(string.Format(ApiUrl, Name)));
            var  responseBodyAsText = await response.Content.ReadAsStringAsync();
            // require Velib.Common
            Velibs = responseBodyAsText.FromJsonString<List<VelibModel>>();
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
