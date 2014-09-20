using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using VelibContext;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Web.Http;

namespace Velib.Contracts
{
    /// <summary>
    /// Note : for the http stack, we will probably have to use Windows.Web.Http.HttpClient instead of  System.Net.Http
    /// </summary>
    public class Contract : INotifyPropertyChanged
    {
        [IgnoreDataMember]
        public DateTime LastUpdate;
        public bool DirectDownloadAvailability;
        public TimeSpan RefreshTimer = TimeSpan.FromSeconds(20);
        public string ApiUrl;
        [IgnoreDataMember]
        protected HttpClient downloadContractHttpClient;

        public string ServiceProvider { get; set; }
        public string Name { get; set; }
        private string technicalName;
        public string TechnicalName
        {
            get
            {
                if (string.IsNullOrEmpty(technicalName))
                    return Name;
                else
                    return technicalName;
            }
            set { technicalName = value; }
        }
        public string Id { get; set; }
        public string Pays { get; set; }
        public string Description { get; set; }
        [IgnoreDataMember]

        private bool downloading;
        public bool Downloading
        {
            get { return downloading; }
            set
            {
                if (value != this.downloading)
                {
                    this.downloading = value;
                    NotifyPropertyChanged("Downloading");
                }
            }
        }

        private bool downloaded;
        public bool Downloaded 
        {
            get { return downloaded; }
            set
            {
                if (value != this.downloaded)
                {
                    this.downloaded = value;
                    NotifyPropertyChanged("Downloaded");
                }
            }
        }
        

        public string PaysImage { get; set; }
        public List<VelibModel> Velibs{ get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        private int velibCounter;
        public int VelibCounter 
        {
            get { return velibCounter; }
            set
            {
                if (value != this.velibCounter)
                {
                    this.velibCounter = value;
                    NotifyPropertyChanged("VelibCounterStr");
                }
            }
        }
        public string VelibCounterStr
        {
            get
            {
                return  velibCounter <= 1 ? velibCounter + " station" : velibCounter + " stations";
            }
        }


        public async virtual Task DownloadContract()
        {
            var velibs = new List<VelibModel>();
            Downloading = true;
            bool failed = false;
            try
            {
                await Task.Run(async () =>
                {
                    downloadContractHttpClient = new HttpClient();
                    await InnerDownloadContract();
                });

                VelibCounter = Velibs.Count;
                Downloaded = true;
                VelibDataSource.StaticVelibs.AddRange(Velibs);
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
                downloadContractHttpClient.Dispose();
                Downloading = false;
            }
            if (failed)
            {
                DownloadContractFail();
            }
        }
        public async virtual Task InnerDownloadContract()
        {
        }

        public virtual void GetAvailableBikes(VelibModel velibModel, CoreDispatcher dispatcher)
        {
        }

        public virtual Contract GetSimpleContract()
        {
            var contract = GetInstanceForSimpleClone();
            contract.Name = this.Name;
            contract.Pays = this.Pays;
            contract.PaysImage = this.PaysImage;
            contract.Downloaded = this.Downloaded;
            return contract;
        }

        protected virtual Contract GetInstanceForSimpleClone(){
            return new Contract();
        }

        protected void DownloadContractFail()
        {
            var dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;
            dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
            {
                var dialog = new MessageDialog("Sorry, you are currently not able to download " + Name);
                dialog.ShowAsync();
            });
        }
    }
}
