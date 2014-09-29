using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using VelibContext;
using Windows.UI.Core;
using Windows.Web.Http;
using Velib.Common;
using Windows.Devices.Geolocation;
namespace Velib.Contracts.Models.CallABike
{
    public class CallABikeContract: Contract
    {

//        private string postContent = @"
//<?xml version='1.0' encoding='utf-8'?>
//<SOAP-ENV:Envelope xmlns:SOAP-ENV='http://schemas.xmlsoap.org/soap/envelope/' xmlns:xsi='http://www.w3.org/1999/XMLSchema-instance' xmlns:xsd='http://www.w3.org/1999/XMLSchema'>
//<SOAP-ENV:Body>
//<CABSERVER.listFreeBikes xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xsd='http://www.w3.org/2001/XMLSchema' xmlns='https://xml.dbcarsharing-buchung.de/hal2_cabserver/'>
//<CommonParams xmlns=''>
//<UserData>
//<User>t_cab_windows</User>
//<Password>6#JbHarD7gKd</Password>
//</UserData>
//<LanguageUID>1</LanguageUID>
//<RequestTime>{0}</RequestTime>
//<Version>1.0</Version>
//</CommonParams>
//<SearchPosition xmlns=''>
//<Longitude>{1}</Longitude>
//<Latitude>{2}</Latitude>
//</SearchPosition>
//<maxResults xmlns=''>100</maxResults>
//<searchRadius xmlns=''>{3}</searchRadius>
//</CABSERVER.listFreeBikes>
//</SOAP-ENV:Body>
//</SOAP-ENV:Envelope>";

        [IgnoreDataMember]
        private Task Updater;
        public CallABikeContract()
        {
            DirectDownloadAvailability = true;
           // ApiUrl = "https://xml.dbcarsharing-buchung.de/hal2_cabserver/hal2_cabserver_3.php ";
            ApiUrl = "http://www.callabike-interaktiv.de/kundenbuchung/hal2ajax_process.php?mapstadt_id={0}&with_staedte=N&ajxmod=hal2map&callee=getMarker";
            this.ServiceProvider = "Call a Bike";
        //    postContent = postContent.Replace('\'', '"');
        }
        // Barclays refresh every 3 minutes the stations informations :/
        public override async void GetAvailableBikes(VelibModel unused, CoreDispatcher dispatcher)
        {
            //if (Updater != null)
            //    return;
            //Updater = new Task(async () =>
            //{
            //    while (true)
            //    {
            //        var httpClient = new HttpClient();
            //        try
            //        {
            //            var content = new HttpStringContent(postContent);
            //            HttpResponseMessage response = await httpClient.PostAsync(new Uri(string.Format(ApiUrl, Id)), content);
            //            var responseBodyAsText = await response.Content.ReadAsStringAsync();
            //            var model = responseBodyAsText.FromXmlString<markers>("");
            //            var city = model.Items.FirstOrDefault().city.FirstOrDefault();
            //            foreach (var station in city.place)
            //            {
            //                foreach (var velibModel in Velibs)
            //                {
            //                    if (velibModel.Latitude == station.lat && velibModel.Longitude == station.lng)
            //                    {
            //                        if (MainPage.BikeMode)
            //                        {
            //                            if((station.bikes.Contains("+") && velibModel.AvailableBikes != 5) || !station.bikes.Contains("+") && station.bikes !=  velibModel.AvailableBikes.ToString() )
            //                            velibModel.Reload = true;

            //                        }
            //                        if (!MainPage.BikeMode)
            //                        {
            //                            if((station.bike_racks.Contains("+") && velibModel.AvailableBikeStands!= 5) || !station.bike_racks.Contains("+") && station.bike_racks !=  velibModel.AvailableBikeStands.ToString() )
            //                            velibModel.Reload = true;

            //                        }
            //                        velibModel.AvailableBikes =  station.bikes.Contains('+') ? 5 : int.Parse(station.bikes);
            //                        velibModel.AvailableBikeStands =  station.bike_racks == null ? 0 : int.Parse(station.bike_racks);
            //                        velibModel.Loaded = true;
            //                        break;
            //                    }

            //                }
            //            }
            //            await dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
            //            {
            //                foreach (var station in Velibs.Where(t => t.Reload && t.VelibControl != null && t.VelibControl.Velibs.Count == 1))
            //                {
            //                    var control = station.VelibControl;
            //                    if (control != null)
            //                    {
            //                        control.ShowVelibStation();
            //                        control.ShowStationColor();
            //                    }
            //                    station.Reload = false;
            //                }

            //            });
                      
            //        }
            //        catch (Exception)
            //        {
            //        }
            //        finally
            //        {
            //            httpClient.Dispose();
            //        }
            //        await Task.Delay(RefreshTimer);
            //    }

            //});
            //Updater.Start();
        }

        public override async Task InnerDownloadContract()
        {

            //postContent = string.Format(postContent, DateTimeOffset.Now.ToString("o"),
            //   8.77430248,
            //   50.8015747,
            //   10000

            //   );
            //var content = new HttpStringContent(postContent.Trim());
            //content.Headers.ContentType = new Windows.Web.Http.Headers.HttpMediaTypeHeaderValue("text/xml;charset=\"utf-8\"");
            //downloadContractHttpClient.DefaultRequestHeaders.Add("SOAPAction", "https://xml.dbcarsharing-buchung.de/hal2_cabserver/CABSERVER.listFreeBikes");
            //downloadContractHttpClient.DefaultRequestHeaders.Accept.Add(new Windows.Web.Http.Headers.HttpMediaTypeWithQualityHeaderValue("text/xml"));
            //downloadContractHttpClient.DefaultRequestHeaders.AcceptEncoding.Add(new Windows.Web.Http.Headers.HttpContentCodingWithQualityHeaderValue("text/xml;charset=\"utf-8\""));
            HttpResponseMessage response = await downloadContractHttpClient.GetAsync(new Uri(string.Format(ApiUrl, Id)));
            var responseBodyAsText = await response.Content.ReadAsStringAsync();
            // require Velib.Common
            var model = responseBodyAsText.FromJsonString<CallABikeModel>().stations;
            Velibs = new List<VelibModel>();
            //this.LastUpdate = tflModel.lastUpdate;
            foreach (var station in model)
            {
                var stationModel = new VelibModel()
                {
                    Contract = this,
                    //Number = station.Id,
                    //Name = station.Label,
                    AvailableBikes = station.Details.Bikelist.Length,
                    //AvailableBikeStands = station.AvailableDocks,
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
            return (CallABikeContract)base.GetSimpleContract();
        }

        protected override Contract GetInstanceForSimpleClone()
        {
            return new CallABikeContract();
        }
    }
}

