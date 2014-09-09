using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using VelibContext;
using Windows.UI.Core;
using Velib.Common;
using Windows.UI.Popups;
using Windows.Devices.Geolocation;
using System.Threading;
using System.Net.Http;
using System.Diagnostics;


namespace Velib.Contracts.Models.Smoove

{
    // New York
    public class SmooveContract: Contract
    {
        [IgnoreDataMember]
        public string StationsUrl = "http://www.velhop.strasbourg.eu/vcstations.xml";

        public string ApiUrl = "http://www.velhop.strasbourg.eu/vcstations.xml";
        private DateTime nextUpdate;
        private CancellationTokenSource tokenSource;
        private Task Updater;
        public SmooveContract()
        {
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
                            if(tokenSource != null)
                                tokenSource.Cancel();
                            tokenSource = new CancellationTokenSource();

                            var httpClient = new GZipHttpClient();


                            
                                bool failed = true;
                                int count = 0;
                                try
                                {
                                    HttpResponseMessage response = await httpClient.GetAsync(new Uri(string.Format(ApiUrl + "?" + Guid.NewGuid().ToString())), tokenSource.Token);//.AsTask(cts.Token);
                                    var responseBodyAsText = await response.Content.ReadAsStringAsync();
                                    var model = responseBodyAsText.FromXmlString<stations>("");

                                    foreach (var station in model.Stations)
                                    {
                                        foreach (var velibModel in Velibs)
                                        {
                                            if (velibModel.Number == station.Id)
                                            {
                                                if (MainPage.BikeMode && velibModel.AvailableBikes != station.AvailableBikes)
                                                {
                                                    velibModel.Reload = true;
                                                    count++;      

                                                }
                                                if (!MainPage.BikeMode && velibModel.AvailableBikeStands != station.AvailableDocks)
                                                {
                                                    velibModel.Reload = true;
                                                    
                                                }
                                                velibModel.AvailableBikes = station.AvailableBikes;
                                                velibModel.AvailableBikeStands = station.AvailableDocks;
                                                velibModel.Loaded = true;
                                                break;
                                            }
                                            
                                        }
                                    }

                                    await dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
                                    {
                                        Debug.WriteLine(count + " reload");
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
                                await Task.Delay(TimeSpan.FromSeconds(20));
                            }
                        
                    });
                    Updater.Start();
        }


        public override async Task DownloadContract()
        {
            var httpClient = new GZipHttpClient();
            var velibs = new List<VelibModel>();
            Downloading = true;
            bool failed = false;
            
            if(tokenSource != null)
                tokenSource.Cancel();
            tokenSource = new CancellationTokenSource();

            try
            {
                //httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
              //Returned JSON
                HttpResponseMessage response = await httpClient.GetAsync(new Uri(string.Format(StationsUrl)));
                var responseBodyAsText = await response.Content.ReadAsStringAsync();
                
                // require Velib.Common
                var model = responseBodyAsText.FromXmlString<stations>("");
                VelibCounter = model.Stations.Length.ToString() + " stations";
                Velibs = new List<VelibModel>();
                //this.LastUpdate = tflModel.lastUpdate;
                foreach (var station in model.Stations)
                {
                    var stationModel = new VelibModel()
                    {
                        Contract = this,
                        Number = station.Id,
                        //Name = station.Label,
                        AvailableBikes = station.AvailableBikes,
                        AvailableBikeStands = station.AvailableDocks,
                        Location = new Windows.Devices.Geolocation.Geopoint(new BasicGeoposition()
                    {
                        Latitude = station.Latitude,
                        Longitude = station.Longitude
                    }),
                        Latitude = station.Latitude,
                        Longitude = station.Longitude,
                        Loaded = true
                    };

                    if (MainPage.BikeMode)
                        stationModel.AvailableStr = stationModel.AvailableBikes.ToString();
                    else
                        stationModel.AvailableStr = stationModel.AvailableBikeStands.ToString();

                    Velibs.Add(stationModel);
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
            return (SmooveContract)base.GetSimpleContract();
        }

        protected override Contract GetInstanceForSimpleClone()
        {
            return new SmooveContract();
        }
    }
}

