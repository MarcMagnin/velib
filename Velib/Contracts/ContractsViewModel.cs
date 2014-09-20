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

          #region CL
          new BCycleContract{Name = "Santiago",
               TechnicalName= "Bikesantiago",
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
               PaysImage = paysImagesRootPath+ "/FR.png",
               Pays = "France"},
               new ContractJCDecauxVelib{Name = "Besancon",
               PaysImage = paysImagesRootPath+ "/FR.png",
               Pays = "France"},
               new ContractJCDecauxVelib{Name = "Cergy-Pontoise",
               PaysImage = paysImagesRootPath+ "/FR.png",
               Pays = "France"},
               new ContractJCDecauxVelib{Name = "Creteil",
               PaysImage = paysImagesRootPath+ "/FR.png",
               Pays = "France"},
          new SmooveContract{Name = "Grenoble",
                ApiUrl = "http://vms.metrovelo.fr/vcstations.xml",
               PaysImage = paysImagesRootPath+ "/FR.png",
               Pays = "France"},    
          new ContractJCDecauxVelib{Name = "Lyon",
               PaysImage = paysImagesRootPath+ "/FR.png",
               Pays = "France"},
               new ContractJCDecauxVelib{Name = "Marseille",
               PaysImage = paysImagesRootPath+ "/FR.png",
               Pays = "France"},
               new ContractJCDecauxVelib{Name = "Mulhouse",
               PaysImage = paysImagesRootPath+ "/FR.png",
               Pays = "France"},
               new ContractJCDecauxVelib{Name = "Nancy",
               PaysImage = paysImagesRootPath+ "/FR.png",
               Pays = "France"},
               new ContractJCDecauxVelib{Name = "Nantes",
               PaysImage = paysImagesRootPath+ "/FR.png",
               Pays = "France"},
               new ContractJCDecauxVelib{Name = "Paris", 
               PaysImage = paysImagesRootPath+ "/FR.png",
               Pays = "France"},
               new ContractJCDecauxVelib{Name = "Rouen",
               PaysImage = paysImagesRootPath+ "/FR.png",
               Pays = "France"},
          new SmooveContract{Name = "Strasbourg",
                ApiUrl = "http://www.velhop.strasbourg.eu/vcstations.xml",
               PaysImage = paysImagesRootPath+ "/FR.png",
               Pays = "France"},
          new ContractJCDecauxVelib{Name = "Toulouse",
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
		  new BCycleContract{Name = "Ann Arbor, MI",
               TechnicalName= "ArborBike",
               ServiceProvider= "ArborBike, B-cycle",
               PaysImage = paysImagesRootPath+ "/US.png",
               Pays = "United States", Id= "76"},
               new BCycleContract{Name = "Austin, TX",
               TechnicalName= "Austin",
               PaysImage = paysImagesRootPath+ "/US.png",
               Pays = "United States", Id= "72"},
               new BCycleContract{Name = "Battle Creek, MI",
               TechnicalName= "Battle Creek B-cycle",
               PaysImage = paysImagesRootPath+ "/US.png",
               Pays = "United States", Id= "71"},
               new BCycleContract{Name = "Boulder, CO",
               TechnicalName= "Boulder B-cycle",
               PaysImage = paysImagesRootPath+ "/US.png",
               Pays = "United States", Id= "54"},
               new BCycleContract{Name = "Broward County, FL",
               TechnicalName= "Broward B-cycle",
               PaysImage = paysImagesRootPath+ "/US.png",
               Pays = "United States", Id= "53"},
               new BCycleContract{Name = "Charlotte, NC",
               TechnicalName= "Charlotte B-cycle",
               PaysImage = paysImagesRootPath+ "/US.png",
               Pays = "United States", Id= "61"},
          new DivyBikeContract{Name = "Chicago, IL",
               TechnicalName= "Chicago",
               ServiceProvider = "Divvy, Bixi",
               PaysImage = paysImagesRootPath+ "/US.png",
               Pays = "United States"},
          new BCycleContract{Name = "Cincinnati, OH",
               TechnicalName= "Cincy Red Bike",
               ServiceProvider= "Red Bike, B-cycle",
               PaysImage = paysImagesRootPath+ "/US.png",
               Pays = "United States", Id= "80"},
               new BCycleContract{Name = "Columbia County, GA",
               TechnicalName= "Columbia County B-cycle",
               PaysImage = paysImagesRootPath+ "/US.png",
               Pays = "United States", Id= "74"},
               new BCycleContract{Name = "Milwaukee, WI",
               TechnicalName= "Bublr Bikes",
               ServiceProvider= "Bublr Bikes, B-cycle",
               PaysImage = paysImagesRootPath+ "/US.png",
               Pays = "United States", Id= "70"},
          new CitiBikeContract{Name = "New York City, NY",
               TechnicalName= "New York",
               ServiceProvider= "Citi Bike, Bixi",
               PaysImage = paysImagesRootPath+ "/US.png",
               Pays = "United States"},


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
               PaysImage = paysImagesRootPath+ "/CH.png",
               Pays = "Switzerland"},         
               new PubliBikeContract{Name = "Avenches",
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
               PaysImage = paysImagesRootPath+ "/CH.png",
               Pays = "Switzerland"},
               new PubliBikeContract{Name = "Delémont",
               PaysImage = paysImagesRootPath+ "/CH.png",
               Pays = "Switzerland"},
               new PubliBikeContract{Name = "Divonne-les-Bains",
               PaysImage = paysImagesRootPath+ "/CH.png",
               Pays = "Switzerland"},
               new PubliBikeContract{Name = "Ecublens",
               TechnicalName= "Ecublens PL4",
               PaysImage = paysImagesRootPath+ "/CH.png",
               Pays = "Switzerland"},
               new PubliBikeContract{Name = "Estavayer-le-Lac",
               PaysImage = paysImagesRootPath+ "/CH.png",
               Pays = "Switzerland"},
               new PubliBikeContract{Name = "Frauenfeld",
               PaysImage = paysImagesRootPath+ "/CH.png",
               Pays = "Switzerland"},
               new PubliBikeContract{Name = "Fribourg",
               PaysImage = paysImagesRootPath+ "/CH.png",
               Pays = "Switzerland"},
               new PubliBikeContract{Name = "Gland",
               PaysImage = paysImagesRootPath+ "/CH.png",
               Pays = "Switzerland"},
               new PubliBikeContract{Name = "Granges-Paccot",
               PaysImage = paysImagesRootPath+ "/CH.png",
               Pays = "Switzerland"},
               new PubliBikeContract{Name = "Kreuzlingen",
               PaysImage = paysImagesRootPath+ "/CH.png",
               Pays = "Switzerland"},
               new PubliBikeContract{Name = "La Tour-de-Peilz",
               PaysImage = paysImagesRootPath+ "/CH.png",
               Pays = "Switzerland"},
               new PubliBikeContract{Name = "La Tour-de-Trême",
               PaysImage = paysImagesRootPath+ "/CH.png",
               Pays = "Switzerland"},
               new PubliBikeContract{Name = "Lausanne",
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
               PaysImage = paysImagesRootPath+ "/CH.png",
               Pays = "Switzerland"},
               new PubliBikeContract{Name = "Melide",
               PaysImage = paysImagesRootPath+ "/CH.png",
               Pays = "Switzerland"},
               new PubliBikeContract{Name = "Monthey",
               PaysImage = paysImagesRootPath+ "/CH.png",
               Pays = "Switzerland"},
               new PubliBikeContract{Name = "Morcote",
               PaysImage = paysImagesRootPath+ "/CH.png",
               Pays = "Switzerland"},
               new PubliBikeContract{Name = "Morges",
               PaysImage = paysImagesRootPath+ "/CH.png",
               Pays = "Switzerland"},
	           new PubliBikeContract{Name = "Murten Morat",
               TechnicalName= "Murten/Morat",
               PaysImage = paysImagesRootPath+ "/CH.png",
               Pays = "Switzerland"},
               new PubliBikeContract{Name = "Nyon",
               PaysImage = paysImagesRootPath+ "/CH.png",
               Pays = "Switzerland"},
               new PubliBikeContract{Name = "Payerne",
               PaysImage = paysImagesRootPath+ "/CH.png",
               Pays = "Switzerland"},
               new PubliBikeContract{Name = "Pazzallo",
               PaysImage = paysImagesRootPath+ "/CH.png",
               Pays = "Switzerland"},
               new PubliBikeContract{Name = "Prangins",
               PaysImage = paysImagesRootPath+ "/CH.png",
               Pays = "Switzerland"},
               new PubliBikeContract{Name = "Préverenges",
               PaysImage = paysImagesRootPath+ "/CH.png",
               Pays = "Switzerland"},
               new PubliBikeContract{Name = "Prilly",
               PaysImage = paysImagesRootPath+ "/CH.png",
               Pays = "Switzerland"},
               new PubliBikeContract{Name = "Rapperswil",
               PaysImage = paysImagesRootPath+ "/CH.png",
               Pays = "Switzerland"},
               new PubliBikeContract{Name = "Renens",
               PaysImage = paysImagesRootPath+ "/CH.png",
               Pays = "Switzerland"},
               new PubliBikeContract{Name = "Romont",
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
               PaysImage = paysImagesRootPath+ "/CH.png",
               Pays = "Switzerland"},
               new PubliBikeContract{Name = "Tolochenaz",
               PaysImage = paysImagesRootPath+ "/CH.png",
               Pays = "Switzerland"},
               new PubliBikeContract{Name = "Vevey",
               PaysImage = paysImagesRootPath+ "/CH.png",
               Pays = "Switzerland"},
               new PubliBikeContract{Name = "Villars-sur-Glâne",
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
