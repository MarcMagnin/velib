using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.Web.Http;
using Velib.Common;
using VelibContext;
using Windows.UI.Popups;
using System.Runtime.Serialization.Json;
using Windows.Storage.Streams;
using Windows.Devices.Geolocation;
using Velib.Contracts;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Core;
using Velib.Contracts.Models.US;
using Velib.Contracts.Models.US.Washington;
using Velib.Contracts.Models.CH.PubliBike;
using Velib.Contracts.Models.NextBike;
using Velib.Contracts.Models.Smoove;
using Velib.Contracts.Models.BCycle;
using Velib.Contracts.Models.Bixi;
using Velib.Contracts.Models.c_bike;
namespace Velib
{
    public class ContractsViewModel
    {
        public static  ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        //static  Windows.Storage.StorageFolder installedLocation = Windows.ApplicationModel.Package.Current.InstalledLocation;
        static Windows.Storage.StorageFolder installedLocation = ApplicationData.Current.LocalFolder;
        
        private static string paysImagesRootPath = "ms-appx:///Assets/Pays";
        private static List<string> downloadedContract;
        private static List<Contract> contracts = new List<Contract>()
        {
            
          #region AU
          new BixxiContract {Name = "Melbourne",
               ServiceProvider="Melbourne Bike Share, Bixi",
               PaysImage = paysImagesRootPath+ "/AU.png",
               Pays = "Australia"},
          #endregion

          #region AT
          new NextBikeContract{Name = "Haag",
               PaysImage = paysImagesRootPath+ "/AT.png",
               Pays = "Austria", Id= "167"},
               new NextBikeContract{Name = "Hollabrunn",
               PaysImage = paysImagesRootPath+ "/AT.png",
               Pays = "Austria", Id= "212"},
               new NextBikeContract{Name = "Innsbruck",
               PaysImage = paysImagesRootPath+ "/AT.png",
               Pays = "Austria", Id= "199"},
               new NextBikeContract{Name = "Krems Ost",
               PaysImage = paysImagesRootPath+ "/AT.png",
               Pays = "Austria", Id= "164"},
               new NextBikeContract{Name = "Laa an der Thaya",
               PaysImage = paysImagesRootPath+ "/AT.png",
               Pays = "Austria", Id= "184"},
               new NextBikeContract{Name = "Marchfeld",
               PaysImage = paysImagesRootPath+ "/AT.png",
               Pays = "Austria", Id= "170"},
               new NextBikeContract{Name = "Mistelbach",
               PaysImage = paysImagesRootPath+ "/AT.png",
               Pays = "Austria", Id= "169"},
               new NextBikeContract{Name = "Mödling",
               PaysImage = paysImagesRootPath+ "/AT.png",
               Pays = "Austria", Id= "64"},
               new NextBikeContract{Name = "Neunkirchen",
               PaysImage = paysImagesRootPath+ "/AT.png",
               Pays = "Austria", Id= "163"},
               new NextBikeContract{Name = "NeusiedlerSee",
               PaysImage = paysImagesRootPath+ "/AT.png",
               Pays = "Austria", Id= "23"},
               new NextBikeContract{Name = "Oberes Ybbstal",
               PaysImage = paysImagesRootPath+ "/AT.png",
               Pays = "Austria", Id= "181"},
               new NextBikeContract{Name = "ÖBB-Bahnhöfe",
               PaysImage = paysImagesRootPath+ "/AT.png",
               Pays = "Austria", Id= "150"},
               new NextBikeContract{Name = "Piestingtal",
               PaysImage = paysImagesRootPath+ "/AT.png",
               Pays = "Austria", Id= "168"},
               new NextBikeContract{Name = "Römerland",
               PaysImage = paysImagesRootPath+ "/AT.png",
               Pays = "Austria", Id= "149"},
               new NextBikeContract{Name = "Sankt Pölten",
               TechnicalName= "St.Pölten",
               PaysImage = paysImagesRootPath+ "/AT.png",
               Pays = "Austria", Id= "57"},
               new NextBikeContract{Name = "Südheide",
               PaysImage = paysImagesRootPath+ "/AT.png",
               Pays = "Austria", Id= "185"},
               new NextBikeContract{Name = "Triestingtal",
               PaysImage = paysImagesRootPath+ "/AT.png",
               Pays = "Austria", Id= "144"},
               new NextBikeContract{Name = "Tulln an der Donau",
               TechnicalName= "Tulln",
               PaysImage = paysImagesRootPath+ "/AT.png",
               Pays = "Austria", Id= "143"},
               new NextBikeContract{Name = "Tullnerfeld West",
               PaysImage = paysImagesRootPath+ "/AT.png",
               Pays = "Austria", Id= "196"},
               new NextBikeContract{Name = "Thermenregion",
               PaysImage = paysImagesRootPath+ "/AT.png",
               Pays = "Austria", Id= "146"},
               new NextBikeContract{Name = "Traisen-Gölsental",
               PaysImage = paysImagesRootPath+ "/AT.png",
               Pays = "Austria", Id= "180"},
               new NextBikeContract{Name = "Unteres Traisental",
               PaysImage = paysImagesRootPath+ "/AT.png",
               Pays = "Austria", Id= "165"},
               new NextBikeContract{Name = "Wachau",
               PaysImage = paysImagesRootPath+ "/AT.png",
               Pays = "Austria", Id= "142"},
               new NextBikeContract{Name = "10 vor Wien",
               TechnicalName= "10vorWien",
               PaysImage = paysImagesRootPath+ "/AT.png",
               Pays = "Austria", Id= "174"},
               new NextBikeContract{Name = "WienerWald",
               PaysImage = paysImagesRootPath+ "/AT.png",
               Pays = "Austria", Id= "213"},
               new NextBikeContract{Name = "Wiener Neustadt",
               TechnicalName= "Wr.Neustadt",
               PaysImage = paysImagesRootPath+ "/AT.png",
               Pays = "Austria", Id= "156"},
               new NextBikeContract{Name = "Wieselburg",
               PaysImage = paysImagesRootPath+ "/AT.png",
               Pays = "Austria", Id= "151"},
          #endregion

          #region AZ
          new NextBikeContract{Name = "Baku",
               PaysImage = paysImagesRootPath+ "/AZ.png",
               Pays = "Azerbaijan", Id= "205"},
          #endregion

          #region BE
          new ContractJCDecauxVelib{Name = "Bruxelles-Capitale",
               PaysImage = paysImagesRootPath+ "/BE.png",
               Pays = "Belgium"},
               new ContractJCDecauxVelib{Name = "Namur",
               PaysImage = paysImagesRootPath+ "/BE.png",
               Pays = "Belgium"},
          #endregion

          #region BG
          new NextBikeContract{Name = "Dobrich",
               PaysImage = paysImagesRootPath+ "/BG.png",
               Pays = "Bulgaria", Id= "215"},
          #endregion

          #region CA
          new CapitalBikeShareContract{Name = "Montréal",
               ApiUrl = "https://montreal.bixi.com/data/bikeStations.xml",
               ServiceProvider = "Bixi Montreal, Bixi",
               PaysImage = paysImagesRootPath+ "/CA.png",
               Pays = "Canada"},
          #endregion CA

          #region CL
          new BCycleContract{Name = "Santiago",
               ServiceProvider= "Bikesantiago, B-cycle",
               PaysImage = paysImagesRootPath+ "/CL.png",
               Pays = "Chile", Id= "68"},
          #endregion

          #region HR
          new NextBikeContract{Name = "Šibenik",
               PaysImage = paysImagesRootPath+ "/HR.png",
               Pays = "Croatia", Id= "248"},
               new NextBikeContract{Name = "Zagreb",
               PaysImage = paysImagesRootPath+ "/HR.png",
               Pays = "Croatia", Id= "220"},
          #endregion

          #region CY
          new NextBikeContract{Name = "Limassol",
               PaysImage = paysImagesRootPath+ "/CY.png",
               Pays = "Cyprus", Id= "190"},
          #endregion

          #region FR
          new ContractJCDecauxVelib{Name = "Amiens",
               ServiceProvider="Vélam', JCDecaux",
               PaysImage = paysImagesRootPath+ "/FR.png",
               Pays = "France"},
               new ContractJCDecauxVelib{Name = "Besancon",
               ServiceProvider="VéloCité, JCDecaux",
               PaysImage = paysImagesRootPath+ "/FR.png",
               Pays = "France"},
               new ContractJCDecauxVelib{Name = "Cergy-Pontoise",
               ServiceProvider="VélO2, JCDecaux",
               PaysImage = paysImagesRootPath+ "/FR.png",
               Pays = "France"},
               new ContractJCDecauxVelib{Name = "Creteil",
               ServiceProvider="Cristolib', JCDecaux",
               PaysImage = paysImagesRootPath+ "/FR.png",
               Pays = "France"},
          new SmooveContract{Name = "Grenoble",
              ServiceProvider="Métrovélo, Smoove",
                ApiUrl = "http://vms.metrovelo.fr/vcstations.xml",
               PaysImage = paysImagesRootPath+ "/FR.png",
               Pays = "France"},    
          new ContractJCDecauxVelib{Name = "Lyon",
               ServiceProvider="Vélo'V, JCDecaux",
               PaysImage = paysImagesRootPath+ "/FR.png",
               Pays = "France"},
               new ContractJCDecauxVelib{Name = "Marseille",
               ServiceProvider="Le vélo, JCDecaux",
               PaysImage = paysImagesRootPath+ "/FR.png",
               Pays = "France"},
               new ContractJCDecauxVelib{Name = "Mulhouse",
               ServiceProvider="Vélocité, JCDecaux",
               PaysImage = paysImagesRootPath+ "/FR.png",
               Pays = "France"},
               new ContractJCDecauxVelib{Name = "Nancy",
               ServiceProvider="VélOstan', JCDecaux",
               PaysImage = paysImagesRootPath+ "/FR.png",
               Pays = "France"},
               new ContractJCDecauxVelib{Name = "Nantes",
               ServiceProvider="Bicloo', JCDecaux",
               PaysImage = paysImagesRootPath+ "/FR.png",
               Pays = "France"},
               new ContractJCDecauxVelib{Name = "Paris", 
               ServiceProvider="Vélib', JCDecaux",
               PaysImage = paysImagesRootPath+ "/FR.png",
               Pays = "France"},
               new ContractJCDecauxVelib{Name = "Rouen",
               ServiceProvider="Cy'clic', JCDecaux",
               PaysImage = paysImagesRootPath+ "/FR.png",
               Pays = "France"},
          new SmooveContract{Name = "Strasbourg",
               ServiceProvider="Vélhop', Smoove",
                ApiUrl = "http://www.velhop.strasbourg.eu/vcstations.xml",
               PaysImage = paysImagesRootPath+ "/FR.png",
               Pays = "France"},
          new ContractJCDecauxVelib{Name = "Toulouse",
               ServiceProvider="VélôToulouse', JCDecaux",
               PaysImage = paysImagesRootPath+ "/FR.png",
               Pays = "France"},
          #endregion

          #region DE
          new NextBikeContract{Name = "Augsburg",
               PaysImage = paysImagesRootPath+ "/DE.png",
               Pays = "Germany", Id= "178"},
               new NextBikeContract{Name = "Berlin",
               PaysImage = paysImagesRootPath+ "/DE.png",
               Pays = "Germany", Id= "20"},
               new NextBikeContract{Name = "Bielefeld",
               PaysImage = paysImagesRootPath+ "/DE.png",
               Pays = "Germany", Id= "16"},
               new NextBikeContract{Name = "Bietigheim-Bissingen",
               PaysImage = paysImagesRootPath+ "/DE.png",
               Pays = "Germany", Id= "226"},
               new NextBikeContract{Name = "Bochum",
               PaysImage = paysImagesRootPath+ "/DE.png",
               Pays = "Germany", Id= "130"},
               new NextBikeContract{Name = "Bottrop",
               PaysImage = paysImagesRootPath+ "/DE.png",
               Pays = "Germany", Id= "131"},
               new NextBikeContract{Name = "Burghausen",
               PaysImage = paysImagesRootPath+ "/DE.png",
               Pays = "Germany", Id= "201"},
               new NextBikeContract{Name = "Dortmund",
               PaysImage = paysImagesRootPath+ "/DE.png",
               Pays = "Germany", Id= "129"},
               new NextBikeContract{Name = "Duisburg",
               PaysImage = paysImagesRootPath+ "/DE.png",
               Pays = "Germany", Id= "132"},
               new NextBikeContract{Name = "Düsseldorf",
               PaysImage = paysImagesRootPath+ "/DE.png",
               Pays = "Germany", Id= "50"},
               new NextBikeContract{Name = "Dresden",
               PaysImage = paysImagesRootPath+ "/DE.png",
               Pays = "Germany", Id= "2"},
               new NextBikeContract{Name = "Essen",
               PaysImage = paysImagesRootPath+ "/DE.png",
               Pays = "Germany", Id= "133"},
               new NextBikeContract{Name = "Flensburg",
               PaysImage = paysImagesRootPath+ "/DE.png",
               Pays = "Germany", Id= "147"},
               new NextBikeContract{Name = "Frankfurt",
               PaysImage = paysImagesRootPath+ "/DE.png",
               Pays = "Germany", Id= "8"},
               new NextBikeContract{Name = "Gelsenkirchen",
               PaysImage = paysImagesRootPath+ "/DE.png",
               Pays = "Germany", Id= "134"},
               new NextBikeContract{Name = "Gütersloh",
               PaysImage = paysImagesRootPath+ "/DE.png",
               Pays = "Germany", Id= "160"},
               new NextBikeContract{Name = "Hannover",
               PaysImage = paysImagesRootPath+ "/DE.png",
               Pays = "Germany", Id= "87"},
               new NextBikeContract{Name = "Hamburg",
               PaysImage = paysImagesRootPath+ "/DE.png",
               Pays = "Germany", Id= "43"},
               new NextBikeContract{Name = "Hamm",
               PaysImage = paysImagesRootPath+ "/DE.png",
               Pays = "Germany", Id= "135"},
               new NextBikeContract{Name = "Herne",
               PaysImage = paysImagesRootPath+ "/DE.png",
               Pays = "Germany", Id= "136"},
               new NextBikeContract{Name = "Karlsruhe",
               PaysImage = paysImagesRootPath+ "/DE.png",
               Pays = "Germany", Id= "21"},
               new NextBikeContract{Name = "Leipzig",
               PaysImage = paysImagesRootPath+ "/DE.png",
               Pays = "Germany", Id= "1"},
               new NextBikeContract{Name = "Magdeburg",
               PaysImage = paysImagesRootPath+ "/DE.png",
               Pays = "Germany", Id= "42"},
               new NextBikeContract{Name = "Mannheim",
               PaysImage = paysImagesRootPath+ "/DE.png",
               Pays = "Germany", Id= "195"},
               new NextBikeContract{Name = "Mülheim an der Ruhr",
               TechnicalName= "Mülheim a.d.R.",
               PaysImage = paysImagesRootPath+ "/DE.png",
               Pays = "Germany", Id= "137"},
               new NextBikeContract{Name = "München",
               PaysImage = paysImagesRootPath+ "/DE.png",
               Pays = "Germany", Id= "139"},
               new NextBikeContract{Name = "Norderstedt",
               PaysImage = paysImagesRootPath+ "/DE.png",
               Pays = "Germany", Id= "177"},
               new NextBikeContract{Name = "Nürnberg",
               PaysImage = paysImagesRootPath+ "/DE.png",
               Pays = "Germany", Id= "6"},
               new NextBikeContract{Name = "Oberhausen",
               PaysImage = paysImagesRootPath+ "/DE.png",
               Pays = "Germany", Id= "138"},
               new NextBikeContract{Name = "Offenbach am Main",
               PaysImage = paysImagesRootPath+ "/DE.png",
               Pays = "Germany", Id= "32"},
               new NextBikeContract{Name = "Offenburg",
               PaysImage = paysImagesRootPath+ "/DE.png",
               Pays = "Germany", Id= "155"},
               new NextBikeContract{Name = "Postdam",
               PaysImage = paysImagesRootPath+ "/DE.png",
               Pays = "Germany", Id= "158"},
               new NextBikeContract{Name = "Schwieberdingen",
               PaysImage = paysImagesRootPath+ "/DE.png",
               Pays = "Germany", Id= "253"},
               new NextBikeContract{Name = "Tübingen",
               PaysImage = paysImagesRootPath+ "/DE.png",
               Pays = "Germany", Id= "101"},
               new NextBikeContract{Name = "Usedom", //8 Stations côté Polonais Uznam,PL
               PaysImage = paysImagesRootPath+ "/DE.png",
               Pays = "Germany", Id= "176"},
          #endregion

          #region JP
          new ContractJCDecauxVelib{Name = "Toyama",
               PaysImage = paysImagesRootPath+ "/JP.png",
               Pays = "Japan"},
          #endregion

          #region LV
          new NextBikeContract{Name = "Jurmala",
               PaysImage = paysImagesRootPath+ "/LV.png",
               Pays = "Latvia", Id= "140"},
               new NextBikeContract{Name = "Riga",
               PaysImage = paysImagesRootPath+ "/LV.png",
               Pays = "Latvia", Id= "128"},
          #endregion

          #region LT
          new ContractJCDecauxVelib{Name = "Vilnius",
               PaysImage = paysImagesRootPath+ "/LT.png",
               Pays = "Lithuania"},
          #endregion

          #region LU
          new ContractJCDecauxVelib{Name = "Luxembourg",
               PaysImage = paysImagesRootPath+ "/LU.png",
               Pays = "Luxembourg"},
          #endregion

          #region NZ
          new NextBikeContract{Name = "Auckland",
               PaysImage = paysImagesRootPath+ "/NZ.png",
               Pays = "New Zealand", Id= "34"},
               new NextBikeContract{Name = "Christchurch",
               PaysImage = paysImagesRootPath+ "/NZ.png",
               Pays = "New Zealand", Id= "193"},
          #endregion

          #region NO
          new ContractJCDecauxVelib{Name = "Lillestrom",
               PaysImage = paysImagesRootPath+ "/NO.png",
               Pays = "Norway"},
          #endregion

          #region PL
          new NextBikeContract{Name = "Bemowo",
               PaysImage = paysImagesRootPath+ "/PL.png",
               Pays = "Poland", Id= "197"},
               new NextBikeContract{Name = "Białystok",
               PaysImage = paysImagesRootPath+ "/PL.png",
               Pays = "Poland", Id= "245"},
               new NextBikeContract{Name = "Konstancin Jeziorna",
               PaysImage = paysImagesRootPath+ "/PL.png",
               Pays = "Poland", Id= "247"},
               new NextBikeContract{Name = "Kraków",
               PaysImage = paysImagesRootPath+ "/PL.png",
               Pays = "Poland", Id= "232"},
               new NextBikeContract{Name = "Lublin",
               PaysImage = paysImagesRootPath+ "/PL.png",
               Pays = "Poland", Id= "251"},
               new NextBikeContract{Name = "Opole",
               PaysImage = paysImagesRootPath+ "/PL.png",
               Pays = "Poland", Id= "202"},
               new NextBikeContract{Name = "Poznan",
               PaysImage = paysImagesRootPath+ "/PL.png",
               Pays = "Poland", Id= "192"},
               new NextBikeContract{Name = "Sopot",
               PaysImage = paysImagesRootPath+ "/PL.png",
               Pays = "Poland", Id= "227"},
               new NextBikeContract{Name = "Warszawa", // Biggest one 199 stations
               PaysImage = paysImagesRootPath+ "/PL.png",
               Pays = "Poland", Id= "210"},
               new NextBikeContract{Name = "Wrocław",
               PaysImage = paysImagesRootPath+ "/PL.png",
               Pays = "Poland", Id= "148"},
               new NextBikeContract{Name = "Wroclaw", // Other Stations near & in
               TechnicalName= "WROCŁAW 61",
               PaysImage = paysImagesRootPath+ "/PL.png",
               Pays = "Poland", Id= "187"},
          #endregion

          #region RU
          new ContractJCDecauxVelib{Name = "Kazan",
               PaysImage = paysImagesRootPath+ "/RU.png",
               Pays = "Russia"},
          #endregion

          #region SE
          new ContractJCDecauxVelib{Name = "Goteborg",
               PaysImage = paysImagesRootPath+ "/SE.png",
               Pays = "Sweden"},
               new ContractJCDecauxVelib{Name = "Stockholm",
               PaysImage = paysImagesRootPath+ "/SE.png",
               Pays = "Sweden"},
          #endregion

          #region SI
          new ContractJCDecauxVelib{Name = "Ljubljana",
               PaysImage = paysImagesRootPath+ "/SI.png",
               Pays = "Slovenia"},
          #endregion

          #region ES
          new ContractJCDecauxVelib{Name = "Santander",
               PaysImage = paysImagesRootPath+ "/ES.png",
               Pays = "Spain"},
               new ContractJCDecauxVelib{Name = "Seville",
               PaysImage = paysImagesRootPath+ "/ES.png",
               Pays = "Spain"},
               new ContractJCDecauxVelib{Name = "Valence",
               PaysImage = paysImagesRootPath+ "/ES.png",
               Pays = "Spain"},
          #endregion

          #region TR
          new NextBikeContract{Name = "Konya",
               PaysImage = paysImagesRootPath+ "/TR.png",
               Pays = "Turkey", Id= "183"},
               new NextBikeContract{Name = "Seferihisar",
               PaysImage = paysImagesRootPath+ "/TR.png",
               Pays = "Turkey", Id= "249"},
          #endregion

          #region AE
          new NextBikeContract{Name = "Al Sharjah",
               PaysImage = paysImagesRootPath+ "/AE.png",
               Pays = "United Arab Emirates", Id= "233"},
               new NextBikeContract{Name = "Dubai",
               PaysImage = paysImagesRootPath+ "/AE.png",
               Pays = "United Arab Emirates", Id= "219"},
          #endregion

          #region GB
          new NextBikeContract{Name = "Bath",
               PaysImage = paysImagesRootPath+ "/GB.png",
               Pays = "United Kingdom", Id= "236"},
               new NextBikeContract{Name = "Glasgow",
               PaysImage = paysImagesRootPath+ "/GB.png",
               Pays = "United Kingdom", Id= "237"},
          new ContractTFLLondon{Name = "London",
               ServiceProvider = "Barclays Cycle Hire, Bixi",
               PaysImage = paysImagesRootPath+ "/GB.png",
               Pays = "United Kingdom"},
          new NextBikeContract{Name = "Stirling",
               PaysImage = paysImagesRootPath+ "/GB.png",
               Pays = "United Kingdom", Id= "243"},
          #endregion

          #region US

        new PhiladelphiaTempContract{Name = "Philadelphia, PA",
               ServiceProvider= "Under construction, available on April 2015",
               PaysImage = paysImagesRootPath+ "/US.png",
               Pays = "United States"},

		  new BCycleContract{Name = "Ann Arbor, MI",
               ServiceProvider= "ArborBike, B-cycle",
               PaysImage = paysImagesRootPath+ "/US.png",
               Pays = "United States", Id= "76"},
               new BCycleContract{Name = "Austin, TX",
               PaysImage = paysImagesRootPath+ "/US.png",
               Pays = "United States", Id= "72"},
               new BCycleContract{Name = "Battle Creek, MI",
               PaysImage = paysImagesRootPath+ "/US.png",
               Pays = "United States", Id= "71"},
          new CapitalBikeShareContract{Name = "Boston, MA",
               ApiUrl = "http://www.thehubway.com/data/stations/bikeStations.xml",
               ServiceProvider = "Hubway, Bixi",
               PaysImage = paysImagesRootPath+ "/US.png",
               Pays = "United States"}, 
               new BCycleContract{Name = "Boulder, CO",
               PaysImage = paysImagesRootPath+ "/US.png",
               Pays = "United States", Id= "54"},
               new BCycleContract{Name = "Broward County, FL",
               PaysImage = paysImagesRootPath+ "/US.png",
               Pays = "United States", Id= "53"},
               new BCycleContract{Name = "Charlotte, NC",
               PaysImage = paysImagesRootPath+ "/US.png",
               Pays = "United States", Id= "61"},
          new DivyBikeContract{Name = "Chicago, IL",
               TechnicalName= "Chicago",
               ServiceProvider = "Divvy, Bixi",
               PaysImage = paysImagesRootPath+ "/US.png",
               Pays = "United States"},
          new DivyBikeContract{Name = "San Francisco Bay Area, CA",
               ApiUrl = "http://www.bayareabikeshare.com/stations/json",
               ServiceProvider = "Bay Area Bike Share, Bixi",
               PaysImage = paysImagesRootPath+ "/US.png",
               Pays = "United States"},
               new DivyBikeContract{Name = "Chattanooga, TM",
               ApiUrl = "http://www.bikechattanooga.com/stations/json",
               ServiceProvider = "Bike Chattanooga, Bixi",
               PaysImage = paysImagesRootPath+ "/US.png",
               Pays = "United States"},
               new DivyBikeContract{Name = "Columbus, OH",
               ApiUrl = "http://cogobikeshare.com/stations/json",
               ServiceProvider = "CoGo Bike Share, Bixi",
               PaysImage = paysImagesRootPath+ "/US.png",
               Pays = "United States"},
          new BCycleContract{Name = "Cincinnati, OH",
               ServiceProvider= "Red Bike, B-cycle",
               PaysImage = paysImagesRootPath+ "/US.png",
               Pays = "United States", Id= "80"},
               new BCycleContract{Name = "Columbia County, GA",
               PaysImage = paysImagesRootPath+ "/US.png",
               Pays = "United States", Id= "74"},
               new BCycleContract{Name = "Milwaukee, WI",
               ServiceProvider= "Bublr Bikes, B-cycle",
               PaysImage = paysImagesRootPath+ "/US.png",
               Pays = "United States", Id= "70"},
          new CitiBikeContract{Name = "New York City, NY",
               TechnicalName= "New York",
               ServiceProvider= "Citi Bike, Bixi",
               PaysImage = paysImagesRootPath+ "/US.png",
               Pays = "United States"},


          new BCycleContract{Name = "Dallas Fair Park, TX",
               PaysImage = paysImagesRootPath+ "/US.png",
               Pays = "United States", Id= "82"},
               new BCycleContract{Name = "Denver, CO",
               PaysImage = paysImagesRootPath+ "/US.png",
               Pays = "United States", Id= "36"},
               new BCycleContract{Name = "Des Moines, IA",
               PaysImage = paysImagesRootPath+ "/US.png",
               Pays = "United States", Id= "45"},
               new BCycleContract{Name = "Denver Federal Center, CO",
               PaysImage = paysImagesRootPath+ "/US.png",
               Pays = "United States", Id= "60"},
               new BCycleContract{Name = "Fargo, ND",
               PaysImage = paysImagesRootPath+ "/US.png",
               Pays = "United States", Id= "81"},
               new BCycleContract{Name = "Fort Worth, TX",
               PaysImage = paysImagesRootPath+ "/US.png",
               Pays = "United States", Id= "67"},
               new BCycleContract{Name = "Salt Lake City, UT",
               ServiceProvider = "GREENbike, B-cycle",
               PaysImage = paysImagesRootPath+ "/US.png",
               Pays = "United States", Id= "66"},
               new BCycleContract{Name = "Greenville, SC",
               PaysImage = paysImagesRootPath+ "/US.png",
               Pays = "United States", Id= "65"},
               new BCycleContract{Name = "South San Francisco, CA",
               ServiceProvider= "gRide, B-cycle",
               PaysImage = paysImagesRootPath+ "/US.png",
               Pays = "United States", Id= "47"},
               new BCycleContract{Name = "Kailua, Honolulu County, HI",
               PaysImage = paysImagesRootPath+ "/US.png",
               Pays = "United States", Id= "49"},
               new BCycleContract{Name = "Houston, TX",
               PaysImage = paysImagesRootPath+ "/US.png",
               Pays = "United States", Id= "59"},
               new BCycleContract{Name = "Indianapolis, IN",
               ServiceProvider= "Indianna Pacers Bikeshare, B-cycle",
               PaysImage = paysImagesRootPath+ "/US.png",
               Pays = "United States", Id= "75"},
               new BCycleContract{Name = "Kansas City, MO",
               PaysImage = paysImagesRootPath+ "/US.png",
               Pays = "United States", Id= "62"},
               new BCycleContract{Name = "Madison, WI",
               PaysImage = paysImagesRootPath+ "/US.png",
               Pays = "United States", Id= "55"},
               new BCycleContract{Name = "Nashville, TN",
               PaysImage = paysImagesRootPath+ "/US.png",
               Pays = "United States", Id= "64"},
               new BCycleContract{Name = "Omaha, NE",
               PaysImage = paysImagesRootPath+ "/US.png",
               Pays = "United States", Id= "56"},
               new BCycleContract{Name = "Rapid City, SD",
               PaysImage = paysImagesRootPath+ "/US.png",
               Pays = "United States", Id= "79"},
               new BCycleContract{Name = "San Antonio, TX",
               PaysImage = paysImagesRootPath+ "/US.png",
               Pays = "United States", Id= "48"},
               new BCycleContract{Name = "Savannah, GA",
               ServiceProvider= "CAT Bike, B-cycle",
               PaysImage = paysImagesRootPath+ "/US.png",
               Pays = "United States", Id= "73"},
               new BCycleContract{Name = "Spartanburg, SC",
               PaysImage = paysImagesRootPath+ "/US.png",
               Pays = "United States", Id= "57"},
               new BCycleContract{Name = "Whippany, NJ",
               PaysImage = paysImagesRootPath+ "/US.png",
               Pays = "United States", Id= "77"},


            // Retrait du XML 19/09/2014 Pittsburgh Bike Share http://www.pghbikeshare.org
            //new NextBikeContract{Name = "Pittsburgh, PA",
            //   TechnicalName= "Pittsburgh",    
            //   ServiceProvider= "Pittsburgh Bike Share, NextBike"
            //   PaysImage = paysImagesRootPath+ "/US.png",
            //   Pays = "United States", Id= "254"},


          new CapitalBikeShareContract{Name = "Washington, D.C. area",
               TechnicalName= "Washington",
               ServiceProvider = "Capital BikeShare, Bixi",
               PaysImage = paysImagesRootPath+ "/US.png",
               Pays = "United States"}, 
          #endregion

          #region CH
          new PubliBikeContract{Name = "Aigle",
               ServiceProvider="Chablais, PubliBike",
               PaysImage = paysImagesRootPath+ "/CH.png",
               Pays = "Switzerland"},         
               new PubliBikeContract{Name = "Avenches",
               ServiceProvider="Les Lacs-Romont, PubliBike",
               PaysImage = paysImagesRootPath+ "/CH.png",
               Pays = "Switzerland"},
               new PubliBikeContract{Name = "Basel",
               PaysImage = paysImagesRootPath+ "/CH.png",
            Pays = "Switzerland"},
            new PubliBikeContract{Name = "Bern",
            PaysImage = paysImagesRootPath+ "/CH.png",
            Pays = "Switzerland"},
            new PubliBikeContract{Name = "Brig",
            PaysImage = paysImagesRootPath+ "/CH.png",
            Pays = "Switzerland"},
            new PubliBikeContract{Name = "Bulle",
            PaysImage = paysImagesRootPath+ "/CH.png",
            Pays = "Switzerland"},
            new PubliBikeContract{Name = "Chavannes-près-Renens",
            ServiceProvider="Lausanne-Morges, PubliBike",
            PaysImage = paysImagesRootPath+ "/CH.png",
            Pays = "Switzerland"},
            new PubliBikeContract{Name = "Cheyres",
            ServiceProvider="Les Lacs-Romont, PubliBike",
            PaysImage = paysImagesRootPath+ "/CH.png",
            Pays = "Switzerland"},
            new PubliBikeContract{Name = "Delémont",
            PaysImage = paysImagesRootPath+ "/CH.png",
            Pays = "Switzerland"},
            new PubliBikeContract{Name = "Divonne-les-Bains",
            ServiceProvider="La Côte, PubliBike",
            PaysImage = paysImagesRootPath+ "/CH.png",
            Pays = "Switzerland"},
            new PubliBikeContract{Name = "Ecublens",
            ServiceProvider="Lausanne-Morges, PubliBike",
            TechnicalName= "Ecublens PL4",
            PaysImage = paysImagesRootPath+ "/CH.png",
            Pays = "Switzerland"},
            new PubliBikeContract{Name = "Estavayer-le-Lac",
            ServiceProvider="Les Lacs-Romont, PubliBike",
            PaysImage = paysImagesRootPath+ "/CH.png",
            Pays = "Switzerland"},
            new PubliBikeContract{Name = "Frauenfeld",
            PaysImage = paysImagesRootPath+ "/CH.png",
            Pays = "Switzerland"},
            new PubliBikeContract{Name = "Fribourg",
            ServiceProvider="Agglo Fribourg, PubliBike",
            PaysImage = paysImagesRootPath+ "/CH.png",
            Pays = "Switzerland"},
            new PubliBikeContract{Name = "Gland",
            ServiceProvider="La Côte, PubliBike",
            PaysImage = paysImagesRootPath+ "/CH.png",
            Pays = "Switzerland"},
            new PubliBikeContract{Name = "Granges-Paccot",
            ServiceProvider="Agglo Fribourg, PubliBike",
            PaysImage = paysImagesRootPath+ "/CH.png",
            Pays = "Switzerland"},
            new PubliBikeContract{Name = "Kreuzlingen",
            PaysImage = paysImagesRootPath+ "/CH.png",
            Pays = "Switzerland"},
            new PubliBikeContract{Name = "La Tour-de-Peilz",
            ServiceProvider="Riviera, PubliBike",
            PaysImage = paysImagesRootPath+ "/CH.png",
            Pays = "Switzerland"},
            new PubliBikeContract{Name = "La Tour-de-Trême",
            ServiceProvider="Bulle, PubliBike",
            PaysImage = paysImagesRootPath+ "/CH.png",
            Pays = "Switzerland"},
            new PubliBikeContract{Name = "Lausanne",
            ServiceProvider="Lausanne-Morges, PubliBike",
            PaysImage = paysImagesRootPath+ "/CH.png",
            Pays = "Switzerland"},
            new PubliBikeContract{Name = "Lugano",
            PaysImage = paysImagesRootPath+ "/CH.png",
            Pays = "Switzerland"},
            new PubliBikeContract{Name = "Luzern",
            PaysImage = paysImagesRootPath+ "/CH.png",
            Pays = "Switzerland"},
            new NextBikeContract{Name = "Luzern",
            PaysImage = paysImagesRootPath+ "/CH.png",
            StorageName= "LuzernNextBike",
            Pays = "Switzerland", Id= "126"},
            new PubliBikeContract{Name = "Marly",
            ServiceProvider="Agglo Fribourg, PubliBike",
            PaysImage = paysImagesRootPath+ "/CH.png",
            Pays = "Switzerland"},
            new PubliBikeContract{Name = "Melide",
            ServiceProvider="Lugano, PubliBike",
            PaysImage = paysImagesRootPath+ "/CH.png",
            Pays = "Switzerland"},
            new PubliBikeContract{Name = "Monthey",
            ServiceProvider="Chablais, PubliBike",
            PaysImage = paysImagesRootPath+ "/CH.png",
            Pays = "Switzerland"},
            new PubliBikeContract{Name = "Morcote",
            ServiceProvider="Lugano, PubliBike",
            PaysImage = paysImagesRootPath+ "/CH.png",
            Pays = "Switzerland"},
            new PubliBikeContract{Name = "Morges",
            ServiceProvider="Lausanne-Morges, PubliBike",
            PaysImage = paysImagesRootPath+ "/CH.png",
            Pays = "Switzerland"},
            new PubliBikeContract{Name = "Murten Morat",
            ServiceProvider="Les Lacs-Romont, PubliBike",
            TechnicalName= "Murten/Morat",
            PaysImage = paysImagesRootPath+ "/CH.png",
            Pays = "Switzerland"},
            new PubliBikeContract{Name = "Nyon",
            ServiceProvider="La Côte, PubliBike",
            PaysImage = paysImagesRootPath+ "/CH.png",
            Pays = "Switzerland"},
            new PubliBikeContract{Name = "Payerne",
            ServiceProvider="Les Lacs-Romont, PubliBike",
            PaysImage = paysImagesRootPath+ "/CH.png",
            Pays = "Switzerland"},
            new PubliBikeContract{Name = "Pazzallo",
            ServiceProvider="Lugano, PubliBike",
            PaysImage = paysImagesRootPath+ "/CH.png",
            Pays = "Switzerland"},
            new PubliBikeContract{Name = "Prangins",
            ServiceProvider="La Côte, PubliBike",
            PaysImage = paysImagesRootPath+ "/CH.png",
            Pays = "Switzerland"},
            new PubliBikeContract{Name = "Préverenges",
            ServiceProvider="Lausanne-Morges, PubliBike",
            PaysImage = paysImagesRootPath+ "/CH.png",
            Pays = "Switzerland"},
            new PubliBikeContract{Name = "Prilly",
            ServiceProvider="Lausanne-Morges, PubliBike",
            PaysImage = paysImagesRootPath+ "/CH.png",
            Pays = "Switzerland"},
            new PubliBikeContract{Name = "Rapperswil",
            PaysImage = paysImagesRootPath+ "/CH.png",
            Pays = "Switzerland"},
            new PubliBikeContract{Name = "Renens",
            ServiceProvider="Lausanne-Morges, PubliBike",
            PaysImage = paysImagesRootPath+ "/CH.png",
            Pays = "Switzerland"},
            new PubliBikeContract{Name = "Romont",
            ServiceProvider="Les Lacs-Romont, PubliBike",
            PaysImage = paysImagesRootPath+ "/CH.png",
            Pays = "Switzerland"},
            new PubliBikeContract{Name = "Sion",
            PaysImage = paysImagesRootPath+ "/CH.png",
            Pays = "Switzerland"},
            new PubliBikeContract{Name = "Solothurn",
            PaysImage = paysImagesRootPath+ "/CH.png",
            Pays = "Switzerland"},
            new NextBikeContract{Name = "Sursee",
            PaysImage = paysImagesRootPath+ "/CH.png",
            Pays = "Switzerland", Id= "88"},
            new PubliBikeContract{Name = "Tesserete",
            ServiceProvider="Lugano, PubliBike",
            PaysImage = paysImagesRootPath+ "/CH.png",
            Pays = "Switzerland"},
            new PubliBikeContract{Name = "Tolochenaz",
            ServiceProvider="Lausanne-Morges, PubliBike",
            PaysImage = paysImagesRootPath+ "/CH.png",
            Pays = "Switzerland"},
            new PubliBikeContract{Name = "Vevey",
            ServiceProvider="Riviera, PubliBike",
            PaysImage = paysImagesRootPath+ "/CH.png",
            Pays = "Switzerland"},
            new PubliBikeContract{Name = "Villars-sur-Glâne",
            ServiceProvider="Agglo Fribourg, PubliBike",
            PaysImage = paysImagesRootPath+ "/CH.png",
            Pays = "Switzerland"},
            new PubliBikeContract{Name = "Winterthur",
            PaysImage = paysImagesRootPath+ "/CH.png",
            Pays = "Switzerland"},
            new PubliBikeContract{Name = "Yverdon-les-Bains",
            PaysImage = paysImagesRootPath+ "/CH.png",
            Pays = "Switzerland"},
            new PubliBikeContract{Name = "Zürich",
            PaysImage = paysImagesRootPath+ "/CH.png",
            Pays = "Switzerland"},
          #endregion CH

#region CN
             new CBikeContract{Name = "Kaohsiung",
               ServiceProvider="C-Bike",
               ApiUrl ="http://www.c-bike.com.tw/xml/stationlistopendata.aspx",
               PaysImage = paysImagesRootPath+ "/TW.png",
               Pays = "Taiwan"},         
#endregion

        };

