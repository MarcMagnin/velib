using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VelibContext;
using Windows.UI.Core;
using Velib.Common;
using Windows.Web.Http;
using Windows.Devices.Geolocation;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;

namespace Velib.Contracts.Models.China
{
    public class PublicBicycleContract2 : Contract
    {
        public string AvailabilityUrl;
        [IgnoreDataMember]
        private Task Updater;
        public PublicBicycleContract2()
        {
            this.ServiceProvider = "Public Bicycle";
        }

        public override async void GetAvailableBikes(VelibModel unused, CoreDispatcher dispatcher)
        {
            if (Updater != null)
                return;
            Updater = new Task(async () =>
            {
                while (true)
                {
                    var httpClient = new HttpClient();
                    try
                    {
                        HttpResponseMessage response = await httpClient.GetAsync(new Uri(string.Format(ApiUrl + "?" + Guid.NewGuid().ToString())));
                        var responseBodyAsText = await response.Content.ReadAsStringAsync();
                        string pattern = @"(\[.*[\s\S]*?])";
                        if (Regex.IsMatch(responseBodyAsText, pattern))
                        {
                            var regex = new Regex(pattern).Match(responseBodyAsText);
                            if (regex != null && regex.Captures.Count > 0)
                            {
                                // require Velib.Common
                                var stations = regex.Captures[0].Value.FromJsonString<List<PublicBicycleModel>>();
                                foreach (var station in stations)
                                {
                                    foreach (var velib in Velibs)
                                    {
                                        if (velib.Latitude == station.Latitude && velib.Longitude == station.Longitude)
                                        {
                                            if (MainPage.BikeMode && velib.AvailableBikes != station.AvailableBikes)
                                            {
                                                velib.Reload = true;
                                            }
                                            if (!MainPage.BikeMode && velib.AvailableBikeStands != station.Capacity - station.AvailableBikes)
                                            {
                                                velib.Reload = true;
                                            }
                                            velib.AvailableBikes = station.AvailableBikes;
                                            velib.AvailableBikeStands = station.Capacity - station.AvailableBikes;
                                            velib.Loaded = true;
                                            break;
                                        }
                                    }
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

            string pattern = @"(\[.*[\s\S]*?])";
            if (Regex.IsMatch(responseBodyAsText, pattern))
            {
                var regex = new Regex(pattern).Match(responseBodyAsText);
                if (regex != null && regex.Captures.Count > 0)
                {
                    // require Velib.Common
                    var stations = regex.Captures[0].Value.FromJsonString<List<PublicBicycleModel>>();
                    Velibs = new List<VelibModel>();
                    foreach (var station in stations)
                    {
                        var stationModel = new VelibModel()
                        {
                            Contract = this,
                            Number = station.Id,
                            AvailableBikes = station.AvailableBikes,
                           // AvailableBikeStands = station.Capacity - station.AvailableBikes,
                            Location = new Windows.Devices.Geolocation.Geopoint(new BasicGeoposition()
                            {
                                Latitude = station.Latitude,
                                Longitude = station.Longitude
                            }),
                            Latitude = station.Latitude,
                            Longitude = station.Longitude,
                        };

                        if (MainPage.BikeMode)
                            stationModel.AvailableStr = stationModel.AvailableBikes.ToString();
                        else
                            stationModel.AvailableStr = stationModel.AvailableBikeStands.ToString();


                        Velibs.Add(stationModel);
                    }
                }
            }

        }

        public override Contract GetSimpleContract()
        {
            return (PublicBicycleContract2)base.GetSimpleContract();
        }

        protected override Contract GetInstanceForSimpleClone()
        {
            return new PublicBicycleContract2();
        }
    }
}
