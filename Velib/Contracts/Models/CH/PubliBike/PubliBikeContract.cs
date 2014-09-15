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

namespace Velib.Contracts.Models.CH.PubliBike
{
    // Chicago
    //https://docs.google.com/document/d/1gKN2Hq0-PxmMFBqg9e-xnwpRRh0GnmTVRQG3fjTxnT8/edit#
    public class PubliBikeContract: Contract
    {
        [IgnoreDataMember]
        private DateTime nextUpdate;
        private CancellationTokenSource tokenSource;
        private Task Updater;
        public PubliBikeContract()
        {
            ApiUrl = "http://customers2011.ssmservice.ch/publibike/getterminals_v2.php";
            DirectDownloadAvailability = true;
            this.ServiceProvider = "PubliBike";
        }

   
        // Barclays refresh every 3 minutes the stations informations :/
        public override async void GetAvailableBikes(VelibModel unused, CoreDispatcher dispatcher)
        {
            if (Updater != null)
                return;
            Updater = new Task(async () =>
            {
                while (true)
                {
                    if (tokenSource != null)
                        tokenSource.Cancel();
                    tokenSource = new CancellationTokenSource();

                    var httpClient = new GZipHttpClient();
                    bool failed = true;
                    int count = 0;
                    try
                    {
                        HttpResponseMessage response = await httpClient.GetAsync(new Uri(string.Format(ApiUrl + "?" + Guid.NewGuid().ToString())), tokenSource.Token);//.AsTask(cts.Token);
                        var responseBodyAsText = await response.Content.ReadAsStringAsync();
                        var model = responseBodyAsText.FromJsonString<PubliBikeModel>();

                        foreach (var station in model.Stations.Where(s => s.City == Name))
                        {
                            foreach (var velibModel in Velibs)
                            {
                                if (velibModel.Number == station.Id)
                                {
                                    if (MainPage.BikeMode && velibModel.AvailableBikes != station.AvailableBikes.Sum(t=>t.Available))
                                    {
                                        velibModel.Reload = true;
                                        count++;      

                                    }
                                    if (!MainPage.BikeMode && velibModel.AvailableBikeStands != station.AvailableDocks.Sum(t => t.HoldersFree))
                                    {
                                        velibModel.Reload = true;
                                                    
                                    }
                                    velibModel.AvailableBikes = station.AvailableBikes.Sum(t => t.Available);
                                    velibModel.AvailableBikeStands = station.AvailableDocks.Sum(t => t.HoldersFree);
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
                    await Task.Delay(RefreshTimer);
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
                HttpResponseMessage response = await httpClient.GetAsync(new Uri(string.Format(ApiUrl)));
                var responseBodyAsText = await response.Content.ReadAsStringAsync();
                
                // require Velib.Common
                var model = responseBodyAsText.FromJsonString<PubliBikeModel>();
                VelibCounter = model.Stations.Where(s=>s.City == TechnicalName).ToList().Count.ToString() + " stations";

                //var test = model.Stations.GroupBy(s => s.City).Select(t => t.First()).Select(s => s.City).OrderBy(s=>s).Aggregate((c, next) => c + " \r\n" + next);
                //Debug.WriteLine(test);
                Velibs = new List<VelibModel>();
                //this.LastUpdate = tflModel.lastUpdate;
                foreach (var station in model.Stations.Where(s => s.City == TechnicalName))
                {
                    var stationModel = new VelibModel()
                    {
                        Contract = this,
                        Number = station.Id,
                        Name = station.Label,
                        AvailableBikes = station.AvailableBikes.Sum(t=>t.Available),
                        AvailableBikeStands = station.AvailableDocks.Sum(t=>t.HoldersFree),
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
            return (PubliBikeContract)base.GetSimpleContract();
        }

        protected override Contract GetInstanceForSimpleClone()
        {
            return new PubliBikeContract();
        }
    }
}

