using Velib.Common;
using Velib.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using VelibContext;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls.Maps;
using Windows.Devices.Geolocation;
using System.Text;
using Windows.UI.Popups;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Reactive.Concurrency;
using System.Net.NetworkInformation;
using Velib.VelibContext;
using Velib.Common.Cluster;
using Windows.UI.Xaml.Shapes;
using Windows.UI;

// Pour en savoir plus sur le modèle Application Hub, consultez la page http://go.microsoft.com/fwlink/?LinkId=391641

namespace Velib
{
    /// <summary>
    /// Page affichant une collection groupée d'éléments.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private readonly NavigationHelper navigationHelper;
        private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();
        private readonly ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView("Resources");
        private Geolocator gl = new Geolocator() { DesiredAccuracy = PositionAccuracy.High };
        private Geopoint userLastLocation;
        private ClustersGenerator clusterGenerator;
        private CoreDispatcher dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;
        public static bool BikeMode = true;
        public MainPage()
        {
            this.InitializeComponent();

            // Hub est pris en charge uniquement en mode Portrait
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;

            this.NavigationCacheMode = NavigationCacheMode.Required;

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        /// <summary>
        /// Obtient le <see cref="NavigationHelper"/> associé à ce <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Obtient le modèle d'affichage pour ce <see cref="Page"/>.
        /// Cela peut être remplacé par un modèle d'affichage fortement typé.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }
        private ObservableCollection<VelibModel> _velibs = new ObservableCollection<VelibModel>();
        public ObservableCollection<VelibModel> Velibs
        {
            get { return this._velibs; }
        }
        /// <summary>
        /// Remplit la page à l'aide du contenu passé lors de la navigation. Tout état enregistré est également
        /// fourni lorsqu'une page est recréée à partir d'une session antérieure.
        /// </summary>
        /// <param name="sender">
        /// La source de l'événement ; en général <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Données d'événement qui fournissent le paramètre de navigation transmis à
        /// <see cref="Frame.Navigate(Type, Object)"/> lors de la requête initiale de cette page et
        /// un dictionnaire d'état conservé par cette page durant une session
        /// antérieure.  L'état n'aura pas la valeur Null lors de la première visite de la page.</param>
        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            // TODO: créez un modèle de données approprié pour le domaine posant problème pour remplacer les exemples de données
           // var sampleDataGroups = await SampleDataSource.GetGroupsAsync();
            //this.DefaultViewModel["Groups"] = sampleDataGroups;


            //var test = await VelibDataSource.GetEventsAsync();
            
            
            CoreDispatcher dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;
            
            //MapItems.ItemsSource = Velibs;

            //foreach (var velib in test)
            //{
            //    await Task.Delay(TimeSpan.FromSeconds(0.0005));
            //    ///////////////////////////////////////////////////
            //    // Creating the MapIcon 
            //    //   (text, image, location, anchor point)
            //    var shape = new MapIcon
            //    {
            //        Title = velib.AvailableBikesStr,
            //        Location = velib.Location,
            //        NormalizedAnchorPoint = anchorPoint,
            //        Image = image,
            //        ZIndex = 5
            //    };
            //    shape.AddData(velib);
               
            //    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
            //        MapCtrl.MapElements.Add(shape);
            //    });

                
            //}
        }


