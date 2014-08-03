using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Web.Http;
using Velib.Common;
using Windows.Devices.Geolocation;
using System.Reactive.Linq;
using Windows.Storage;
using Velib;
using Windows.UI.Core;
using System.Diagnostics;

namespace VelibContext
{
    static class VelibDataSource
    {
        //private HttpClient httpClient;
        //private CancellationTokenSource cts;
        private static Uri dataURL = new Uri("https://api.jcdecaux.com/vls/v1/stations?contract=Paris&apiKey=c3ae49d442f47c94ccfdb032328be969febe06ed");

        static VelibDataSource()
         {
             Task.Run(
               async () =>
               {
                   await ContractsViewModel.GetContractsFromHardDrive();
                    if(MainPage.mainPage != null)
                       MainPage.mainPage.DataSourceLoaded();
                       
               });
    
        }

        //private ObservableCollection<VelibModel> _velibs = new ObservableCollection<VelibModel>();
        //public ObservableCollection<VelibModel> Velibs
        //{
        //    get { return this._velibs; }
        //}


        //public static async Task<ObservableCollection<VelibModel>> GetEventsAsync()
        //{
        //    await _dataSource.GetDataAsync();

        //    return _dataSource.Velibs;
        //}

        private static async Task GetDataAsync()
        {

           var httpClient = new HttpClient();
          var  cts = new CancellationTokenSource();

            //Helpers.ScenarioStarted(StartButton, CancelButton, OutputField);
            //rootPage.NotifyUser("In progress", NotifyType.StatusMessage);
            bool failed = true;
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(dataURL).AsTask(cts.Token);

                var responseBodyAsText = await response.Content.ReadAsStringAsync().AsTask(cts.Token);
                var rootNode = responseBodyAsText.FromJsonString<List<VelibModel>>();
                foreach (var evt in rootNode)
                {

                    evt.Location = new Windows.Devices.Geolocation.Geopoint(new BasicGeoposition() { Latitude = evt.Position.Latitude, Longitude = evt.Position.Longitude });
                    //evt.AvailableBikesStr = evt.AvailableBikes.ToString();
                    //Velibs.Add(evt);
                }
                httpClient.Dispose();
                cts.Token.ThrowIfCancellationRequested();
            }
            catch (TaskCanceledException)
            {

            }
            catch (Exception ex)
            {
                failed = true;
            }
            finally
            {
                //  Helpers.ScenarioCompleted(StartButton, CancelButton);
            }
            if (failed)
            {
                // load local sample data
                try
                {
                    //Uri dataUri = new Uri("ms-appx:///DataSample/Events.json");
                    //StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(dataUri);
                    //string jsonText = await FileIO.ReadTextAsync(file);
                    //var rootNode = jsonText.FromJsonString<EventList>();
                    //foreach (var evt in rootNode.Events)
                    //{
                    //    evt.Location = new Windows.Devices.Geolocation.Geopoint(new BasicGeoposition() { Latitude = evt.Latitude, Longitude = evt.Longitude });
                    //    Events.Add(evt);
                    //}
                }
                catch (Exception eee) { }
            }
        }

        public static List<VelibModel> StaticVelibs = new List<VelibModel>();




        public static IObservable<List<VelibModel>> ObservableVelibs(int id)
        {
        return Observable.Create<List<VelibModel>>(async (observer, token) =>
        {
             // no exception handling required.  If this method throws,
             // Rx will catch it and call observer.OnError() for us.
            using (var httpClient = new HttpClient())
             {
                var cts = new CancellationTokenSource();

                //Helpers.ScenarioStarted(StartButton, CancelButton, OutputField);
                //rootPage.NotifyUser("In progress", NotifyType.StatusMessage);
                bool failed = true;
                HttpResponseMessage response = await httpClient.GetAsync(dataURL).AsTask(cts.Token);

                var responseBodyAsText = await response.Content.ReadAsStringAsync().AsTask(cts.Token);
                var rootNode = responseBodyAsText.FromJsonString<List<VelibModel>>();
                var velibs =new List<VelibModel>();
                foreach (var evt in rootNode)
                {
                
                    evt.Location = new Windows.Devices.Geolocation.Geopoint(new BasicGeoposition() { Latitude = evt.Position.Latitude, Longitude = evt.Position.Longitude});
                    //evt.AvailableBikesStr = evt.AvailableBikes.ToString();
                    velibs.Add(evt);
                }
                cts.Token.ThrowIfCancellationRequested();


                if (token.IsCancellationRequested) { return; }
                    observer.OnNext(velibs);
                 observer.OnCompleted();
             }
        });
    }   

    }
}

