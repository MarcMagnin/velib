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

namespace Velib.Contracts.Models.EasyBike
{
     public class EasyBikeContract: Contract
    {
        private Task Updater;
        public EasyBikeContract()
        {
            ApiUrl = "http://map.easybike.gr/";
            DirectDownloadAvailability = true;
            this.ServiceProvider = "Easy Bike";
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
                        string pattern = @"(?<=var list =)(.*[\s\S]*?])(?=;)";
                        if (Regex.IsMatch(responseBodyAsText, pattern))
                        {
                            var regex = new Regex(pattern).Match(responseBodyAsText);
                            if (regex != null && regex.Captures.Count > 0)
                            {
                                var text = regex.Captures[0].Value;
                                text = text.Replace("lat:", "\"lat\":");
                                text = text.Replace("lng:", "\"lng\":");
                                text = text.Replace("data:", "\"data\":");
                                text = text.Replace("desc:", "\"desc\":");
                                text = text.Replace("text:", "\"text\":");
                                text = text.Replace("logo:", "\"logo\":");
                                text = text.Replace("options:", "\"options\":");
                                text = text.Replace("icon:", "\"icon\":");
                                text = text.Replace("green_bike", "\"green_bike\"");
                                text = text.Remove(text.LastIndexOf(","), 1);
                                var model = text.FromJsonString<List<EasyBikeModel>>();
                                foreach (var item in model)
                                {
                                    pattern = @"\d+";
                                    if (Regex.IsMatch(item.Data.Text, pattern))
                                    {
                                        var matches = new Regex(pattern).Matches(item.Data.Text);
                                        item.AvailableDocks = int.Parse(matches[1].Value);
                                        item.AvailableBikes = int.Parse(matches[2].Value);
                                    }
                                }
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
            var  responseBodyAsText = await response.Content.ReadAsStringAsync();
           
            string pattern = @"(?<=var list =)(.*[\s\S]*?])(?=;)";
            if (Regex.IsMatch(responseBodyAsText, pattern))
            {
                var regex = new Regex(pattern).Match(responseBodyAsText);
                if (regex != null && regex.Captures.Count > 0)
                {
                    var text = regex.Captures[0].Value;
                    text = text.Replace("lat:", "\"lat\":");
                    text = text.Replace("lng:", "\"lng\":");
                    text = text.Replace("data:", "\"data\":");
                    text = text.Replace("desc:", "\"desc\":");
                    text = text.Replace("text:", "\"text\":");
                    text = text.Replace("logo:", "\"logo\":");
                    text = text.Replace("options:", "\"options\":");
                    text = text.Replace("icon:", "\"icon\":");
                    text = text.Replace("green_bike", "\"green_bike\"");
                    text = text.Remove(text.LastIndexOf(","), 1);
                    var model = text.FromJsonString<List<EasyBikeModel>>();
                    foreach (var item in model)
                    {
                        pattern = @"\d+";
                        if (Regex.IsMatch(item.Data.Text, pattern))
                        {
                            var matches = new Regex(pattern).Matches(item.Data.Text);
                            item.AvailableDocks= int.Parse(matches[1].Value);
                            item.AvailableBikes= int.Parse(matches[2].Value);
                        }
                    }
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
            }
            



        }
        public override Contract GetSimpleContract()
        {
            return (EasyBikeContract)base.GetSimpleContract();
        }

        protected override Contract GetInstanceForSimpleClone()
        {
            return new EasyBikeContract();
        }
    }
}

