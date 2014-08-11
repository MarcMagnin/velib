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
using Windows.Devices.Sensors;
using Windows.Services.Maps;
using System.ComponentModel;
using Windows.Phone.UI.Input;
using Windows.UI.Xaml.Documents;
using System.Threading;
using System.Globalization;
using Velib.Favorits;
using Windows.UI.Xaml.Media.Animation;
using Velib.Tutorial;

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
        private Geolocator gl = new Geolocator() { DesiredAccuracy = PositionAccuracy.High, MovementThreshold = 5, ReportInterval = 1000 };
        private Geopoint userLastLocation;
        private ClustersGenerator clusterGenerator;
        public static CoreDispatcher dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;
        public static bool BikeMode = true;
        private Compass compass = Compass.GetDefault();
        public static MapControl Map;
        public static MainPage mainPage;
        Storyboard NorthIndicatorStoryboard;
        DoubleAnimation NorthIndicatorRotationAnimation;
        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        Storyboard UserLocationStoryboard;
        DoubleAnimation UserLocationRotationAnimation;
        public MainPage()
        {
            this.InitializeComponent();

            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            Map = MapCtrl;
            mainPage = this;
            // Hub est pris en charge uniquement en mode Portrait
           // DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;
            MapCtrl.Center = new Geopoint(new BasicGeoposition { Latitude = 48.8791, Longitude = 2.354 });

            if (Windows.Storage.ApplicationData.Current.LocalSettings.Values["PreviousMapCenterLat"] != null)
            {
                MapCtrl.Center = new Geopoint(new BasicGeoposition { Latitude = (double)localSettings.Values["PreviousMapCenterLat"] 
                    , Longitude = (double)localSettings.Values["PreviousMapCenterLon"]});
                MapCtrl.ZoomLevel = (double)localSettings.Values["PreviousMapZoom"];
            }


            NorthIndicatorStoryboard = this.Resources["NorthIndicatorStoryboard"] as Storyboard;
            NorthIndicatorRotationAnimation = NorthIndicatorStoryboard.Children.First() as DoubleAnimation;

            UserLocationStoryboard = this.Resources["UserLocationStoryboard"] as Storyboard;
            UserLocationRotationAnimation = UserLocationStoryboard.Children.First() as DoubleAnimation;

            clusterGenerator = new ClustersGenerator(MapCtrl, this.Resources["VelibTemplate"] as ControlTemplate);
            gl.PositionChanged += gl_PositionChanged;
            compass.ReportInterval = 200;
            compass.ReadingChanged -= compass_ReadingChanged;
            compass.ReadingChanged += compass_ReadingChanged;
            MapCtrl.MapTapped += MyMap_MapTapped;
            MapCtrl.DoubleTapped += MapCtrl_DoubleTapped;
            MapCtrl.PitchChanged += MapCtrl_PitchChanged;
            MapCtrl.HeadingChanged += MapCtrl_HeadingChanged;
            //MapCtrl.ManipulationStarted += MapCtrl_ManipulationStarted;
            TouchPanel.Holding += TouchPanel_Holding;
            TouchPanel.ManipulationStarted += TouchPanel_ManipulationStarted;
            TouchPanel.ManipulationStarting += TouchPanel_ManipulationStarting;
            this.NavigationCacheMode = NavigationCacheMode.Required;

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
            this.Loaded += MainPage_Loaded;
        }

        void TouchPanel_ManipulationStarting(object sender, ManipulationStartingRoutedEventArgs e)
        {
            this.Focus(Windows.UI.Xaml.FocusState.Programmatic);
            HideSearch();

        }

        void TouchPanel_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            StopCompassAndUserLocationTracking();
            LocationButton.Icon = new SymbolIcon(Symbol.Target);
            LocationButton.Label = "Location";
        }




     
        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Page LOADED");
        }

        void MapCtrl_PitchChanged(MapControl sender, object args)
        {
            //(UserLocation.Projection as PlaneProjection).RotationX = MapCtrl.Pitch; 
             //UserLocationProjection.RotationX = MapCtrl.Pitch;
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
            if (DateTime.Now.Hour > 20 || DateTime.Now.Hour < 5)
                MapCtrl.ColorScheme = MapColorScheme.Dark;

            
            
           
        }

        //private VelibControl previousItemTapped;
        //void VelibTapped(object sender, TappedRoutedEventArgs e)
        //{
        //    var itemTapped = (sender as VelibControl);

        //    if (previousItemTapped != itemTapped)
        //    {
        //        if (previousItemTapped != null)
        //        {
        //            VisualStateManager.GoToState(previousItemTapped, "Normal", true);
        //            previousItemTapped.Selected = false;
        //        }
        //    }

        //    if (itemTapped.Selected)
        //    {
        //        VisualStateManager.GoToState(itemTapped, "Normal", true);
        //    }
        //    else
        //    {
        //        VisualStateManager.GoToState(itemTapped, "Selected", true);
        //    }

        //    itemTapped.Selected = !itemTapped.Selected;
        //    previousItemTapped = itemTapped;
        //}

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }
        private void MyMap_MapTapped(MapControl sender, MapInputEventArgs args)
        {
            //HideSearchLocationPoint();
        }


        private bool searchingLocation = false;
        private bool stickToUserLocation = false;
        private bool compassMode = false;
        private async void AppBarButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (stickToUserLocation == false)
            {
            if (userLastLocation == null)
            {
                if (searchingLocation)
                    return;
                searchingLocation = true;
                Geoposition locationGeoPos = null;
                try { 
                    locationGeoPos = await (new Geolocator() { DesiredAccuracy = PositionAccuracy.Default }).GetGeopositionAsync();
                }
                catch (Exception ex)
                {
                    var dialog = new MessageDialog("Unable to find your location now. Please try again later.");
                    Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => await dialog.ShowAsync());
                    return;
                }
                userLastLocation = new Geopoint(new BasicGeoposition() { Longitude = locationGeoPos.Coordinate.Longitude, Latitude = locationGeoPos.Coordinate.Latitude });
                searchingLocation = false;
            }
         
            LocationButton.Icon = new SymbolIcon(Symbol.View);
            LocationButton.Label = "Compass";
            
            ShowUserLocation();

            SetView(userLastLocation, null, null, null, MapAnimationKind.None);
            //MapCtrl.TrySetViewAsync(userLastLocation, MapCtrl.ZoomLevel, null, null, MapAnimationKind.None);
            

            stickToUserLocation = true;
            // enable touch for relay to the map in order to abort the following of the user location
            //TouchPanel.Visibility = Visibility.Visible;
            return;
        }

            if (compassMode)
            {
                StopCompass(new SymbolIcon(Symbol.View), "Compass");
            }
            else
            {
                if (stickToUserLocation) { 
                // Compass mode
                VisualStateManager.GoToState(this, "NorthIndicatorVisible", true);
                compassMode = true;
                LocationButton.Icon = new SymbolIcon(Symbol.Target);
                LocationButton.Label = "Location";

                }
            }
        }

        private void ShowUserLocation()
        {
            UserLocation.SetValue(MapControl.LocationProperty, userLastLocation);
            UserLocation.Opacity = 1;
        }
        private async void StopCompass(SymbolIcon locationButtonIcon, string locationButtonLabel)
        {
            compassMode = false;
            LocationButton.Icon = locationButtonIcon;
            LocationButton.Label = locationButtonLabel;
            SetView(MapCtrl.Center, null, 0, null, MapAnimationKind.Linear);
            //TouchPanel.Visibility = Visibility.Collapsed;
            //await MapCtrl.TrySetViewAsync(MapCtrl.Center, MapCtrl.ZoomLevel, 0, null, MapAnimationKind.Default);
        }

        void MapCtrl_HeadingChanged(MapControl sender, object args)
        {
            if (MapCtrl.Heading != 0)
            {
                VisualStateManager.GoToState(this, "NorthIndicatorVisible", true);
            }
            else
            {
                VisualStateManager.GoToState(this, "NorthIndicatorHidden", true);
            }

            //NorthIndicatorRotationAnimation.To = Map.;

        
        }

       

        private void compass_ReadingChanged(Compass sender, CompassReadingChangedEventArgs args)
        {
            double angle;
            if (args.Reading.HeadingTrueNorth.HasValue)
            {
                angle = args.Reading.HeadingTrueNorth.Value;
            }
            else
                angle = args.Reading.HeadingMagneticNorth;


            
            
            dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                UpdateNorthElementAngle(MapCtrl.Heading);
                UpdateUserLocationElementAngle(angle);
                if (compassMode){
                    //.Heading = angle;
                    SetView(null, null, angle, null, MapAnimationKind.Linear);
                     //MapCtrl.TrySetViewAsync(MapCtrl.Center, MapCtrl.ZoomLevel, angle, null, MapAnimationKind.Linear);
                    //MapCtrl.Heading = angle;
                    
                }

                

            });

        }

        Geopoint previousBigChangeInUserLocation;
        async void gl_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            userLastLocation = new Geopoint(new BasicGeoposition() { Longitude = args.Position.Coordinate.Longitude, Latitude = args.Position.Coordinate.Latitude });
            if (previousBigChangeInUserLocation == null)
                previousBigChangeInUserLocation = userLastLocation;
            
            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                var pixelDistance = userLastLocation.Position.GetDistancePixel(MapCtrl.Center.Position, MapCtrl.ZoomLevel);
                // recalculate route if there is any big change in the position
                if (MapCtrl.Routes.Count > 0)
                {
                    if (previousBigChangeInUserLocation.Position.GetDistancePixel(userLastLocation.Position, MapCtrl.ZoomLevel) > 50)
                    {
                        previousBigChangeInUserLocation = userLastLocation;
                        GetRoute(LastSearchGeopoint);
                    }
                }
                if (stickToUserLocation)
                {
                    if (compassMode)
                        SetView(userLastLocation, null, null, null, MapAnimationKind.None);
                    else
                    {
                        // only move center if the userlocation is far from the center
                        if (pixelDistance > 50)
                        {
                            SetView(userLastLocation, null, null, null, MapAnimationKind.Linear);
                        }
                    }
                }
                ShowUserLocation();
                
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

        private void DownloadCitiesButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ContractsPage));
        }

        private async void MapCtrl_DoubleTapped(object sender, Windows.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
        {
            double zoom = MapCtrl.ZoomLevel + 1;
            if (zoom > 20)
                zoom = 20;
            
            if(stickToUserLocation){
                await MapCtrl.TrySetViewAsync(userLastLocation, zoom, null, null, MapAnimationKind.None);
            }
                //SetView(null, zoom, null, null, MapAnimationKind.None);
            else
            {
                Geopoint location;
                MapCtrl.GetLocationFromOffset(e.GetPosition(MapCtrl), out location);
                await MapCtrl.TrySetViewAsync(location, zoom, null, null, MapAnimationKind.None);
            }
                
        }



        private void ToggleButtonVelibParking_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (clusterGenerator == null)
                return;

            if (ToggleButtonVelibParking.IsChecked.Value)
                BikeMode = false;
            else
                BikeMode = true;
            foreach (var control in clusterGenerator.Items.Where(v => v.VelibControl != null && v.VelibControl.Velibs.Count == 1).Select(v => v.VelibControl).ToList())
            {
                control.SwitchModeVelibParking();
            }
        }


        private async void AddressTextBox_KeyUp(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                if (string.IsNullOrWhiteSpace(SearchTextBox.Text))
                    return;
                if (searchCancellationToken == null)
                    searchCancellationToken = new CancellationTokenSource();
                else
                    searchCancellationToken.Cancel();
                searchCancellationToken = new CancellationTokenSource(); 
                SearchAddress(searchCancellationToken.Token, searchCounter++);
                
            }
        }
        int searchCounter = 0;
        MapLocationFinderResult lastSearchLocationResult;
        private async void SearchAddress(CancellationToken token, int counter)
        {
            if (string.IsNullOrWhiteSpace(SearchTextBox.Text))
                return;

            VisualStateManager.GoToState(this, "Searching", true);
            if (token.IsCancellationRequested) {
                return;
            }
            SearchingProgressBar.Visibility = Visibility.Visible;
            // Geocode the specified address, using the specified reference point as
            // a query hint. Return no more than 3 results.
            MapLocationFinderResult result = await MapLocationFinder.FindLocationsAsync(
                                    SearchTextBox.Text,
                                    userLastLocation,
                                    1);

            if (token.IsCancellationRequested)
            {
                return;
            }



            // If the query returns results, display the coordinates
            // of the first result.
            if (result.Status == MapLocationFinderStatus.Success && result.Locations.Count > 0)
            {
                StopCompassAndUserLocationTracking();

                lastSearchLocationResult = result;
                MapCtrl.Center = result.Locations.FirstOrDefault().Point;
                
                SearchLocationText.Text =result.ParseMapLocationFinderResultAddress() ;
                ShowSearchLocationPoint(result.Locations.FirstOrDefault().Point, SearchLocationText.Text);
                HideSearch();

                
            }
            else
            {
               // VisualStateManager.GoToState(this, "SearchVisible", true);
                SearchStatusTextBlock.Text = "Couldn't find that place. Try using different spelling or keywords.";
            }
        }

        public void StopCompassAndUserLocationTracking()
        {
            compassMode = false;
            stickToUserLocation = false;
        }

        private void HideSearch()
        {
            actionVisisible = false;
            // reenable holding
            //TouchPanel.Visibility = Visibility.Collapsed;
            VisualStateManager.GoToState(this, "SearchHidden", true);
            SearchTextBox.Text = string.Empty;
            SearchStatusTextBlock.Text = string.Empty;
            this.Focus(Windows.UI.Xaml.FocusState.Programmatic); 
        }

        private bool actionVisisible;
        private CancellationTokenSource searchCancellationToken;
        private void SearchButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            VelibFlyout.Hide();
            FavoriteFlyout.Hide();

            if (actionVisisible && string.IsNullOrWhiteSpace(SearchTextBox.Text))
            {
                HideSearch();
                return;
            }else if(!string.IsNullOrWhiteSpace(SearchTextBox.Text))
            {
                if (searchCancellationToken == null)
                    searchCancellationToken = new CancellationTokenSource();
                else
                    searchCancellationToken.Cancel();
                searchCancellationToken = new CancellationTokenSource();
                SearchAddress(searchCancellationToken.Token, searchCounter++);
            }
            else
            {
                VisualStateManager.GoToState(this, "SearchVisible", true);
                actionVisisible = true;
            
            }
            SearchTextBox.Focus(FocusState.Programmatic);
            //SearchStatusTextBlock.Select((TextPointer)0, (TextPointer)0);
            //var t = new Pointer();
        }



        void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            if (actionVisisible)
            {
                e.Handled = true;
                HideSearch();
            }
        }
        public void GetRoute(Geopoint point, Favorite favorit= null){
            HideSearch();

             if (SearchRouteCancellationToken != null)
                SearchRouteCancellationToken.Cancel();
            SearchRouteCancellationToken = new CancellationTokenSource();
            GetRouteWithToken(point, SearchRouteCancellationToken.Token, favorit);
        }

        MapRoute previousMapRoute;
        public async void GetRouteWithToken(Geopoint endPoint, CancellationToken token, Favorite favorite = null)
        {



            if (favorite != null)
            {
                ShowSearchLocationPoint(endPoint, favorite.Name);
                if (userLastLocation == null)
                {
                    var dialog = new MessageDialog(
                        "To get there, the phone must find your location first. Please wait a bit an try again.");
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                    {
                        await dialog.ShowAsync();
                    });
                    return;


                    
                }
                // Fit the MapControl to the route.
                await MapCtrl.TrySetViewBoundsAsync(MapExtensions.GetAreaFromLocations(new List<Geopoint>() { userLastLocation, endPoint }),
                    new Thickness(40, 40, 40, 40), MapAnimationKind.None);

            }

            if (userLastLocation == null)
                return;

            // Get the route between the points.
                MapRouteFinderResult routeResult =
                    await MapRouteFinder.GetWalkingRouteAsync(
                    userLastLocation,
                    endPoint
                    );

                if (token.IsCancellationRequested)
                {
                    return;
                }



                if (routeResult.Status == MapRouteFinderStatus.Success)
                {

                    // Use the route to initialize a MapRouteView.
                    MapRouteView viewOfRoute = new MapRouteView(routeResult.Route);
                    viewOfRoute.RouteColor = new SolidColorBrush((Application.Current.Resources["PhoneAccentBrush"] as SolidColorBrush).Color).Color;
                    viewOfRoute.OutlineColor = Colors.Black;


                    if (previousMapRoute == null || previousMapRoute.LengthInMeters != routeResult.Route.LengthInMeters)
                    {



                        MapCtrl.Routes.Clear();
                        Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => {
                            // Add the new MapRouteView to the Routes collection
                            // of the MapControl.
                            MapCtrl.Routes.Add(viewOfRoute);
                      
                            ShowUserLocation();
                            
                        });
                    }
                    previousMapRoute= routeResult.Route;
                }
                else
                {
                    MapCtrl.Routes.Clear();
                }

        }

        public void HideSearchLocationPoint()
        {
            SearchLocationPoint.Opacity = 0;
            SearchLocationPoint.Visibility = Visibility.Collapsed;
        }

        public object PreviousSelectedItem;
        public VelibModel PreviousSelectedVelibStation;
        public CancellationTokenSource SearchRouteCancellationToken = new CancellationTokenSource();

        public void SelectItem(object item, bool skipFlyout)
        {
                

            
            

            //if (item == SearchLocationPoint && PreviousSelectedItem != null && PreviousSelectedItem is VelibControl)
            //{

            //    var control = PreviousSelectedItem as VelibControl;
            //    VisualStateManager.GoToState(control, "HideSelected", true);
            //    if (PreviousSelectedVelibStation != null && PreviousSelectedVelibStation.Selected)
            //        PreviousSelectedVelibStation.Selected = false;
            //}
            if (item == SearchLocationPoint )
            {
                LastSearchGeopoint = SearchLocationPoint.GetValue(MapControl.LocationProperty) as Geopoint;
            }

            


            if (item is VelibControl)
            {
                var control = item as VelibControl;
               
                VisualStateManager.GoToState(control, "ShowSelected", true);
                
                
                
                var velib = control.Velibs.FirstOrDefault();
                if (velib != null)
                {
                    LastSearchGeopoint = velib.Location;
                    if (PreviousSelectedVelibStation != null)
                    {
                        var prevControl = PreviousSelectedVelibStation.VelibControl as Control;
                        if (prevControl != null && prevControl != control)
                            VisualStateManager.GoToState(prevControl, "HideSelected", true);
                        PreviousSelectedVelibStation.Selected = false;


                    }
                    velib.Selected = true;
                    PreviousSelectedVelibStation = velib;

                    
                  
                }

                //if (PreviousSelectedItem != control)
                //{
                //    if (PreviousSelectedVelibStation != null) {
                //        var prevControl = PreviousSelectedVelibStation.VelibControl as Control;
                //        if (prevControl != null)
                //            VisualStateManager.GoToState(prevControl, "HideSelected", true);
                //        PreviousSelectedVelibStation.Selected = false;

                //    }
                //}


            }
            if (PreviousSelectedItem == item && !skipFlyout)
            {
                VelibFlyout.ShowAt(this);
            }

            // Show the route if the user is at least 15 KM from the selected item
            if (userLastLocation != null && LastSearchGeopoint.Position.GetDistanceKM(userLastLocation.Position) < 15)
            {
                GetRoute(LastSearchGeopoint);
            }
            else
            {
                VelibFlyout.ShowAt(this);

            }
            PreviousSelectedItem = item;


        }

        private void ShowSearchLocationPoint(Geopoint location, string name)
        {
            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => {
                
                SearchLocationPoint.SetValue(MapControl.LocationProperty, location);

                // le nom est précisé, c'est une localisation pour un favoris
                if (!string.IsNullOrWhiteSpace(name))
                {
                    SearchLocationText.Text = name;
                    VisualStateManager.GoToState(this, "SearchAddressPinSearchedFinished", true);
                }
                else
                {
                    VisualStateManager.GoToState(this, "SearchAddressPinSearching", true);
                }
                    
                VisualStateManager.GoToState(this, "SearchLocationHide", true);
                VisualStateManager.GoToState(this, "SearchLocationShow", true);

                


                SelectItem(SearchLocationPoint, true);



                // put the canvas on the map this way cause zindex is not working
                if (LayoutRoot.Children.Remove(SearchLocationPoint))
                {
                    MapCtrl.Children.Add(SearchLocationPoint);
                }
                

                
            });
            
            
        }

        public Geopoint LastSearchGeopoint;
        CancellationTokenSource reverseGeocodeCancellationTokenSource;

        void TouchPanel_Holding(object sender, HoldingRoutedEventArgs e)
        {
            // prevent twice holding
            if (e.HoldingState == Windows.UI.Input.HoldingState.Completed)
                return;

            
            Geopoint point;
            MapCtrl.GetLocationFromOffset(e.GetPosition(MapCtrl), out point);
            LastSearchGeopoint = point;

            ShowSearchLocationPoint(LastSearchGeopoint, string.Empty);

            //MapCtrl.TrySetViewAsync(LastSearchGeopoint,null,null,null,MapAnimationKind.None);

            if (reverseGeocodeCancellationTokenSource == null)
                reverseGeocodeCancellationTokenSource = new CancellationTokenSource();
            else
                reverseGeocodeCancellationTokenSource.Cancel();
            reverseGeocodeCancellationTokenSource = new CancellationTokenSource();
            ReverseGeocode(LastSearchGeopoint, reverseGeocodeCancellationTokenSource.Token);
        }

        private async void ReverseGeocode(Geopoint location, CancellationToken token)
        {
            stickToUserLocation = false;

            var result = await MapLocationFinder.FindLocationsAtAsync(location);
            if (token.IsCancellationRequested)
                return;
            var searchedText = "";
             if (result.Status == MapLocationFinderStatus.Success && result.Locations.Count > 0)
            {
                 searchedText = result.ParseMapLocationFinderResultAddress();
            }

             

            if(string.IsNullOrWhiteSpace(searchedText))
             SearchLocationText.Text = "No address found.";
            else
                SearchLocationText.Text = searchedText.Trim();

            VisualStateManager.GoToState(this, "SearchAddressPinSearchedFinished", true);
        }


        double angleCorrectionUserLoc;
        double previousAngleUserLoc;
        bool triggerReinitRotation;
        #endregion
        public void UpdateUserLocationElementAngle( double angle)
        {

            double modifiedAngleUserLoc = angle ;


            if (compassMode) {
                
                modifiedAngleUserLoc = 0;
            //    UserLocationRotationAnimation.From = 0;
            }
            else
            {
                if (angle - previousAngleUserLoc > 150)
                {
                    angleCorrectionUserLoc = 360;
              //      UserLocationRotationAnimation.From = 0;
                }
                else if (angle - previousAngleUserLoc < -150)
                {
                    angleCorrectionUserLoc = 0;
                //    UserLocationRotationAnimation.From = 0;
                }
                modifiedAngleUserLoc = angle - angleCorrectionUserLoc;

            }
            UserLocationRotationAnimation.To = modifiedAngleUserLoc;


            UserLocationStoryboard.Begin();



            previousAngleUserLoc = angle;
        }   
        
        
        #region north element

        double previousAngle;
        double angleCorrection;
        public void UpdateNorthElementAngle( double angle)
        {
            if (NorthIndicator.Visibility == Visibility.Collapsed )
                return;

            double modifiedAngle = angle ;
            if (angle - previousAngle > 150)
            {
                angleCorrection = 360;
            }else if (angle - previousAngle < -150){
                angleCorrection = 0;
            }
            modifiedAngle = -(angleCorrection - angle);

            NorthIndicatorRotationAnimation.To = -modifiedAngle;

            NorthIndicatorStoryboard.Begin();

            

            previousAngle = angle;
        }   

        private void NorthIndicator_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (stickToUserLocation) { 
                StopCompass(new SymbolIcon(Symbol.View), "Compass");
            }
            else
            {
                StopCompass(new SymbolIcon(Symbol.Target), "Location");
            }
        }

       
        private async void FlyoutButtonDriveTo_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (LastSearchGeopoint != null)
            {
                var uriString = String.Format(CultureInfo.InvariantCulture, "ms-drive-to:?destination.latitude={0}&destination.longitude={1}", LastSearchGeopoint.Position.Latitude, LastSearchGeopoint.Position.Longitude);
                Uri uri = new Uri(uriString);
            // The resulting Uri is: "ms-drive-to:?destination.latitude=47.6451413797194
            //  &destination.longitude=-122.141964733601&destination.name=Redmond, WA")

            // Launch the Uri.
            var success = await Windows.System.Launcher.LaunchUriAsync(uri);

            }
        }

        private void FlyoutButtonAddFavorit_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (PreviousSelectedItem is VelibControl)
            {
                var control = PreviousSelectedItem as VelibControl;
                var velib = control.Velibs.FirstOrDefault();
                if (velib != null)
                {
                    FavoriteNameTextBox.Text= "Station " + velib.Number ;
                }
            }
            
        }



        private void FavoritsButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            VelibFlyout.Hide();
            FavoriteFlyout.Hide();
            Frame.Navigate(typeof(FavoritsPage));
        }

        private void FavoriteNameTextBox_KeyUp(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                FavoriteNameButtonOk_Click(null, null);
            }
        }

        private async void FavoriteNameButtonOk_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            
            var name = FavoriteNameTextBox.Text;
            if (string.IsNullOrWhiteSpace(FavoriteNameTextBox.Text))
            {
                var dialog = new MessageDialog("Sorry, favorite name can't be empty.");
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => {
                    await dialog.ShowAsync();
                    FavoriteFlyout.ShowAt(this);
                });
                return;
            }
            if (PreviousSelectedItem is VelibControl)
            {
                var control = PreviousSelectedItem as VelibControl;
                var velib = control.Velibs.FirstOrDefault();
                if (velib != null)
                {
                    FavoritsViewModel.AddFavorit(new Favorite { Latitude = velib.Latitude, Longitude = velib.Longitude, Name = name });
                }
            }

            else if (PreviousSelectedItem  == SearchLocationPoint)
            {

                FavoritsViewModel.AddFavorit(new Favorite { Latitude = LastSearchGeopoint.Position.Latitude, Longitude = LastSearchGeopoint.Position.Longitude, Name = name, 
                    Address = SearchLocationText.Text !=  "Searching address..." ? SearchLocationText.Text  : ""});
                SearchLocationText.Text = name;

            }
            
        	FavoriteFlyout.Hide();
        }

        

        #endregion

        Geopoint prevCenter;
        double? prevZoomLevel;
        double? prevHeading;
        double? prevDesiredPitch;
        internal async void SetView(Geopoint center, double? zoomLevel, double? heading, double? desiredPitch,MapAnimationKind animation){
            if (center != null)
                prevCenter = center;
            if (zoomLevel.HasValue)
                prevZoomLevel = zoomLevel;
            if(heading.HasValue)
                prevHeading = heading;
            if (desiredPitch.HasValue)
                prevDesiredPitch = desiredPitch;
            if (compassMode)
                prevZoomLevel = null;
            if(prevCenter != null)
            await MapCtrl.TrySetViewAsync(prevCenter, prevZoomLevel, prevHeading, prevDesiredPitch, animation);
        }

        internal async void GetNearestStationRoute(Favorite favorit, CancellationToken token )
        {
           
                
            var destination = new Geopoint(
                            new BasicGeoposition() { Latitude = favorit.Latitude, 
                                Longitude = favorit.Longitude });

            ShowSearchLocationPoint(destination, favorit.Name);
            MapCtrl.Routes.Clear();

            // Fit the MapControl to the points
            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                ShowUserLocation();
                
                    
            });


            // Get the route between the points.
            MapRouteFinderResult routeResult =
                await MapRouteFinder.GetWalkingRouteAsync(
                userLastLocation,
                destination
                );

            if (token.IsCancellationRequested)
            {
                return;
            }


            if (routeResult.Status == MapRouteFinderStatus.Success)
            {
                // Use the route to initialize a MapRouteView.
                MapRouteView viewOfRoute = new MapRouteView(routeResult.Route);
                viewOfRoute.RouteColor = new SolidColorBrush((Application.Current.Resources["PhoneAccentBrush"] as SolidColorBrush).Color).Color;
                viewOfRoute.OutlineColor = Colors.Black;


                Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    // Add the new MapRouteView to the Routes collection
                    // of the MapControl.
                    MapCtrl.Routes.Add(viewOfRoute);
                   
                });

            }
        }

        private void SearchLocationPoint_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            SelectItem(SearchLocationPoint, false);
            //
            //VelibFlyout.ShowAt(this);
        }

        // trigger a map center changed to refresh the view
        internal  void DataSourceLoaded()
        {
            dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Map.Center = new Geopoint(new BasicGeoposition() { Longitude = Map.Center.Position.Longitude+0.0001, Latitude = Map.Center.Position.Latitude });
            });
            
        }

        private void SearchLocationPoint_ManipulationStarting(object sender, Windows.UI.Xaml.Input.ManipulationStartingRoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "SearchLocationClear", true);
            VisualStateManager.GoToState(this, "SearchLocationClick", true);
          
        }

        private void VelibTemplateStationRootCanvas_ManipulationStarting(object sender, Windows.UI.Xaml.Input.ManipulationStartingRoutedEventArgs e)
        {
            var model = (sender as Canvas).DataContext as VelibModel;
            if (model != null && model.VelibControl != null) {
                VisualStateManager.GoToState(model.VelibControl, "Clear", true);
                VisualStateManager.GoToState(model.VelibControl, "Click", true);
            }
        
        }

        private async void CrashLogsButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var dialog = new MessageDialog(localSettings.Values["CrashLog"] == null ? "" : localSettings.Values["CrashLog"].ToString());
            await dialog.ShowAsync();	
        }


        private void HowToUseThisAppButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
        	Frame.Navigate(typeof(HowTo));
        }

        private void AboutButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Frame.Navigate(typeof(About.About));
        }
       
    }
}
