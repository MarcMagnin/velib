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
using Windows.UI.Xaml.Media.Imaging;

namespace Velib.Contracts
{

    public class Contract : INotifyPropertyChanged
    {
        [IgnoreDataMember]
        public DateTime LastUpdate;
        public bool DirectDownloadAvailability;
        public TimeSpan RefreshTimer = TimeSpan.FromSeconds(20);
        [IgnoreDataMember]
        public string ApiUrl;

        public string Name { get; set; }
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


        private string velibCounter;
        public string VelibCounter 
        {
            get { return velibCounter; }
            set
            {
                if (value != this.velibCounter)
                {
                    this.velibCounter = value;
                    NotifyPropertyChanged("VelibCounter");
                }
            }
        }

        public async virtual Task DownloadContract()
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
    }
}
