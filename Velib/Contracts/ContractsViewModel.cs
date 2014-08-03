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

namespace Velib
{
    public class ContractsViewModel
    {
        public static  ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        static  Windows.Storage.StorageFolder installedLocation = Windows.ApplicationModel.Package.Current.InstalledLocation;
        private static string villeUri = "https://developer.jcdecaux.com/rest/vls/stations/{0}.json";
        private static string paysImagesRootPath = "ms-appx:///Assets/Pays";

        private static List<Contract> contracts = new List<Contract>(){
            new Contract{Name = "Bruxelles-Capitale",
               PaysImage = paysImagesRootPath+ "/BE.png",
               Pays = "Belgium"},
               new Contract{Name = "Namur",
               PaysImage = paysImagesRootPath+ "/BE.png",
               Pays = "Belgium"},
               new Contract{Name = "Santander",
               PaysImage = paysImagesRootPath+ "/ES.png",
               Pays = "Spain"},
               new Contract{Name = "Seville",
               PaysImage = paysImagesRootPath+ "/ES.png",
               Pays = "Spain"},
               new Contract{Name = "Valence",
               PaysImage = paysImagesRootPath+ "/ES.png",
               Pays = "Spain"},
               new Contract{Name = "Amiens",
               PaysImage = paysImagesRootPath+ "/FR.png",
               Pays = "France"},
               new Contract{Name = "Besancon",
               PaysImage = paysImagesRootPath+ "/FR.png",
               Pays = "France"},
               new Contract{Name = "Cergy-Pontoise",
               PaysImage = paysImagesRootPath+ "/FR.png",
               Pays = "France"},
               new Contract{Name = "Creteil",
               PaysImage = paysImagesRootPath+ "/FR.png",
               Pays = "France"},
               new Contract{Name = "Lyon",
               PaysImage = paysImagesRootPath+ "/FR.png",
               Pays = "France"},
               new Contract{Name = "Marseille",
               PaysImage = paysImagesRootPath+ "/FR.png",
               Pays = "France"},
               new Contract{Name = "Mulhouse",
               PaysImage = paysImagesRootPath+ "/FR.png",
               Pays = "France"},
               new Contract{Name = "Nancy",
               PaysImage = paysImagesRootPath+ "/FR.png",
               Pays = "France"},
               new Contract{Name = "Nantes",
               PaysImage = paysImagesRootPath+ "/FR.png",
               Pays = "France"},
           new Contract{Name = "Paris", 
               //Velibs = new List<VelibModel>(){new VelibModel(){Name = "test"}},
               //Downloaded = true,
               PaysImage = paysImagesRootPath+ "/FR.png",
               Pays = "France"},
           
               new Contract{Name = "Rouen",
               PaysImage = paysImagesRootPath+ "/FR.png",
               Pays = "France"},
               new Contract{Name = "Toulouse",
               PaysImage = paysImagesRootPath+ "/FR.png",
               Pays = "France"},
                new Contract{Name = "Toyama",
               PaysImage = paysImagesRootPath+ "/JP.png",
               Pays = "Japan"},
               new Contract{Name = "Vilnius",
               PaysImage = paysImagesRootPath+ "/LT.png",
               Pays = "Lithuania"},
               new Contract{Name = "Vilnius",
               PaysImage = paysImagesRootPath+ "/LT.png",
               Pays = "Lithuania"},
               new Contract{Name = "Luxembourg",
               PaysImage = paysImagesRootPath+ "/LU.png",
               Pays = "Luxembourg"},
               new Contract{Name = "Lillestrom",
               PaysImage = paysImagesRootPath+ "/NO.png",
               Pays = "Norway"},


               new Contract{Name = "Kazan",
               PaysImage = paysImagesRootPath+ "/RU.png",
               Pays = "Russia"},
               new Contract{Name = "Goteborg",
               PaysImage = paysImagesRootPath+ "/SE.png",
               Pays = "Sweden"},
               new Contract{Name = "Stockholm",
               PaysImage = paysImagesRootPath+ "/SE.png",
               Pays = "Sweden"},

               new Contract{Name = "Ljubljana",
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
        public ContractsViewModel()
        {
            var test = contracts.ToJson();

        }
    
        public static async void DownloadAndSaveContract(Contract contract)
        {
            await DownloadContract(contract);
            await writeJsonAsync(contract);
        }


        private static async Task writeJsonAsync(Contract contract)
        {
            localSettings.Values[contract.Name] = true;
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

        public static async Task<bool> GetContractsFromHardDrive()
        {
            var velibs = new List<VelibModel>();
            for (int i = 0; i<Contracts.Count; i++)
            {
                var loadedContract = await GetContractFromFile(Contracts[i]);
                Contracts[i] = loadedContract;
                if (Contracts[i].Velibs!= null)
                    VelibDataSource.StaticVelibs.AddRange(Contracts[i].Velibs);
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

                                contract = dataReader.ReadString(numBytesLoaded).FromJsonString<Contract>();
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

        private static async Task DownloadContract(Contract contract){
            var httpClient = new HttpClient();
            var cts = new CancellationTokenSource();
            var velibs = new List<VelibModel>();
            contract.Downloading = true;
            bool failed = false;
            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(new Uri(string.Format(villeUri, contract.Name))).AsTask(cts.Token);
                var responseBodyAsText = await response.Content.ReadAsStringAsync().AsTask(cts.Token);
                contract.Velibs = responseBodyAsText.FromJsonString<List<VelibModel>>();
                contract.VelibCounter = contract.Velibs.Count.ToString() + " cycles";
                foreach (var velib in contract.Velibs)
                {
                    velib.Location = new Windows.Devices.Geolocation.Geopoint(new BasicGeoposition()
                    {
                        Latitude = velib.Latitude,
                        Longitude = velib.Longitude
                    });
                    velib.AvailableBikes = -1;
                    velib.AvailableBikeStands = -1;
                }
                contract.Downloaded = true;
                VelibDataSource.StaticVelibs.AddRange(contract.Velibs);
                httpClient.Dispose();
                cts.Token.ThrowIfCancellationRequested();
            }
            catch (TaskCanceledException)
            {
                failed = true;
            }
            catch (Exception ex)
            {
                failed = true;
            }
            finally
            {
                contract.Downloading = false;
                //  Helpers.ScenarioCompleted(StartButton, CancelButton);
            }
            if (failed)
            {
                var dialog = new MessageDialog("Sorry, you are currently not able to download " + contract.Name);
                await dialog.ShowAsync();
            }
        }

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
            }
            catch (Exception e)
            {
            }
        }
    }
}
