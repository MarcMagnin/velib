using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Velib.Common;
using VelibContext;
using Windows.Devices.Geolocation;
using Windows.UI.Core;
using Windows.Web.Http;

namespace Velib.Contracts.Models.SP
{
    public class BicimadContract : Contract
    {
        private Task Updater;
        public BicimadContract()
        {
            DirectDownloadAvailability = true;
            ServiceProvider = "BiciMAD";
            ApiUrl = "http://www.infobicimad.com/estaciones.json";
        }


        public override async void GetAvailableBikes(VelibModel unused, CoreDispatcher dispatcher)
        {
            if (Updater != null)
                return;
            Updater = new Task(async () => {
                while (true)
                {
                    var httpClient = new HttpClient();
                    try
                    {
                        HttpResponseMessage response = await httpClient.GetAsync(new Uri(string.Format(ApiUrl + "?" + Guid.NewGuid().ToString())));
                        var responseBodyAsText = await response.Content.ReadAsStringAsync();
                        var model = JsonConvert.DeserializeObject<BicimadModel>(responseBodyAsText).Stations;

                        foreach (var station in model)
                        {
                            foreach (var velibModel in Velibs)
                            {
                                if (velibModel.Latitude == station.Latitude && velibModel.Longitude == station.Longitude)
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
            HttpResponseMessage response = await downloadContractHttpClient.GetAsync(new Uri(ApiUrl));
            var responseBodyAsText = await response.Content.ReadAsStringAsync();
            // require Velib.Common
            var model = JsonConvert.DeserializeObject<BicimadModel>(responseBodyAsText).Stations;
            Velibs = new List<VelibModel>();
            foreach (var station in model)
            {
                var stationModel = new VelibModel()
                {
                    Contract = this,
                    AvailableBikes = station.AvailableBikes,
                    AvailableBikeStands = station.AvailableBikeStands,
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
            return (BicimadContract)base.GetSimpleContract();
        }

        protected override Contract GetInstanceForSimpleClone()
        {
            return new BicimadContract();
        }
    }
}
