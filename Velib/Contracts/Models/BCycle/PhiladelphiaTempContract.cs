using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Velib.Common;
using VelibContext;
using Windows.Devices.Geolocation;
using Windows.UI.Core;
using Windows.Web.Http;


namespace Velib.Contracts.Models.BCycle
{
     public class PhiladelphiaTempContract: Contract
    {
        [IgnoreDataMember]
        private Task Updater;
        public PhiladelphiaTempContract()
        {
            this.ServiceProvider = "B-cycle";
            DirectDownloadAvailability = true;
            ApiUrl = "http://www.phillybikeshare.com/api/places?format=json";
        }

        public override async void GetAvailableBikes(VelibModel unused, CoreDispatcher dispatcher)
        {
            // TODO on april 2015
        }

        public override async Task InnerDownloadContract()
        {
            HttpResponseMessage response = await downloadContractHttpClient.GetAsync(new Uri(ApiUrl));
            var responseBodyAsText = await response.Content.ReadAsStringAsync();
            // require Velib.Common
            var model = responseBodyAsText.FromJsonString<PhiladelphiaTempModel>();

            Velibs = new List<VelibModel>();
            //this.LastUpdate = tflModel.lastUpdate;
            foreach (var station in model.Features)
            {
                var stationModel = new VelibModel()
                {
                    Contract = this,
                    Number = station.Id,
                    Name = station.Properties.LocationName,
                    Location = new Windows.Devices.Geolocation.Geopoint(new BasicGeoposition()
                    {
                        Latitude = station.Geometry.Location[1],
                        Longitude = station.Geometry.Location[0]
                    }),
                    Latitude = station.Geometry.Location[1],
                    Longitude = station.Geometry.Location[0],
                    Loaded = true
                };


                //if (MainPage.BikeMode)
                //    stationModel.AvailableStr = stationModel.AvailableBikes.ToString();
                //else
                //    stationModel.AvailableStr = stationModel.AvailableBikeStands.ToString();

                Velibs.Add(stationModel);
            }
        }




        public override Contract GetSimpleContract()
        {
            return (BCycleContract)base.GetSimpleContract();
        }

        protected override Contract GetInstanceForSimpleClone()
        {
            return new BCycleContract();
        }
    }
}

