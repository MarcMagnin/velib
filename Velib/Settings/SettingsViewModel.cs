using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Velib.Settings
{
    public class SettingsViewModel :ViewModelBase
    {

        public bool Localization
        {
            get {
                if (App.LocalSettings.Values["Localization"] == null)
                {
                    App.LocalSettings.Values["Localization"] = true;
                }

                return (bool)App.LocalSettings.Values["Localization"]; 
            
            }
            set {
                App.LocalSettings.Values["Localization"] = value;
                if (value == false)
                {
                    MainPage.mainPage.DisableLocalization();

                }
                else
                {
                    MainPage.mainPage.EnableLocalization();
                }
                RaisePropertyChanged();
            }
        }
        
    }
}
