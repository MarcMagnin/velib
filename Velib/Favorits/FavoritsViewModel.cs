using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Velib.Common;
using Windows.Storage;

namespace Velib.Favorits
{
    public class FavoritsViewModel
    {
        public static ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        public static ObservableCollection<Favorite> Favorits = new ObservableCollection<Favorite>();

        public FavoritsViewModel()
        {
            if (Windows.Storage.ApplicationData.Current.LocalSettings.Values["Favorits"] != null)
            {
                Favorits = (Windows.Storage.ApplicationData.Current.LocalSettings.Values["Favorits"] as String).FromJsonString<ObservableCollection<Favorite>>();
            }
        }



        public static void AddFavorit(Favorite item)
        {
            if (Windows.Storage.ApplicationData.Current.LocalSettings.Values["Favorits"] != null)
            {
                Favorits = (Windows.Storage.ApplicationData.Current.LocalSettings.Values["Favorits"] as String).FromJsonString<ObservableCollection<Favorite>>();
            }
            Favorits.Add(item);
            Windows.Storage.ApplicationData.Current.LocalSettings.Values["Favorits"] = Favorits.ToJson();
        }
        public static void RemoveFavorites(List<Favorite> items)
        {
            foreach(var item in items)
                Favorits.Remove(item);
            Windows.Storage.ApplicationData.Current.LocalSettings.Values["Favorits"] = Favorits.ToJson();
        }


    }
}
