using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using VelibContext;
using Windows.UI.Core;
using Windows.Web.Http;
using Velib.Common;
using Windows.Devices.Geolocation;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Globalization;


namespace Velib.Contracts.Models.China
{

    /// <summary>
    /// map : http://hz.2773456.com/zdfb/
    /// website : http://www.2773456.com/
    /// utf 8 decode : http://www.endmemo.com/unicode/unicodeconverter.php
    /// </summary>
    public class HuiminOperateContract : Contract
    {
        [IgnoreDataMember]
        private Task Updater;
        public HuiminOperateContract()
        {
            DirectDownloadAvailability = true;
            ApiUrl = "http://hz.2773456.com/zdfb/sz_station.php";
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

                        string pattern = @" (\{.*[\s\S]*?\})";
                        if (Regex.IsMatch(responseBodyAsText, pattern))
                        {
                            var regex = new Regex(pattern).Match(responseBodyAsText);
                            if (regex != null && regex.Captures.Count > 0)
                            {
                                dynamic stations = JsonConvert.DeserializeObject<dynamic>(regex.Captures[0].Value);

                                foreach (var station in stations)
                                {
                                    if (station.Value["FDZBZ"].Value == null)
                                        continue;

                                    var location = station.Value["FDZBZ"].Value.Split(',');
                                    double latitude, longitude;
                                    if (!double.TryParse(station.Value["lng"].Value, NumberStyles.AllowDecimalPoint, new CultureInfo("en-US"), out longitude))
                                        continue;
                                    double.TryParse(station.Value["lat"].Value, NumberStyles.AllowDecimalPoint, new CultureInfo("en-US"), out latitude);

                                    latitude = latitude - 0.005689;
                                    longitude = longitude - 0.00652;

                                    foreach (var velibModel in Velibs)
                                    {
                                        if (velibModel.Latitude == latitude && velibModel.Longitude == longitude)
                                        {
                                            if (MainPage.BikeMode && velibModel.AvailableBikes != (int)station.Value["DQCSZ"].Value)
                                            {
                                                velibModel.Reload = true;
                                            }
                                            if (!MainPage.BikeMode && velibModel.AvailableBikeStands != (int)station.Value["kzcs"].Value)
                                            {
                                                velibModel.Reload = true;
                                            }
                                            velibModel.AvailableBikes = (int)station.Value["DQCSZ"].Value;
                                            velibModel.AvailableBikeStands = (int)station.Value["kzcs"].Value;
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
                    await Task.Delay(TimeSpan.FromSeconds(20));
                }

            });
            Updater.Start();
        }

        public override async Task InnerDownloadContract()
        {
            HttpResponseMessage response = await downloadContractHttpClient.GetAsync(new Uri(ApiUrl));
            var responseBodyAsText = await response.Content.ReadAsStringAsync();
            string pattern = @" (\{.*[\s\S]*?\})";
            if (Regex.IsMatch(responseBodyAsText, pattern))
            {
                var regex = new Regex(pattern).Match(responseBodyAsText);
                if (regex != null && regex.Captures.Count > 0)
                {
                    dynamic stations = JsonConvert.DeserializeObject<dynamic>(regex.Captures[0].Value);

                    Velibs = new List<VelibModel>();
                    foreach (var station in stations)
                    {
                        if (station.Value["FDZBZ"].Value == null)
                            continue;

                        var location = station.Value["FDZBZ"].Value.Split(',');
                        double latitude, longitude;
                        if (!double.TryParse(station.Value["lng"].Value, NumberStyles.AllowDecimalPoint, new CultureInfo("en-US"), out longitude))
                            continue;
                        double.TryParse(station.Value["lat"].Value, NumberStyles.AllowDecimalPoint, new CultureInfo("en-US"), out latitude);


                        latitude = latitude - 0.005689;
                        longitude = longitude - 0.00652;

                        var stationModel = new VelibModel()
                        {
                            Contract = this,
                            AvailableBikes = (int)station.Value["DQCSZ"].Value,
                            AvailableBikeStands = (int)station.Value["kzcs"].Value,
                            Location = new Windows.Devices.Geolocation.Geopoint(new BasicGeoposition()
                            {
                                Latitude = latitude,
                                Longitude = longitude
                            }),
                            Latitude = latitude,
                            Longitude = longitude,
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
            return (HuiminOperateContract)base.GetSimpleContract();
        }

        protected override Contract GetInstanceForSimpleClone()
        {
            return new HuiminOperateContract();
        }
    }
}
