using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VelibContext;
using Windows.Web.Http;
using Velib.Common;
using Windows.Devices.Geolocation;
using Windows.UI.Core;
using System.Diagnostics;

namespace Velib.Contracts.Models.Velo
{
    public class VeloPlusContract : Contract
    {
        private Task Updater;
        public VeloPlusContract()
        {
            ApiUrl = "https://www.agglo-veloplus.fr/fr/carte/";
            DirectDownloadAvailability = true;
            this.ServiceProvider = "Vélo'+";
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
                        string replacePattern = @"(// var spots =.*[\s\S]*?)(?<=;)";
                        var regex = new Regex(replacePattern).Match(responseBodyAsText);
                        if (regex != null && regex.Captures.Count > 0)
                        {
                            responseBodyAsText = responseBodyAsText.Replace(regex.Captures[0].Value, "");

                        }
                        string pattern = @"(?<=var spots =)(.*[\s\S]);";
                        if (Regex.IsMatch(responseBodyAsText, pattern))
                        {
                            regex = new Regex(pattern).Match(responseBodyAsText);
                            if (regex != null && regex.Captures.Count > 0)
                            {
                                var text = regex.Captures[0].Value;
                                text = text.Replace(";", "");
                                var model = text.FromJsonString<List<VeloPlusModel>>();
                                foreach (var station in model)
                                {
                                    foreach (var velibModel in Velibs)
                                    {
                                        if (velibModel.Latitude == station.GPS.Latitude && velibModel.Longitude == station.GPS.Longitude)
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
            HttpResponseMessage response = await downloadContractHttpClient.GetAsync(new Uri(string.Format(ApiUrl)));
            var responseBodyAsText = await response.Content.ReadAsStringAsync();

            string replacePattern = @"(// var spots =.*[\s\S]*?)(?<=;)";
            var regex = new Regex(replacePattern).Match(responseBodyAsText);
            if (regex != null && regex.Captures.Count > 0)
            {
                responseBodyAsText = responseBodyAsText.Replace(regex.Captures[0].Value, "");

            }
            string pattern = @"(?<=var spots =)(.*[\s\S]);";
            if (Regex.IsMatch(responseBodyAsText, pattern))
            {
                regex = new Regex(pattern).Match(responseBodyAsText);
                if (regex != null && regex.Captures.Count > 0)
                {
                    var text = regex.Captures[0].Value;
                    text = text.Replace(";", "");
                    var model = text.FromJsonString<List<VeloPlusModel>>();
                    Velibs = new List<VelibModel>();

                    foreach (var station in model)
                    {
                        var stationModel = new VelibModel()
                        {
                            Contract = this,
                            AvailableBikes = station.AvailableBikes,
                            AvailableBikeStands = station.AvailableDocks,
                            Location = new Windows.Devices.Geolocation.Geopoint(new BasicGeoposition()
                            {
                                Latitude = station.GPS.Latitude,
                                Longitude = station.GPS.Longitude
                            }),
                            Latitude = station.GPS.Latitude,
                            Longitude = station.GPS.Longitude,
                            Loaded = true
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
            return (VeloPlusContract)base.GetSimpleContract();
        }

        protected override Contract GetInstanceForSimpleClone()
        {
            return new VeloPlusContract();
        }
    }
}

