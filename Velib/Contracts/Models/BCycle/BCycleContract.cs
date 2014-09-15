using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VelibContext;
using Windows.UI.Core;
using Windows.UI.Popups;
using Velib.Common;
using Windows.Devices.Geolocation;

namespace Velib.Contracts.Models.BCycle
{
    public class BCycleContract: Contract
    {
        [IgnoreDataMember]
        private CancellationTokenSource tokenSource;
        private Task Updater;
        private string ApiKey = "A231E49A-920B-4C20-8752-E1B650ED1A49";
        public BCycleContract()
        {
            this.ServiceProvider = "B-cycle";
            DirectDownloadAvailability = true;
            ApiUrl = "https://publicapi.bcycle.com/api/1.0/ListProgramKiosks/{0}";
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

                    var httpClient = new HttpClient();
                    bool failed = true;
                    try
                    {
                        httpClient.DefaultRequestHeaders.Add("ApiKey", ApiKey);
                        HttpResponseMessage response = await httpClient.GetAsync(new Uri(string.Format(ApiUrl, Id)), tokenSource.Token);//.AsTask(cts.Token);
                        var responseBodyAsText = await response.Content.ReadAsStringAsync();
                        var model = responseBodyAsText.FromJsonString<List<BCycleModel>>();
                        foreach (var station in model)
                        {
                            foreach (var velibModel in Velibs)
                            {
                                if (velibModel.Number == station.Id)
                                {
                                    if (MainPage.BikeMode && velibModel.AvailableBikes != station.AvailableBikes)
                                    {
                                        velibModel.Reload = true;
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
                            foreach (var station in Velibs.Where(t => t.Reload && t.VelibControl != null && t.VelibControl.Velibs.Count == 1))
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
            var httpClient = new HttpClient();
            var velibs = new List<VelibModel>();
            Downloading = true;
            bool failed = false;
            
            if(tokenSource != null)
                tokenSource.Cancel();
            tokenSource = new CancellationTokenSource();

            try
            {
                httpClient.DefaultRequestHeaders.Add("ApiKey", ApiKey);
                HttpResponseMessage response = await httpClient.GetAsync(new Uri(string.Format(ApiUrl,Id)));
                var responseBodyAsText = await response.Content.ReadAsStringAsync();
                
                // require Velib.Common
                var model = responseBodyAsText.FromJsonString<List<BCycleModel>>();
                VelibCounter = model.Count.ToString() + " stations";
               
               // var test =model.Items;
               // var coolstring ="";
               // foreach (var t in test){
               //   coolstring += t.city.Select(v=>t.country +" : "+ v.name+ " : " + v.uid ).Aggregate((v1, v2)=> v1 + "\r\n"+ v2);
               //   coolstring += "\r\n";
               // }
               // Debug.WriteLine(coolstring);

                Velibs = new List<VelibModel>();
                //this.LastUpdate = tflModel.lastUpdate;
                foreach (var station in model)
                {
                    var stationModel = new VelibModel()
                    {
                        Contract = this,
                        Number = station.Id,
                        Name = station.Label,
                        AvailableBikes = station.AvailableBikes,
                        AvailableBikeStands = station.AvailableDocks,
                        Location = new Windows.Devices.Geolocation.Geopoint(new BasicGeoposition()
                        {
                            Latitude = station.Location.Latitude,
                            Longitude = station.Location.Longitude
                        }),
                        Latitude = station.Location.Latitude,
                        Longitude = station.Location.Longitude,
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
            return (BCycleContract)base.GetSimpleContract();
        }

        protected override Contract GetInstanceForSimpleClone()
        {
            return new BCycleContract();
        }
    }
}

