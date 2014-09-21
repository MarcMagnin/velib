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
                if (MainPage.mainPage != null)
                   MainPage.mainPage.DataSourceLoaded();

            });

        }

        public static List<VelibModel> StaticVelibs = new List<VelibModel>();
    }
}