        public static List<Contract> Contracts
        {
            get
            {
                return contracts;
            }
        }
    
        public static async void DownloadAndSaveContract(Contract contract)
        {
            
                await contract.DownloadContract();
                if (contract.Downloaded)
                {
                    StoreContractsInAppSetting(contract);
                    await writeJsonAsync(contract);
                }
            
        }


        public static List<string> DownloadedContracts { get{
            if (downloadedContract != null)
                return downloadedContract;
            var jsonContractNames = (localSettings.Values["DownloadedContracts"] as string);
            if (jsonContractNames != null)
                downloadedContract = jsonContractNames.FromJsonString<List<string>>();
            else
                downloadedContract = new List<string>();
            return
                downloadedContract;
        }
        }
        private static async Task writeJsonAsync(Contract contract)
        {
            localSettings.Values[contract.StorageName] = true;
            try
            {


                StorageFile file = await installedLocation.CreateFileAsync(contract.StorageName, CreationCollisionOption.ReplaceExisting);
                
            using (StorageStreamTransaction transaction = await file.OpenTransactedWriteAsync())
            {
                using (DataWriter dataWriter = new DataWriter(transaction.Stream))
                {
                    dataWriter.WriteString(contract.ToJson());
                    transaction.Stream.Size = await dataWriter.StoreAsync(); // reset stream size to override the file
                    await transaction.CommitAsync();
                }
            }
            }
            catch (Exception e)
            {
                var dialog = new MessageDialog("Unable to save city to you storage : "+ e.Message + e.InnerException+ e.ToString());
                dialog.ShowAsync();
            }
        }

