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


namespace Velib.Contracts.Models.NextBike{
    // New York
    public class NextBikeContract: Contract
    {
        private Task Updater;
        public NextBikeContract()
        {
            DirectDownloadAvailability = true;
            ApiUrl = "http://nextbike.net/maps/nextbike-live.xml?city={0}";
            this.ServiceProvider = "NextBike";
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
                        HttpResponseMessage response = await httpClient.GetAsync(new Uri(string.Format(ApiUrl, Id)));
                        var responseBodyAsText = await response.Content.ReadAsStringAsync();
                        var model = responseBodyAsText.FromXmlString<markers>("");
                        var city = model.Items.FirstOrDefault().city.FirstOrDefault();
                        foreach (var station in city.place)
                        {
                            foreach (var velibModel in Velibs)
                            {
                                if (velibModel.Latitude == station.lat && velibModel.Longitude == station.lng)
                                {
                                    if (MainPage.BikeMode)
                                    {
                                        if (station.bikes != null && (station.bikes.Contains("+") && velibModel.AvailableBikes != 5) || !station.bikes.Contains("+") && station.bikes != velibModel.AvailableBikes.ToString())
                                        velibModel.Reload = true;

                                    }
                                    if (!MainPage.BikeMode)
                                    {
                                        if (station.bike_racks != null && (station.bike_racks.Contains("+") && velibModel.AvailableBikeStands != 5) || !station.bike_racks.Contains("+") && station.bike_racks != velibModel.AvailableBikeStands.ToString())
                                        velibModel.Reload = true;

                                    }
                                    velibModel.AvailableBikes =  station.bikes.Contains('+') ? 5 : int.Parse(station.bikes);
                                    velibModel.AvailableBikeStands =  station.bike_racks == null ? 0 : int.Parse(station.bike_racks);
                                    velibModel.Loaded = true;
                                    break;
                                }

                            }
                        }
                    }
                    catch (Exception ex)
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
            HttpResponseMessage response = await downloadContractHttpClient.GetAsync(new Uri(string.Format(ApiUrl, Id)));
            var responseBodyAsText = await response.Content.ReadAsStringAsync();
            // require Velib.Common
            var  model = responseBodyAsText.FromXmlString<markers>("");

            //httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //Returned JSON

         
            // var test =model.Items;
            // var coolstring ="";
            // foreach (var t in test){
            //   coolstring += t.city.Select(v=>t.country +" : "+ v.name+ " : " + v.uid ).Aggregate((v1, v2)=> v1 + "\r\n"+ v2);
            //   coolstring += "\r\n";
            // }
            // Debug.WriteLine(coolstring);

            Velibs = new List<VelibModel>();
            //this.LastUpdate = tflModel.lastUpdate;
            foreach (var station in model.Items.FirstOrDefault().city.FirstOrDefault().place)
            {
                var stationModel = new VelibModel()
                {
                    Contract = this,
                    Number = station.uid,
                    //Name = station.Label,
                    AvailableBikes = station.bikes.Contains('+') ? 5 : int.Parse(station.bikes),
                    AvailableBikeStands = station.bike_racks == null ? 0 : int.Parse(station.bike_racks),
                    Location = new Windows.Devices.Geolocation.Geopoint(new BasicGeoposition()
                    {
                        Latitude = station.lat,
                        Longitude = station.lng,
                    }),
                    Latitude = station.lat,
                    Longitude = station.lng,
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
            return (NextBikeContract)base.GetSimpleContract();
        }

        protected override Contract GetInstanceForSimpleClone()
        {
            return new NextBikeContract();
        }
    }
}

