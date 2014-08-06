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
    public class ContractTFLLondon : Contract
    {
        [IgnoreDataMember]
        public string ApiUrl = "http://www.tfl.gov.uk/tfl/syndication/feeds/cycle-hire/livecyclehireupdates.xml";
        private string ApplicationId = "37451f1f";
        private string ApplicationKeys = "b62f82ae23b105aad2ecc72c48c35c3d";
        private DateTime nextUpdate;
        private Task Updater;
        public ContractTFLLondon()
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
                                var httpClient = new HttpClient();


                            
                                bool failed = true;
                                try
                                {
                                    HttpResponseMessage response = await httpClient.GetAsync(new Uri(string.Format(ApiUrl + "?" + Guid.NewGuid().ToString())));//.AsTask(cts.Token);
                                    var responseBodyAsText = await response.Content.ReadAsStringAsync();
                                    var tflModel = responseBodyAsText.FromXmlString<stations>("stations");
                                    //this.LastUpdate = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                                    //this.LastUpdate = this.LastUpdate.AddMilliseconds(Convert.ToInt64(tflModel.lastUpdate)).ToLocalTime();
                                    //nextUpdate = LastUpdate.AddMinutes(3);

                                    foreach (var station in tflModel.station)
                                    {
                                        foreach (var velibModel in Velibs)
                                        {
                                            velibModel.Reload = false;
                                            if (velibModel.Number == station.id)
                                            {
                                                if (MainPage.BikeMode && velibModel.AvailableBikes != station.nbBikes)
                                                {
                                                    velibModel.Reload = true;
                                                    

                                                }
                                                if (!MainPage.BikeMode && velibModel.AvailableBikeStands != station.nbEmptyDocks)
                                                {
                                                    velibModel.Reload = true;
                                                    
                                                }
                                                velibModel.AvailableBikes = station.nbBikes;
                                                velibModel.AvailableBikeStands = station.nbEmptyDocks;
                                            }
                                            velibModel.Loaded = true;
                                        }
                                    }

                                    await dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
                                    {
                                        foreach (var velibControl in Velibs.Where(t => t.VelibControl != null).Select(t => t.VelibControl))
                                        {

                                            if (velibControl.Velibs.Count != 1)
                                                continue;
                                            var station = velibControl.Velibs.FirstOrDefault();
                                            if (station == null)
                                                continue;

                                            if (station.Reload )
                                            {
                                                velibControl.ShowVelibStation();
                                                velibControl.ShowStationColor();
                                                station.Reload = false;
                                            }
                                                

                                           

                                            //if (MainPage.BikeMode)
                                            //{
                                            //    velibControl.ShowColor(station.AvailableBikes);
                                            //    station.AvailableStr = station.AvailableBikes.ToString();
                                            //}
                                            //else
                                            //{
                                            //    velibControl.ShowColor(station.AvailableBikeStands);
                                            //    station.AvailableStr = station.AvailableBikeStands.ToString();
                                            //}
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
                                await Task.Delay(TimeSpan.FromMinutes(1));
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
                var tflModel = responseBodyAsText.FromXmlString<stations>("stations");
                VelibCounter = tflModel.station.Length.ToString() + " stations";
                Velibs = new List<VelibModel>();
                //this.LastUpdate = tflModel.lastUpdate;
                foreach (var station in tflModel.station)
                {
                    var stationModel = new VelibModel()
                    {
                        Contract = this,
                        Number = station.id,
                        Name = station.name,
                        AvailableBikes = station.nbBikes,
                        AvailableBikeStands = station.nbEmptyDocks,
                        Location = new Windows.Devices.Geolocation.Geopoint(new BasicGeoposition()
                    {
                        Latitude = station.lat,
                        Longitude = station.@long
                    }),
                        Latitude = station.lat,
                        Longitude = station.@long,
                        Loaded = true
                    };

                    if (MainPage.BikeMode)
                        stationModel.AvailableStr = stationModel.AvailableBikes.ToString();
                    else
                        stationModel.AvailableStr = stationModel.AvailableBikeStands.ToString();

                    Velibs.Add(stationModel);

                    //velib.Location = new Windows.Devices.Geolocation.Geopoint(new BasicGeoposition()
                    //{
                    //    Latitude = velib.Latitude,
                    //    Longitude = velib.Longitude
                    //});
                    //velib.AvailableBikes = -1;
                    //velib.AvailableBikeStands = -1;
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
            return (ContractTFLLondon)base.GetSimpleContract();
        }

        protected override Contract GetInstanceForSimpleClone()
        {
            return new ContractTFLLondon();
        }
    }
}