        public static async Task<bool> GetContractsFromHardDrive()
        {
            var velibs = new List<VelibModel>();

            foreach (var contract in Contracts.Where(c => DownloadedContracts.Contains(c.StorageName)).ToList())
            {
                var loadedContract = await GetContractFromFile(contract);
                 Contracts[Contracts.IndexOf(contract)] = loadedContract;
                 if (loadedContract.Velibs != null)
                {
                    VelibDataSource.StaticVelibs.AddRange(loadedContract.Velibs);
                    if (loadedContract.DirectDownloadAvailability)
                    {
                        loadedContract.GetAvailableBikes(null, MainPage.dispatcher);
                    }
                }
                
            }
            return true;
        }

        private static async Task<Contract> GetContractFromFile(Contract contract)
        {
            if (localSettings.Values[contract.StorageName] == null)
                return contract;
            try {
                StorageFile file = await installedLocation.GetFileAsync(contract.StorageName);
                if (file != null) {
                    using (IRandomAccessStream readStream = await file.OpenAsync(FileAccessMode.Read))
                    {
                        using (DataReader dataReader = new DataReader(readStream))
                        {
                            UInt64 size = readStream.Size;
                            if (size <= UInt32.MaxValue)
                            {
                                UInt32 numBytesLoaded = await dataReader.LoadAsync((UInt32)size);

                                contract = dataReader.ReadString(numBytesLoaded).FromJsonString<Contract>(contract.GetType());
                                if (contract.Velibs != null)
                                {
                                    foreach (var velib in contract.Velibs)
                                    {
                                        velib.Location = new Windows.Devices.Geolocation.Geopoint(new BasicGeoposition()
                                        {
                                            Latitude = velib.Latitude,
                                            Longitude = velib.Longitude
                                        });
                                        velib.AvailableBikes = -1;
                                        velib.AvailableBikeStands = -1;
                                        velib.Contract = contract;
                                    }
                                }
                            }
                        }
                    }
                }
            }catch(Exception e){
    
            }
            return contract;
        }


        internal static async void RemoveContract(Contract contract)
        {
            try
            {
                localSettings.Values[contract.StorageName] = null;
                StorageFile file = await installedLocation.GetFileAsync(contract.StorageName);
                contract.Downloaded = false;
                // remove from static velibs
                VelibDataSource.StaticVelibs.RemoveAll(t=> contract.Velibs.Any(v=>v.Longitude == t.Longitude && v.Latitude == t.Latitude));
                contract.Velibs.Clear();
                await file.DeleteAsync();
                RemoveContractsInAppSetting(contract);
            }
            catch (Exception e)
            {
            }
        }

        private static void RemoveContractsInAppSetting(Contract contract)
        {
            DownloadedContracts.Remove(contract.StorageName);
            Windows.Storage.ApplicationData.Current.LocalSettings.Values["DownloadedContracts"] = DownloadedContracts.ToJson();
        }
        private static void StoreContractsInAppSetting(Contract contract)
        {
            if (!DownloadedContracts.Contains(contract.StorageName))
            {
                DownloadedContracts.Add(contract.StorageName);
            }
            Windows.Storage.ApplicationData.Current.LocalSettings.Values["DownloadedContracts"] = DownloadedContracts.ToJson();
        }

    }
}
