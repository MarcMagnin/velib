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
using System.Globalization;

namespace Velib.Contracts.Models.Velo
{
    public class ShanghaiContract : Contract
    {
        private Task Updater;
        public ShanghaiContract()
        {
            ApiUrl = "http://self.chinarmb.com/FormStations.aspx";
            DirectDownloadAvailability = true;
            this.ServiceProvider = "Shanghai Forever Bicycle Rental (no availability)";
        }


        public override async void GetAvailableBikes(VelibModel unused, CoreDispatcher dispatcher)
        {

        }

        public override async Task InnerDownloadContract()
        {
            HttpResponseMessage response = await downloadContractHttpClient.GetAsync(new Uri(string.Format(ApiUrl)));
            var responseBodyAsText = await response.Content.ReadAsStringAsync();

            string pattern = @"(?<=new GLatLng\()([^\)]+)";
            if (Regex.IsMatch(responseBodyAsText, pattern))
            {
                Regex regex = new Regex(pattern, RegexOptions.None);

                Velibs = new List<VelibModel>();
                foreach (Match myMatch in regex.Matches(responseBodyAsText))
                {
                    if (myMatch.Success)
                    {
                        var values = myMatch.Captures[0].Value.Split(',');
                        double latitutde, longitude;
                        if (!double.TryParse(values[0].Trim(), NumberStyles.AllowDecimalPoint, new CultureInfo("en-US"), out latitutde))
                            continue;
                        double.TryParse(values[1].Trim(), NumberStyles.AllowDecimalPoint, new CultureInfo("en-US"), out longitude);

                        var stationModel = new VelibModel()
                        {
                            Contract = this,
                            Location = new Windows.Devices.Geolocation.Geopoint(new BasicGeoposition()
                            {
                                Latitude = latitutde,
                                Longitude = longitude 
                            }),
                            Latitude = latitutde,
                            Longitude = longitude,
                            //Loaded = true
                        };

                        stationModel.AvailableStr = "?";

                        Velibs.Add(stationModel);
                    }
                }
            }
        }
        public override Contract GetSimpleContract()
        {
            return (ShanghaiContract)base.GetSimpleContract();
        }

        protected override Contract GetInstanceForSimpleClone()
        {
            return new ShanghaiContract();
        }
    }
}

