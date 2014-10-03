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

namespace Velib.Contracts.Models.US
{
    // New York
    // http://www.citibikenyc.com/stations/json
    public class CitiBikeContract: Contract
    {
        private Task Updater;
        public CitiBikeContract()
        {
            DirectDownloadAvailability = true;
            ApiUrl = "http://www.citibikenyc.com/stations/json";

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
                                    HttpResponseMessage response = await httpClient.GetAsync(new Uri(string.Format(ApiUrl + "?" + Guid.NewGuid().ToString())));
                                    var responseBodyAsText = await response.Content.ReadAsStringAsync();
                                    var model = responseBodyAsText.FromJsonString<DivyBikeModel>();

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
                                await Task.Delay(TimeSpan.FromSeconds(20));
                            }
                        
                    });
                    Updater.Start();
        }

        public override async Task InnerDownloadContract()
        {
            //downloadContractHttpClient.DefaultRequestHeaders.Accept.Add(new Windows.Web.Http.Headers.HttpMediaTypeWithQualityHeaderValue("application/json"));
            //downloadContractHttpClient.DefaultRequestHeaders.AcceptEncoding.Add(new Windows.Web.Http.Headers.HttpContentCodingWithQualityHeaderValue("gzip"));
            //downloadContractHttpClient.DefaultRequestHeaders.AcceptEncoding.Add(new Windows.Web.Http.Headers.HttpContentCodingWithQualityHeaderValue("deflate"));
            HttpResponseMessage response = await downloadContractHttpClient.GetAsync(new Uri(ApiUrl));
            var responseBodyAsText = await response.Content.ReadAsStringAsync();
            // require Velib.Common
            var model = responseBodyAsText.FromJsonString<DivyBikeModel>();
            Velibs = new List<VelibModel>();
            foreach (var station in model.Stations)
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
            return (CitiBikeContract)base.GetSimpleContract();
        }

        protected override Contract GetInstanceForSimpleClone()
        {
            return new CitiBikeContract();
        }
    }
}

