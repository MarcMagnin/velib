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
using System.Diagnostics;
using Windows.Web.Http;


namespace Velib.Contracts.Models.US.Washington
{
    // New York
    public class CapitalBikeShareContract: Contract
    {
        [IgnoreDataMember]
        private Task Updater;
        public CapitalBikeShareContract()
        {
            DirectDownloadAvailability = true;
            ApiUrl = "http://www.capitalbikeshare.com/data/stations/bikeStations.xml";

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

                                try
                                {
                                    HttpResponseMessage response = await httpClient.GetAsync(new Uri(string.Format(ApiUrl )));
                                    var responseBodyAsText = await response.Content.ReadAsStringAsync();
                                    var model = responseBodyAsText.FromXmlString<stations>("");

                                    foreach (var station in model.Stations)
                                    {
                                        foreach (var velibModel in Velibs)
                                        {
                                            if (velibModel.Latitude == station.Latitude && velibModel.Longitude == station.Longitude)
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
                                }
                                catch (Exception)
                                {
                                }
                                finally
                                {
                                    httpClient.Dispose();
                                }
                                await Task.Delay(RefreshTimer);
                            }
                        
                    });
                    Updater.Start();
        }

        public override async Task InnerDownloadContract()
        {
            //httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //Returned JSON
            HttpResponseMessage response = await downloadContractHttpClient.GetAsync(new Uri(ApiUrl));
            var responseBodyAsText = await response.Content.ReadAsStringAsync();

            // require Velib.Common
            var model = responseBodyAsText.FromXmlString<stations>("");
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
        }

        public override Contract GetSimpleContract()
        {
            return (CapitalBikeShareContract)base.GetSimpleContract();
        }

        protected override Contract GetInstanceForSimpleClone()
        {
            return new CapitalBikeShareContract();
        }
    }
}