        /// <summary>
        /// Conserve l'état associé à cette page en cas de suspension de l'application ou de
        /// suppression de la page du cache de navigation.  Les valeurs doivent être conformes aux
        /// exigences en matière de sérialisation de <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">La source de l'événement ; en général <see cref="NavigationHelper"/></param>
        /// <param name="e">Données d'événement qui fournissent un dictionnaire vide à remplir à l'aide de l'
        /// état sérialisable.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            // TODO: enregistrer l'état unique de la page ici.
        }


        

        #region Inscription de NavigationHelper



        TimeSpan timeSpan = TimeSpan.FromMilliseconds(200);
        int counter = 0;



        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);


            MapCtrl.Center = new Geopoint(new BasicGeoposition { Latitude = 48.8791, Longitude = 2.354 });
            MapCtrl.ZoomLevel = 15.93;

            clusterGenerator = new ClustersGenerator(MapCtrl, this.Resources["VelibTemplate"] as ControlTemplate);

            gl.MovementThreshold = 5;
            gl.ReportInterval = 1000;
            gl.PositionChanged += gl_PositionChanged;
            
           
        }

        private VelibControl previousItemTapped;
        void VelibTapped(object sender, TappedRoutedEventArgs e)
        {
            var itemTapped = (sender as VelibControl);

            if (previousItemTapped != itemTapped)
            {
                if (previousItemTapped != null)
                {
                    VisualStateManager.GoToState(previousItemTapped, "Normal", true);
                    previousItemTapped.Selected = false;
                }
            }

            if (itemTapped.Selected)
            {
                VisualStateManager.GoToState(itemTapped, "Normal", true);
            }
            else
            {
                VisualStateManager.GoToState(itemTapped, "Selected", true);
            }

            itemTapped.Selected = !itemTapped.Selected;
            previousItemTapped = itemTapped;
        }
        void MapCtrl_TransformOriginChanged(MapControl sender, object args)
        {
            Debug.WriteLine("new center : " + sender.Center);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }
        private async void MyMap_MapTapped(MapControl sender, MapInputEventArgs args)
        {
            //var resultText = new StringBuilder();
            //resultText.AppendLine(string.Format("Position={0},{1}", args.Position.X, args.Position.Y));
            //resultText.AppendLine(string.Format("Location={0:F9},{1:F9}", args.Location.Position.Latitude, args.Location.Position.Longitude));
            //resultText.AppendLine(string.Format("Zoom={0}", MapCtrl.ZoomLevel));

            //foreach (var mapObject in sender.FindMapElementsAtOffset(args.Position))
            //{
            //    resultText.AppendLine("Found: " + mapObject.ReadData<VelibModel>().AvailableBikesStr);
            //}
            //var dialog = new MessageDialog(resultText.ToString());
            //await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => alwait dialog.ShowAsync());
        }


        private bool searchingLocation = false;
        private async void AppBarButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (userLastLocation == null)
            {
                if (searchingLocation)
                    return;
                searchingLocation = true;
                var locationGeoPos = await (new Geolocator() { DesiredAccuracy = PositionAccuracy.Default }).GetGeopositionAsync();
                userLastLocation = new Geopoint(new BasicGeoposition() { Longitude = locationGeoPos.Coordinate.Longitude, Latitude = locationGeoPos.Coordinate.Latitude });
                searchingLocation = false;
            }
            UserLocation.SetValue(MapControl.LocationProperty, userLastLocation);
            UserLocation.Opacity = 1;
            double zoom = (MapCtrl.ZoomLevel < 16)? 16 : MapCtrl.ZoomLevel;
            MapCtrl.TrySetViewAsync(userLastLocation, zoom);
        }

        async void gl_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            userLastLocation = new Geopoint(new BasicGeoposition() { Longitude = args.Position.Coordinate.Longitude, Latitude = args.Position.Coordinate.Latitude });
            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                UserLocation.SetValue(MapControl.LocationProperty, userLastLocation);
                UserLocation.Opacity = 1;
            });
        }

  

        private void RadioButtonParking_Checked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (clusterGenerator == null)
                return;
            BikeMode = false;
            foreach (var control in clusterGenerator.Items.Where(v => v.VelibControl != null && v.VelibControl.Velibs.Count == 1).Select(v=>v.VelibControl).ToList())
            {
                control.SwitchModeVelibParking();
            }
        }

        private void RadioButtonVelib_Checked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (clusterGenerator == null)
                return;
            BikeMode = true;
            foreach (var control in clusterGenerator.Items.Where(v => v.VelibControl != null && v.VelibControl.Velibs.Count == 1).Select(v => v.VelibControl).ToList())
            {
                control.SwitchModeVelibParking();
            }
        }


        #endregion
    }
}
