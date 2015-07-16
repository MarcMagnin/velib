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
    public class DangtuContract : PublicBicycleContract
    {
        public DangtuContract()
        {
        }

        public override async Task InnerDownloadContract()
        {
            // get an id used to query the service
            HttpResponseMessage response = await downloadContractHttpClient.GetAsync(new Uri("http://218.93.33.59:85/map/maanshanmap/dangtuindex.asp"));
            var responseBodyAsText = await response.Content.ReadAsStringAsync();
            string pattern = @"(?<=ibikestation.asp\?)([^""]*)";
            if (Regex.IsMatch(responseBodyAsText, pattern))
            {
                var regex = new Regex(pattern).Match(responseBodyAsText);
                if (regex != null && regex.Captures.Count > 0)
                {
                    var id = regex.Captures[0].Value;
                    
                    response = await downloadContractHttpClient.GetAsync(new Uri(string.Format("{0}?{1}", ApiUrl, id)));
                    responseBodyAsText = await response.Content.ReadAsStringAsync();

                    pattern = @"(\[.*[\s\S]*?])";
                    if (Regex.IsMatch(responseBodyAsText, pattern))
                    {
                        regex = new Regex(pattern).Match(responseBodyAsText);
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
            }
        }

        public override Contract GetSimpleContract()
        {
            return (DangtuContract)base.GetSimpleContract();
        }

        protected override Contract GetInstanceForSimpleClone()
        {
            return new DangtuContract();
        }
    }
}
