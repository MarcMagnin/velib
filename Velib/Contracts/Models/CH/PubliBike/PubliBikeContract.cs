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

namespace Velib.Contracts.Models.CH.PubliBike
{
    // Chicago
    //https://docs.google.com/document/d/1gKN2Hq0-PxmMFBqg9e-xnwpRRh0GnmTVRQG3fjTxnT8/edit#
    public class PubliBikeContract: Contract
    {
        private Task Updater;
        public PubliBikeContract()
        {
            ApiUrl = "http://customers2011.ssmservice.ch/publibike/getterminals_v2.php";
            DirectDownloadAvailability = true;
            this.ServiceProvider = "PubliBike";
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
                   // httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
                    try
                    {
                        HttpResponseMessage response = await httpClient.GetAsync(new Uri(string.Format(ApiUrl + "?" + Guid.NewGuid().ToString())));
                        var responseBodyAsText = await response.Content.ReadAsStringAsync();
                        var model = responseBodyAsText.FromJsonString<PubliBikeModel>();

                        foreach (var station in model.Stations.Where(s => s.City == Name))
                        {
                            foreach (var velibModel in Velibs)
                            {
                                if (velibModel.Latitude == station.Latitude && velibModel.Longitude == station.Longitude)
                                {
                                    if (MainPage.BikeMode && velibModel.AvailableBikes != station.AvailableBikes.Sum(t=>t.Available))
                                    {
                                        velibModel.Reload = true;

                                    }
                                    if (!MainPage.BikeMode && velibModel.AvailableBikeStands != station.AvailableDocks.Sum(t => t.HoldersFree))
                                    {
                                        velibModel.Reload = true;
                                                    
                                    }
                                    velibModel.AvailableBikes = station.AvailableBikes.Sum(t => t.Available);
                                    velibModel.AvailableBikeStands = station.AvailableDocks.Sum(t => t.HoldersFree);
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
            //httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
            HttpResponseMessage response = await downloadContractHttpClient.GetAsync(new Uri(string.Format(ApiUrl)));
            var  responseBodyAsText = await response.Content.ReadAsStringAsync();
            var model = responseBodyAsText.FromJsonString<PubliBikeModel>();
            Velibs = new List<VelibModel>();
            //var test = model.Stations.GroupBy(s => s.City).Select(t => t.First()).Select(s => s.City).OrderBy(s=>s).Aggregate((c, next) => c + " \r\n" + next);
            foreach (var station in model.Stations.Where(s => TechnicalName.Split(',').Contains(s.City)))
            {
                var stationModel = new VelibModel()
                {
                    Contract = this,
                    Number = station.Id,
                    Name = station.Label,
                    AvailableBikes = station.AvailableBikes.Sum(t => t.Available),
                    AvailableBikeStands = station.AvailableDocks.Sum(t => t.HoldersFree),
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
            return (PubliBikeContract)base.GetSimpleContract();
        }

        protected override Contract GetInstanceForSimpleClone()
        {
            return new PubliBikeContract();
        }
    }
}

