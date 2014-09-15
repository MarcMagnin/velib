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
using System.Net.Http;
using System.Diagnostics;

namespace Velib.Contracts.Models.Stras.Smoove
{
    // New York
    public class StrasSmooveContract: Contract
    {
        [IgnoreDataMember]
        private CancellationTokenSource tokenSource;
        [IgnoreDataMember]
        private Task Updater;
        public StrasSmooveContract()
        {
            DirectDownloadAvailability = true;
            ApiUrl = "http://www.velhop.strasbourg.eu/vcstations.xml";
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
                    if (tokenSource != null)
                        tokenSource.Cancel();
                    tokenSource = new CancellationTokenSource();
                    var httpClient = new HttpClient();
                    try
                    {
                        HttpResponseMessage response = await httpClient.GetAsync(new Uri(string.Format(ApiUrl + "?" + Guid.NewGuid().ToString())), tokenSource.Token);//.AsTask(cts.Token);
                        var responseBodyAsText = await response.Content.ReadAsStringAsync();
                        var model = responseBodyAsText.FromXmlString<vcs>("");

                        foreach (var station in model.Node.Stations)
                        {
                            foreach (var velibModel in Velibs)
                            {
                                if (velibModel.Number == station.Id)
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

                        await dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
                        {
                            foreach (var station in Velibs.Where(t => t.Reload && t.VelibControl != null && t.VelibControl.Velibs.Count == 1))
                            {
                                var control = station.VelibControl;
                                if (control != null)
                                {
                                    control.ShowVelibStation();
                                    control.ShowStationColor();
                                }
                                station.Reload = false;

                            }

                        });
                        httpClient.Dispose();
                    }
                    catch (TaskCanceledException)
                    {

                    }
                    catch (Exception ex)
                    {
                    }
                    finally
                    {
                    }
                    await Task.Delay(RefreshTimer);
                }

            });
            Updater.Start();
        }


        public override async Task DownloadContract()
        {
            var httpClient = new HttpClient();
            var velibs = new List<VelibModel>();
            Downloading = true;
            bool failed = false;
            
            if(tokenSource != null)
                tokenSource.Cancel();
            tokenSource = new CancellationTokenSource();

            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(new Uri(string.Format(ApiUrl)));
                var responseBodyAsText = await response.Content.ReadAsStringAsync();
                
                // require Velib.Common
                var model = responseBodyAsText.FromXmlString<vcs>("");
                VelibCounter = model.Node.Stations.Length.ToString() + " stations";
                Velibs = new List<VelibModel>();
                foreach (var station in model.Node.Stations)
                {
                    var stationModel = new VelibModel()
                    {
                        Contract = this,
                        Number = station.Id,
                        //Name = station.Label,
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

                Downloaded = true;
                VelibDataSource.StaticVelibs.AddRange(Velibs);
                httpClient.Dispose();
            }
            catch (TaskCanceledException)
            {
                failed = true;
            }
            catch (Exception ex)
            {
                failed = true;
            }
            finally
            {
                Downloading = false;
            }
            if (failed)
            {
                var dialog = new MessageDialog("Sorry, you are currently not able to download " + Name);
                await dialog.ShowAsync();
            }
        }


        public override Contract GetSimpleContract()
        {
            return (StrasSmooveContract)base.GetSimpleContract();
        }

        protected override Contract GetInstanceForSimpleClone()
        {
            return new StrasSmooveContract();
        }
    }
}

