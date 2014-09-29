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

namespace Velib.Contracts.Models.MVG
{
    public class MVGContract: Contract
    {
        private Task Updater;
        public MVGContract()
        {
            ApiUrl = "http://mobil.mvg-mainz.de/stationen-karte.html?type=1296727025&tx_mvgmeinrad_mvgmeinradstationenfull%5Baction%5D=getStationsAjax&tx_mvgmeinrad_mvgmeinradstationenfull%5Bcontroller%5D=Benutzer";
            DirectDownloadAvailability = true;
            this.ServiceProvider = "MVG Mainzer Verkehrsgesellschaft";
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
                        HttpResponseMessage response = await httpClient.GetAsync(new Uri(string.Format(ApiUrl)));
                        var responseBodyAsText = await response.Content.ReadAsStringAsync();
                        var model = responseBodyAsText.FromJsonString<List<MVGModel>>();
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
            var model = responseBodyAsText.FromJsonString<List<MVGModel>>();
            Velibs = new List<VelibModel>(model.Count);

            foreach (var station in model)
            {
                var stationModel = new VelibModel()
                {
                    Contract = this,
                    Number = station.Id,
                    Name = station.Label,
                    AvailableBikes = station.AvailableBikes,
                    AvailableBikeStands = station.AvailableDocks,
                    Locked = station.Locked,
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
            return (MVGContract)base.GetSimpleContract();
        }

        protected override Contract GetInstanceForSimpleClone()
        {
            return new MVGContract();
        }
    }
}

