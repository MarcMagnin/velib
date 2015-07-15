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
using System.Globalization;
using Windows.Web.Http;

namespace Velib.Contracts.Models.Smoove
{
    // New York
    public class SmooveContract: Contract
    {
        [IgnoreDataMember]
        private Task Updater;
        public SmooveContract()
        {
            DirectDownloadAvailability = true;
            this.ServiceProvider = "Smoove";
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
                    var httpClient = new HttpClient();
                    try
                    {
                        HttpResponseMessage response = await httpClient.GetAsync(new Uri(string.Format(ApiUrl + "?" + Guid.NewGuid().ToString())));
                        var responseBodyAsText = await response.Content.ReadAsStringAsync();
                        var models = responseBodyAsText.FromXmlString<vcs>("").Node.Stations.ToList();
                        // for duplicates :/
                        var dupplicates = models.GroupBy(x => x.Latitude).Where(g => g.Count() > 1).ToList();
                        if (dupplicates.Count > 0)
                        {
                            var aggregatedStation = dupplicates.Select(t =>
                                new station
                                {
                                    AvailableBikes = t.Sum(b => b.AvailableBikes),
                                    AvailableDocks = t.Sum(b => b.AvailableDocks),
                                    Id = t.FirstOrDefault().Id,
                                    Latitude = t.FirstOrDefault().Latitude,
                                    Longitude = t.FirstOrDefault().Longitude,
                                    TotalDocks = t.Sum(b => b.TotalDocks)

                                });
                            models.RemoveAll(t => aggregatedStation.Any(v => v.Latitude == t.Latitude && v.Longitude == t.Longitude));
                            models.Add(aggregatedStation.FirstOrDefault());
                        }
                        foreach (var station in models)
                        {
                            double latitutde, longitude = 0;
                            if (!double.TryParse(station.Latitude, NumberStyles.AllowDecimalPoint, new CultureInfo("en-US"), out latitutde))
                                continue;
                            double.TryParse(station.Longitude, NumberStyles.AllowDecimalPoint, new CultureInfo("en-US"), out longitude);

                            foreach (var velibModel in Velibs)
                            {


                                if (velibModel.Latitude == latitutde && velibModel.Longitude == longitude)
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
                    catch (Exception )
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
            // require Velib.Common
            var models = responseBodyAsText.FromXmlString<vcs>("").Node.Stations.ToList();
            Velibs = new List<VelibModel>();
            // for duplicates :/
            
            var dupplicates = models.GroupBy(x => x.Latitude).Where(g => g.Count() > 1).ToList();
            if (dupplicates.Count > 0) { 
                var aggregatedStation = dupplicates.Select(t =>
                    new station
                    {
                        AvailableBikes = t.Sum(b => b.AvailableBikes),
                        AvailableDocks = t.Sum(b => b.AvailableDocks),
                        Id = t.FirstOrDefault().Id,
                        Latitude = t.FirstOrDefault().Latitude,
                        Longitude = t.FirstOrDefault().Longitude,
                        TotalDocks = t.Sum(b => b.TotalDocks)

                    });
                models.RemoveAll(t => aggregatedStation.Any(v => v.Latitude == t.Latitude && v.Longitude == t.Longitude));
                models.Add(aggregatedStation.FirstOrDefault());
            }
            foreach (var station in models)
            {

                double latitutde, longitude = 0;
                if (!double.TryParse(station.Latitude, NumberStyles.AllowDecimalPoint, new CultureInfo("en-US"), out latitutde))
                    continue;
                double.TryParse(station.Longitude, NumberStyles.AllowDecimalPoint, new CultureInfo("en-US"), out longitude);

                var stationModel = new VelibModel()
                {
                    Contract = this,
                    //Number = int.TryParse(station.Id,
                    //Name = station.Label,
                    AvailableBikes = station.AvailableBikes,
                    AvailableBikeStands = station.AvailableDocks,
                    Location = new Windows.Devices.Geolocation.Geopoint(new BasicGeoposition()
                    {
                        Latitude = latitutde,
                        Longitude = longitude
                    }),
                    Latitude = latitutde,
                    Longitude = longitude,
                    Loaded = true
                };
                if (Velibs.Contains(stationModel))
                {
                    // fuck that station
                    var duplicatedStation = Velibs.FirstOrDefault(t => t.Latitude == stationModel.Latitude && t.Longitude == stationModel.Longitude);
                    if(duplicatedStation != null){
                        duplicatedStation.AvailableBikes += stationModel.AvailableBikes;
                        duplicatedStation.AvailableBikeStands += stationModel.AvailableBikeStands;
                    }
                    if (MainPage.BikeMode)
                        duplicatedStation.AvailableStr = duplicatedStation.AvailableBikes.ToString();
                    else
                        duplicatedStation.AvailableStr = duplicatedStation.AvailableBikeStands.ToString();
                    continue;
                }


                if (MainPage.BikeMode)
                    stationModel.AvailableStr = stationModel.AvailableBikes.ToString();
                else
                    stationModel.AvailableStr = stationModel.AvailableBikeStands.ToString();

                Velibs.Add(stationModel);
            }
        }


        public override Contract GetSimpleContract()
        {
            return (SmooveContract)base.GetSimpleContract();
        }

        protected override Contract GetInstanceForSimpleClone()
        {
            return new SmooveContract();
        }
    }
}

