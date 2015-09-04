using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Velib.Common;
using VelibContext;
using Windows.Devices.Geolocation;
using Windows.UI.Core;
using Windows.Web.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Diagnostics;
using Windows.UI.Xaml.Media.Imaging;
using System.IO;

namespace Velib.Contracts.Models.China
{
    /// <summary>
    /// http://www.ibike668.com/
    /// http://www.ibike668.com/CaseMap.asp
    /// http://ws.uibike.com/cityjson.php
    /// </summary>
    public class PublicBicycleContract : Contract
    {
        public string AvailabilityUrl;

        public PublicBicycleContract()
        {
            this.ServiceProvider = "Public Bicycle";
        }

        public override async void GetAvailableBikes(VelibModel velibModel, CoreDispatcher dispatcher)
        {
            if (velibModel.DownloadingAvailability)
                return;
            velibModel.DownloadingAvailability = true;
            var httpClient = new HttpClient();
            try
            {
                HttpResponseMessage responseBikes = await httpClient.GetAsync(new Uri(string.Format(AvailabilityUrl, velibModel.Number, 1) + "&t=" + Guid.NewGuid()));
                HttpResponseMessage responseStands = await httpClient.GetAsync(new Uri(string.Format(AvailabilityUrl, velibModel.Number, 2) + "&t=" + Guid.NewGuid()));
                var responseBuffer = await responseBikes.Content.ReadAsBufferAsync();
                var responseDocks = await responseStands.Content.ReadAsBufferAsync();

                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    try
                    {
                        var bitmap = new BitmapImage();
                        var array = responseBuffer.ToArray();
                        using (var ms = new MemoryStream(array))
                        {
                            await bitmap.SetSourceAsync(ms.AsRandomAccessStream());
                            velibModel.ImageAvailable = bitmap;
                        }


                        bitmap = new BitmapImage();
                        using (var ms = new MemoryStream(responseDocks.ToArray()))
                        {
                            await bitmap.SetSourceAsync(ms.AsRandomAccessStream());
                            velibModel.ImageDocks = bitmap;
                        }

                        var control = velibModel.VelibControl;
                        if (control != null)
                        {
                            control.ShowVelibStation();
                        }

                        if (MainPage.BikeMode)
                        {
                            velibModel.ImageNumber = velibModel.ImageAvailable;
                        }
                        else
                        {
                            velibModel.ImageNumber = velibModel.ImageDocks;
                        }
                    }
                    catch
                    { //// ignored 
                    }

                });

                velibModel.Loaded = true;
            }
            catch (Exception)
            {
            }
            finally
            {
                velibModel.DownloadingAvailability = false;
                httpClient.Dispose();
            }
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
                            AvailableBikes = -1,
                            AvailableBikeStands = -1,
                            Location = new Windows.Devices.Geolocation.Geopoint(new BasicGeoposition()
                            {
                                Latitude = station.Latitude,
                                Longitude = station.Longitude
                            }),
                            Latitude = station.Latitude,
                            Longitude = station.Longitude,
                        };

                        Velibs.Add(stationModel);
                    }
                }
            }

        }

        public override Contract GetSimpleContract()
        {
            return (PublicBicycleContract)base.GetSimpleContract();
        }

        protected override Contract GetInstanceForSimpleClone()
        {
            return new PublicBicycleContract();
        }
    }
}
