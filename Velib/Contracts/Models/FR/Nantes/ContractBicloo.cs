using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VelibContext;
using Windows.Devices.Geolocation;
using Windows.Web.Http;
using Velib.Common;
using Velib.Contracts.Models;
using Windows.UI.Popups;
using Windows.UI.Core;


namespace Velib.Contracts
{
    public class ContractBicloo : Contract
    {
        [IgnoreDataMember]
        public string ApiUrl = "http://geovelo.nantesmetropole.fr/geovelo/getdecauxbikesharing/?contract=Nantes&apiKey=c3ae49d442f47c94ccfdb032328be969febe06ed&_=1409549782583";
        private DateTime nextUpdate;
        private Task Updater;
        public ContractBicloo()
        {
            RefreshTimer = 10;
            DirectDownloadAvailability = true;
        }
        // Barclays refresh every 3 minutes the stations informations :/
        public override async void GetAvailableBikes(VelibModel unused, CoreDispatcher dispatcher)
        {
            if (Updater != null)
                return;
                    Updater = new Task(async ()=>{
                        while (true)
                        {
                                var httpClient = new HttpClient();


                            
                                bool failed = true;
                                try
                                {
                                    HttpResponseMessage response = await httpClient.GetAsync(new Uri(string.Format(ApiUrl + "?" + Guid.NewGuid().ToString())));//.AsTask(cts.Token);
                                    var responseBodyAsText = await response.Content.ReadAsStringAsync();
                                    var items = responseBodyAsText.FromJsonString<List<VelibModel>>();
                                    VelibCounter = Velibs.Count.ToString() + " stations";
                                    foreach (var station in items)
                                    {
                                        foreach (var velibModel in Velibs)
                                        {
                                            if (velibModel.Number == station.Number)
                                            {
                                                if (MainPage.BikeMode && velibModel.AvailableBikes != station.AvailableBikes)
                                                {
                                                    velibModel.Reload = true;
                                                    

                                                }
                                                if (!MainPage.BikeMode && velibModel.AvailableBikeStands != station.AvailableBikeStands)
                                                {
                                                    velibModel.Reload = true;
                                                    
                                                }
                                                velibModel.AvailableBikes = station.AvailableBikes;
                                                velibModel.AvailableBikeStands = station.AvailableBikeStands;
                                                velibModel.Loaded = true;
                                                break;
                                            }
                                            
                                        }
                                    }

                                    await dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
                                    {
                                      foreach (var station in Velibs.Where(t => t.Reload && t.VelibControl != null && t.VelibControl.Velibs.Count == 1 ))
                                        {
                                            var control = station.VelibControl;
                                            if (control != null)
                                            {
                                                control.ShowVelibStation();
                                                control.ShowStationColor();
                                            }
                                            station.Reload = false;
                                           
                                        }
                                    });
                                    httpClient.Dispose();
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
                                }
                                await Task.Delay(TimeSpan.FromSeconds(RefreshTimer));
                            }
                        
                    });
                    Updater.Start();
        }


        public override async Task DownloadContract()
        {
            var httpClient = new HttpClient();
            var velibs = new List<VelibModel>();
            Downloading = true;
            bool failed = false;
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(new Uri(string.Format(ApiUrl, Name)));
                var responseBodyAsText = await response.Content.ReadAsStringAsync();
                // require Velib.Common
                Velibs = responseBodyAsText.FromJsonString<List<VelibModel>>();
                VelibCounter = Velibs.Count.ToString() + " stations";
                foreach (var station in Velibs)
                {
                    if (MainPage.BikeMode)
                        station.AvailableStr = station.AvailableBikes.ToString();
                    else
                        station.AvailableStr = station.AvailableBikeStands.ToString();

                    station.Location = new Windows.Devices.Geolocation.Geopoint(new BasicGeoposition()
                    {
                        Latitude = station.Position.Latitude,
                        Longitude = station.Position.Longitude
                    });
                    station.Latitude = station.Position.Latitude;
                    station.Longitude = station.Position.Longitude; 
                    station.Contract = this;
                    station.Loaded = true;

                }

                Downloaded = true;
                VelibDataSource.StaticVelibs.AddRange(Velibs);
                httpClient.Dispose();
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
            return (ContractBicloo)base.GetSimpleContract();
        }

        protected override Contract GetInstanceForSimpleClone()
        {
            return new ContractBicloo();
        }
    }
}

