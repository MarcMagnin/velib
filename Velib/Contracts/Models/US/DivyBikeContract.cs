﻿using System;
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
    // Chicago
    //https://docs.google.com/document/d/1gKN2Hq0-PxmMFBqg9e-xnwpRRh0GnmTVRQG3fjTxnT8/edit#
    public class DivyBikeContract: Contract
    {
        [IgnoreDataMember]
        private Task Updater;
        public DivyBikeContract()
        {
            DirectDownloadAvailability = true;
            ApiUrl = "http://www.divvybikes.com/stations/json";
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
            return (DivyBikeContract)base.GetSimpleContract();
        }

        protected override Contract GetInstanceForSimpleClone()
        {
            return new DivyBikeContract();
        }
    }
}

