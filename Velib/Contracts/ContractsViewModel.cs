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
namespace Velib
{
    public class ContractsViewModel
    {
        public static  ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        //static  Windows.Storage.StorageFolder installedLocation = Windows.ApplicationModel.Package.Current.InstalledLocation;
        static Windows.Storage.StorageFolder installedLocation = ApplicationData.Current.LocalFolder;
        



        private static string paysImagesRootPath = "ms-appx:///Assets/Pays";
        private static List<string> downloadedContract;
        private static List<Contract> contracts = new List<Contract>(){
// CH TOP            
            new PubliBikeContract{Name = "Agglo Fribourg",
               PaysImage = paysImagesRootPath+ "/CH.png",
               Pays = "Switzerland"}, 
               new PubliBikeContract{Name = "Bern", //name
               PaysImage = paysImagesRootPath+ "/CH.png",
               Pays = "Switzerland"},
               new PubliBikeContract{Name = "Lausanne", //city
               PaysImage = paysImagesRootPath+ "/CH.png",
               Pays = "Switzerland"},
               new PubliBikeContract{Name = "Murten Morat", // city
               TechnicalName= "Murten/Morat",
               PaysImage = paysImagesRootPath+ "/CH.png"},

               new PubliBikeContract{Name = "La Tour-de-Peilz", // city
               PaysImage = paysImagesRootPath+ "/CH.png",
               Pays = "Switzerland"},

               new PubliBikeContract{Name = "Ecublens", // name
               TechnicalName= "Ecublens PL4",
               PaysImage = paysImagesRootPath+ "/CH.png",
               Pays = "Switzerland"},
         
            new NextBikeContract{Name = "Luzern",
               PaysImage = paysImagesRootPath+ "/CH.png",
               Pays = "Switzerland", Id= "126"},
            new NextBikeContract{Name = "Sursee",
               PaysImage = paysImagesRootPath+ "/CH.png",
               Pays = "Switzerland", Id= "88"},

// CH BOTTOM
            new NextBikeContract{Name = "Luzern",
               PaysImage = paysImagesRootPath+ "/CH.png",
               Pays = "Switzerland", Id= "126"},
               new NextBikeContract{Name = "Sursee",
               PaysImage = paysImagesRootPath+ "/CH.png",
               Pays = "Switzerland", Id= "88"},
               new NextBikeContract{Name = "Pittsburgh",
               PaysImage = paysImagesRootPath+ "/US.png",
               Pays = "United States", Id= "254"},
            new CapitalBikeShareContract{Name = "Washington",
               PaysImage = paysImagesRootPath+ "/US.png",
               Pays = "United States"},
            new DivyBikeContract{Name = "Chicago",
               PaysImage = paysImagesRootPath+ "/US.png",
               Pays = "United States"},
            new CitiBikeContract{Name = "New York",
               PaysImage = paysImagesRootPath+ "/US.png",
               Pays = "United States"},
            new NextBikeContract{Name = "Dubai",
               PaysImage = paysImagesRootPath+ "/AE.png",
               Pays = "United Arab Emirates", Id= "219"},
               new NextBikeContract{Name = "Al Sharjah",
               PaysImage = paysImagesRootPath+ "/AE.png",
               Pays = "United Arab Emirates", Id= "233"},
               new NextBikeContract{Name = "Bath",
               PaysImage = paysImagesRootPath+ "/GB.png",
               Pays = "United Kingdom", Id= "236"},
               new NextBikeContract{Name = "Glasgow",
               PaysImage = paysImagesRootPath+ "/GB.png",
               Pays = "United Kingdom", Id= "237"},
            new ContractTFLLondon{Name = "London",
               PaysImage = paysImagesRootPath+ "/GB.png",
               Pays = "United Kingdom"},
            new NextBikeContract{Name = "Stirling",
               PaysImage = paysImagesRootPath+ "/GB.png",
               Pays = "United Kingdom", Id= "243"},
            new ContractJCDecauxVelib{Name = "Bruxelles-Capitale",
               PaysImage = paysImagesRootPath+ "/BE.png",
               Pays = "Belgium"},
               new ContractJCDecauxVelib{Name = "Namur",
               PaysImage = paysImagesRootPath+ "/BE.png",
               Pays = "Belgium"},
            new NextBikeContract{Name = "Limassol",
               PaysImage = paysImagesRootPath+ "/CY.png",
               Pays = "Cyprus", Id= "190"},
            new ContractJCDecauxVelib{Name = "Santander",
               PaysImage = paysImagesRootPath+ "/ES.png",
               Pays = "Spain"},
               new ContractJCDecauxVelib{Name = "Seville",
               PaysImage = paysImagesRootPath+ "/ES.png",
               Pays = "Spain"},
               new ContractJCDecauxVelib{Name = "Valence",
               PaysImage = paysImagesRootPath+ "/ES.png",
               Pays = "Spain"},
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
               new ContractJCDecauxVelib{Name = "Toulouse",
               PaysImage = paysImagesRootPath+ "/FR.png",
               Pays = "France"},
            new SmooveContract{Name = "Strasbourg",
               PaysImage = paysImagesRootPath+ "/FR.png",
               Pays = "France"},
            new NextBikeContract{Name = "Berlin",
               PaysImage = paysImagesRootPath+ "/DE.png",
               Pays = "Germany", Id= "20"},
               new NextBikeContract{Name = "Bielefeld",
               PaysImage = paysImagesRootPath+ "/DE.png",
               Pays = "Germany", Id= "16"},
               new NextBikeContract{Name = "Frankfurt",
               PaysImage = paysImagesRootPath+ "/DE.png",
               Pays = "Germany", Id= "8"},
               new NextBikeContract{Name = "Leipzig",
               PaysImage = paysImagesRootPath+ "/DE.png",
               Pays = "Germany", Id= "1"},
               new NextBikeContract{Name = "Offenbach am Main",
               PaysImage = paysImagesRootPath+ "/DE.png",
               Pays = "Germany", Id= "32"},
               new NextBikeContract{Name = "Tübingen",
               PaysImage = paysImagesRootPath+ "/DE.png",
               Pays = "Germany", Id= "101"},
            new ContractJCDecauxVelib{Name = "Toyama",
               PaysImage = paysImagesRootPath+ "/JP.png",
               Pays = "Japan"},
               new ContractJCDecauxVelib{Name = "Vilnius",
               PaysImage = paysImagesRootPath+ "/LT.png",
               Pays = "Lithuania"},
               new ContractJCDecauxVelib{Name = "Luxembourg",
               PaysImage = paysImagesRootPath+ "/LU.png",
               Pays = "Luxembourg"},
               new ContractJCDecauxVelib{Name = "Lillestrom",
               PaysImage = paysImagesRootPath+ "/NO.png",
               Pays = "Norway"},
               new ContractJCDecauxVelib{Name = "Kazan",
               PaysImage = paysImagesRootPath+ "/RU.png",
               Pays = "Russia"},
               new ContractJCDecauxVelib{Name = "Goteborg",
               PaysImage = paysImagesRootPath+ "/SE.png",
               Pays = "Sweden"},
               new ContractJCDecauxVelib{Name = "Stockholm",
               PaysImage = paysImagesRootPath+ "/SE.png",
               Pays = "Sweden"},
               new ContractJCDecauxVelib{Name = "Ljubljana",
               PaysImage = paysImagesRootPath+ "/SI.png",
               Pays = "Slovenia"},
            new NextBikeContract{Name = "Jurmala",
               PaysImage = paysImagesRootPath+ "/LV.png",
               Pays = "Latvia", Id= "140"},
               new NextBikeContract{Name = "Riga",
               PaysImage = paysImagesRootPath+ "/LV.png",
               Pays = "Latvia", Id= "128"},
               new NextBikeContract{Name = "Auckland",
               PaysImage = paysImagesRootPath+ "/NZ.png",
               Pays = "New Zealand", Id= "34"},
               new NextBikeContract{Name = "Christchurch",
               PaysImage = paysImagesRootPath+ "/NZ.png",
               Pays = "New Zealand", Id= "193"},
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
            localSettings.Values[contract.Name] = true;
            try
            {

           
            StorageFile file = await installedLocation.CreateFileAsync(contract.Name, CreationCollisionOption.ReplaceExisting);
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

            foreach (var contract in Contracts.Where(c=>DownloadedContracts.Contains( c.Name)).ToList())
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
            if (localSettings.Values[contract.Name] == null)
                return contract;
            try {
                StorageFile file = await installedLocation.GetFileAsync(contract.Name);
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

        //private static async Task DownloadContract(Contract contract){
        //    var httpClient = new HttpClient();
        //    var cts = new CancellationTokenSource();
        //    var velibs = new List<VelibModel>();
        //    contract.Downloading = true;
        //    bool failed = false;
        //    try
        //    {
        //        HttpResponseMessage response = await httpClient.GetAsync(new Uri(string.Format(villeUri, contract.Name))).AsTask(cts.Token);
        //        var responseBodyAsText = await response.Content.ReadAsStringAsync().AsTask(cts.Token);
        //        contract.Velibs = responseBodyAsText.FromJsonString<List<VelibModel>>();
        //        contract.VelibCounter = contract.Velibs.Count.ToString() + " cycles";
        //        foreach (var velib in contract.Velibs)
        //        {
        //            velib.Location = new Windows.Devices.Geolocation.Geopoint(new BasicGeoposition()
        //            {
        //                Latitude = velib.Latitude,
        //                Longitude = velib.Longitude
        //            });
        //            velib.AvailableBikes = -1;
        //            velib.AvailableBikeStands = -1;
        //        }
        //        contract.Downloaded = true;
        //        VelibDataSource.StaticVelibs.AddRange(contract.Velibs);
        //        httpClient.Dispose();
        //        cts.Token.ThrowIfCancellationRequested();
        //    }
        //    catch (TaskCanceledException)
        //    {
        //        failed = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        failed = true;
        //    }
        //    finally
        //    {
        //        contract.Downloading = false;
        //        //  Helpers.ScenarioCompleted(StartButton, CancelButton);
        //    }
        //    if (failed)
        //    {
        //        var dialog = new MessageDialog("Sorry, you are currently not able to download " + contract.Name);
        //        await dialog.ShowAsync();
        //    }
        //}

        internal static async void RemoveContract(Contract contract)
        {
            try
            {
                localSettings.Values[contract.Name] = null;
                StorageFile file = await installedLocation.GetFileAsync(contract.Name);
                contract.Downloaded = false;
                // remove from static velibs
                VelibDataSource.StaticVelibs.RemoveAll(t=> contract.Velibs.Any(v=>v.Number == t.Number));
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
            DownloadedContracts.Remove(contract.Name);
            Windows.Storage.ApplicationData.Current.LocalSettings.Values["DownloadedContracts"] = DownloadedContracts.ToJson();
        }
        private static void StoreContractsInAppSetting(Contract contract)
        {
            if (!DownloadedContracts.Contains(contract.Name))
            {
                DownloadedContracts.Add(contract.Name);
            }
            Windows.Storage.ApplicationData.Current.LocalSettings.Values["DownloadedContracts"] = DownloadedContracts.ToJson();
        }

    }
}
