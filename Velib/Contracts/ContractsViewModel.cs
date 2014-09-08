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
               new PubliBikeContract{Name = "SwissBikes",
               PaysImage = paysImagesRootPath+ "/CH.png",
               Pays = "Switzerland"},
              new NextBikeContract{Name = "Luzern",
               PaysImage = paysImagesRootPath+ "/CH.png",
               Pays = "Switzerland", Id= "126"},
                new NextBikeContract{Name = "Sursee",
               PaysImage = paysImagesRootPath+ "/CH.png",
               Pays = "Switzerland", Id= "88"},
            new CapitalBikeShareContract{Name = "Washington",
               PaysImage = paysImagesRootPath+ "/US.png",
               Pays = "United States"},
             new DivyBikeContract{Name = "Chicago",
               PaysImage = paysImagesRootPath+ "/US.png",
               Pays = "United States"},
            new CitiBikeContract{Name = "New York",
               PaysImage = paysImagesRootPath+ "/US.png",
               Pays = "United States"},
             new ContractTFLLondon{Name = "London",
               PaysImage = paysImagesRootPath+ "/UK.png",
               Pays = "United Kingdom"},
            new ContractJCDecauxVelib{Name = "Bruxelles-Capitale",
               PaysImage = paysImagesRootPath+ "/BE.png",
               Pays = "Belgium"},
               new ContractJCDecauxVelib{Name = "Namur",
               PaysImage = paysImagesRootPath+ "/BE.png",
               Pays = "Belgium"},
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
