﻿using Velib.Common;
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
using Windows.ApplicationModel.Chat;
using Windows.ApplicationModel.Email;
using Windows.ApplicationModel.Store;
using Velib.Settings;

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
        public Geolocator gl ;
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
        Storyboard AccuracyStoryboard;
        DoubleAnimation AccuracyScaleX;
        DoubleAnimation AccuracyScaleY;

        public static Image PreviewImgBikeAvailability;

        public MainPage()
        {
            this.InitializeComponent();

            PreviewImgBikeAvailability = this.PreviewImage;
            //MapService.ServiceToken = "AkVm6BZviS25-7mLQNpXUKvwcY3PxZsY7drDLo_QHRUao3xwbyEUsH2T7sOhXdWo";
            
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
           // gl.GetGeopositionAsync(TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(3));
            Map = MapCtrl;
            mainPage = this;
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

            AccuracyStoryboard = this.Resources["AccuracyStoryboard"] as Storyboard;
            AccuracyScaleX = AccuracyStoryboard.Children[0] as DoubleAnimation;
            AccuracyScaleY = AccuracyStoryboard.Children[1] as DoubleAnimation;

            clusterGenerator = new ClustersGenerator(MapCtrl, this.Resources["VelibTemplate"] as ControlTemplate);
            if (App.LocalSettings.Values["Localization"] == null || (bool)App.LocalSettings.Values["Localization"] == true)
            {
                App.LocalSettings.Values["Localization"] = true;
                EnableLocalization();
            }

            if (compass != null)
            {
                compass.ReportInterval = 200;
                compass.ReadingChanged -= compass_ReadingChanged;
                compass.ReadingChanged += compass_ReadingChanged;
                VisualStateManager.GoToState(this, "compassState", true);
            }
            else
            {
                VisualStateManager.GoToState(this, "noCompassState", true);
            }
            
            MapCtrl.DoubleTapped += MapCtrl_DoubleTapped;
            Observable.FromEventPattern(MapCtrl, "HeadingChanged")
                .Throttle(TimeSpan.FromMilliseconds(100))
                .Subscribe(x =>
                {
                    Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        if (MapCtrl.Heading != 0)
                        {
                            VisualStateManager.GoToState(this, "NorthIndicatorVisible", true);
                        }
                        else
                        {
                            VisualStateManager.GoToState(this, "NorthIndicatorHidden", true);
                        }
                        UpdateNorthElementAngle(MapCtrl.Heading);
                    });
                });
       

            Observable.FromEventPattern(MapCtrl, "ZoomLevelChanged")
                        .Throttle(TimeSpan.FromMilliseconds(200))
                        .Subscribe(x =>
                        {
                            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                            {
                                refreshAccuracyIndicator();
                            });
                        });
            TouchPanel.Holding += TouchPanel_Holding;
            TouchPanel.ManipulationStarted += TouchPanel_ManipulationStarted;
            TouchPanel.ManipulationStarting += TouchPanel_ManipulationStarting;
           // SearchLocationPoint.Holding += SearchLocationPoint_Holding;
            this.NavigationCacheMode = NavigationCacheMode.Required;

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
            this.Loaded += MainPage_Loaded;
        }


        public void EnableLocalization()
        {
            gl = new Geolocator() { DesiredAccuracy = PositionAccuracy.High, MovementThreshold = 5, ReportInterval = 1000 };
            gl.PositionChanged += gl_PositionChanged;
            gl.StatusChanged += gl_StatusChanged;
        }
        public void DisableLocalization()
        {
            gl.PositionChanged -= gl_PositionChanged;
            gl.StatusChanged -= gl_StatusChanged;
            gl = null;
        }

        void SearchLocationPoint_Holding(object sender, HoldingRoutedEventArgs e)
        {
            ShowFlyout(sender);
        }
    

        void gl_StatusChanged(Geolocator sender, StatusChangedEventArgs args)
        {
            Debug.WriteLine("Geolocator  status: " + args.Status);
        }

        void TouchPanel_ManipulationStarting(object sender, ManipulationStartingRoutedEventArgs e)
        {
            if(searchCancellationToken != null)
                searchCancellationToken.Cancel();
            HideSearch();
            VisualStateManager.GoToState(this, "SearchLocationMinimize", true);
            if (PreviousSelectedItem == SearchLocationPoint)
                PreviousSelectedItem = null;
        }

        void TouchPanel_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            StopCompassAndUserLocationTracking();
           // MapCtrl.TrySetViewAsync(MapCtrl.Center, null, null, null, MapAnimationKind.None);
            //LocationButton.Icon = new SymbolIcon(Symbol.Target);
            //LocationButton.Label = "Location";
        }



        private bool pageLoaded = false;
       public async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {

           pageLoaded = true;
           //var routeResult = await MapRouteFinder.GetWalkingRouteAsync(
           //        new Geopoint(new BasicGeoposition() { Latitude = 51.494228, Longitude = -0.247820 }),
           //         new Geopoint(new BasicGeoposition() { Latitude = 51.482809, Longitude =-0.262452 }),
           //        ); 

            //Geopoint point1 = new Geopoint(new BasicGeoposition()
            //{
            //    Latitude = 51.494228,
            //    Longitude = -0.247820
            //});
            //Geopoint point2 = new Geopoint(new BasicGeoposition()
            //{
            //    Latitude = 51.482809,
            //    Longitude = -0.262452 
            //});
            //var routeResult = await MapRouteFinder.GetWalkingRouteAsync(point1, point2);
            //Debug.WriteLine(routeResult.Status);  
        


           Debug.WriteLine("Page LOADED");  
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


        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
            if (DateTime.Now.Hour > 20 || DateTime.Now.Hour < 5)
                MapCtrl.ColorScheme = MapColorScheme.Dark;
        }

        
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }
        //private void MyMap_MapTapped(MapControl sender, MapInputEventArgs args)
        //{
        //    HideSearchLocationPoint();
        //}

        private void SetCompassIcon()
        {
            LocationButton.Icon = SymbolView;
            LocationButton.Label = "Compass";
        }
        private void SetTargetIcon()
        {
            LocationButton.Icon = SymbolTarget;
            LocationButton.Label = "Location";
        }
        private bool searchingLocation = false;
        private bool stickToUserLocation = false;
        public bool compassMode = false;
        private SymbolIcon SymbolView = new SymbolIcon(Symbol.View);
        private SymbolIcon SymbolTarget = new SymbolIcon(Symbol.Target);
        //private Geolocator glOnDemande;
        private async void LocationButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (App.LocalSettings.Values["Localization"] != null && (bool)App.LocalSettings.Values["Localization"] == false)
            {
                var dialog = new MessageDialog("Localization in disabled, go to the settings page to activate it ?");
                //Commands
                dialog.Commands.Add(new UICommand("Ok", new UICommandInvokedHandler((f) => Frame.Navigate(typeof(SettingsPage)))));
                dialog.Commands.Add(new UICommand("Cancel"));


                Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => await dialog.ShowAsync());
                return;
            }

            if (stickToUserLocation == false)
            {
                if (userLastLocation == null)
                {
                    if (searchingLocation)
                        return;
                    searchingLocation = true;
                    Geoposition locationGeoPos = null;
                    try
                    {
                        locationGeoPos = await (new Geolocator() { DesiredAccuracy = PositionAccuracy.Default }).GetGeopositionAsync();
                    }
                    catch (Exception ex)
                    {
                        var dialog = new MessageDialog("Unable to find your location now. Please try again later.");
                        Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => await dialog.ShowAsync());
                        return;
                    }
                    userLastLocation = new Geopoint(new BasicGeoposition() { Longitude = locationGeoPos.Coordinate.Longitude, Latitude = locationGeoPos.Coordinate.Latitude });
                    // for emulator only
                    //userLastLocation = new Geopoint(new BasicGeoposition() { Longitude = 2.3465, Latitude = 48.8494 });
                    searchingLocation = false;
                }


                ShowUserLocation();

                //SetView(userLastLocation, null, null, null, MapAnimationKind.None);
                var zoom = MapCtrl.ZoomLevel;
                if (MapCtrl.ZoomLevel < 13)
                {
                    zoom = 13;
                }

                MapCtrl.TrySetViewAsync(userLastLocation, zoom, null, null, MapAnimationKind.None);
                //MapCtrl.TrySetViewAsync(userLastLocation, MapCtrl.ZoomLevel, null, null, MapAnimationKind.None);

                SetCompassIcon();
                stickToUserLocation = true;
                // enable touch for relay to the map in order to abort the following of the user location
                //TouchPanel.Visibility = Visibility.Visible;
                return;
            }

            if (compassMode)
            {
                StopCompass();
                if (Map.Heading != 0)
                    MapCtrl.TrySetViewAsync(MapCtrl.Center, null, 0, null, MapAnimationKind.Linear);
            }
            else
            {
                // Compass mode
                if (compass != null)
                {
                    compassMode = true;
                    VisualStateManager.GoToState(this, "NorthIndicatorVisible", true);
                    SetTargetIcon();
                }
            }
        }

        private void ShowUserLocation()
        {
            UserLocation.SetValue(MapControl.LocationProperty, userLastLocation);
            UserLocation.Opacity = 1;
        }
        private  void StopCompass()
        {
            compassMode = false;
            if (stickToUserLocation)
            {
                SetCompassIcon();
            }
            else
            {
                SetTargetIcon();
            }
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

            angle = Math.Round(angle, 0);

     
     
            dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
               // UpdateNorthElementAngle(MapCtrl.Heading);
                UpdateUserLocationElementAngle(angle - Map.Heading);
                if (compassMode){
                    //.Heading = angle;
                    SetView(userLastLocation, null, angle, null, MapAnimationKind.Linear);
                    //MapCtrl.TrySetViewAsync(userLastLocation, null, angle, null, MapAnimationKind.Linear);
                    //MapCtrl.Heading = angle;
                    
                }

                

            });

        }

        private void refreshAccuracyIndicator()
        {
            if (lastPositionChangedEventArgs != null 
                && lastPositionChangedEventArgs.Position != null 
                && lastPositionChangedEventArgs.Position.Coordinate != null 
                && lastPositionChangedEventArgs.Position.Coordinate.Accuracy != 0
                && lastPositionChangedEventArgs.Position.Coordinate.Accuracy < 1000)
            {
                double metersPerPixels = MapExtensions.GroundResolution(lastPositionChangedEventArgs.Position.Coordinate.Point.Position.Latitude, Map.ZoomLevel);
                double radius = lastPositionChangedEventArgs.Position.Coordinate.Accuracy / metersPerPixels;
                AccuracyScaleX.To = radius;
                AccuracyScaleY.To = radius;
                AccuracyStoryboard.Begin();
            }
        }

        Geopoint previousBigChangeInUserLocation;
        PositionChangedEventArgs lastPositionChangedEventArgs;
        async void gl_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            lastPositionChangedEventArgs = args;
            userLastLocation = new Geopoint(new BasicGeoposition() { Longitude = args.Position.Coordinate.Point.Position.Longitude, Latitude = args.Position.Coordinate.Point.Position.Latitude });
            // for emulator only
            //userLastLocation = new Geopoint(new BasicGeoposition() { Longitude = 2.3465, Latitude = 48.8494 });
            if (previousBigChangeInUserLocation == null)
                previousBigChangeInUserLocation = userLastLocation;
            
            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                refreshAccuracyIndicator();

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
                            Map.TrySetViewAsync(userLastLocation, null, null, null, MapAnimationKind.Linear);
                            //SetView(userLastLocation, null, null, null, MapAnimationKind.Linear);
                        }
                    }
                }
                ShowUserLocation();
                
            });

        }


        //private void RadioButtonParking_Checked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        //{
        //    if (clusterGenerator == null)
        //        return;
        //    BikeMode = false;
        //    foreach (var control in clusterGenerator.Items.Where(v => v.VelibControl != null && v.VelibControl.Velibs.Count == 1).Select(v=>v.VelibControl).ToList())
        //    {
        //        control.SwitchModeVelibParking();
        //    }
        //}

        //private void RadioButtonVelib_Checked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        //{
        //    if (clusterGenerator == null)
        //        return;
        //    BikeMode = true;
        //    foreach (var control in clusterGenerator.Items.Where(v => v.VelibControl != null && v.VelibControl.Velibs.Count == 1).Select(v => v.VelibControl).ToList())
        //    {
        //        control.SwitchModeVelibParking();
        //    }
        //}

        private void DownloadCitiesButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ContractsPage));
        }
        private void SettingsButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SettingsPage));
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
            //    Geopoint location;
            //    MapCtrl.GetLocationFromOffset(e.GetPosition(MapCtrl), out location);
            //    await MapCtrl.TrySetViewAsync(location, zoom, null, null, MapAnimationKind.None);
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
                control.SwitchModeVelibParking(control.Velibs.FirstOrDefault());
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
                SearchAddress(searchCancellationToken.Token);
            }
        }



        private async Task<MapLocationFinderResult> FindLocation(string address, Geopoint hintPoint, uint maxResult)
        {
            return await MapLocationFinder.FindLocationsAsync(
                                    address,
                                    hintPoint,
                                    maxResult);
        }
        MapLocationFinderResult lastSearchLocationResult;
        private async void SearchAddress(CancellationToken token)
        {
            if (string.IsNullOrWhiteSpace(SearchTextBox.Text))
                return;

            VisualStateManager.GoToState(this, "Searching", true);
            if (token.IsCancellationRequested) {
                return;
            }
            SearchingProgressBar.Visibility = Visibility.Visible;

            var task = FindLocation(SearchTextBox.Text, userLastLocation, 1);
            if (task != await Task.WhenAny(task, Task.Delay(3000, token)))
            {
                // timout case 
                Debug.WriteLine("searching address TIMEOUT or CANCELED !");
                if(!token.IsCancellationRequested)
                    SearchAddress(token);
                return;
            }

            if (token.IsCancellationRequested)
            {
                return;
            }
            var result = task.Result;


            // If the query returns results, display the coordinates
            // of the first result.
            if (result.Status == MapLocationFinderStatus.Success && result.Locations.Count > 0)
            {
                StopCompassAndUserLocationTracking();
                
              

                lastSearchLocationResult = result;
                MapCtrl.Center = result.Locations.FirstOrDefault().Point;
                MapCtrl.Heading = 0;
                SearchLocationText.Text =result.ParseMapLocationFinderResultAddress() ;
                ShowSearchLocationPoint(result.Locations.FirstOrDefault().Point, SearchLocationText.Text);
                HideSearch();

                
            }
            else
            {
                if (!token.IsCancellationRequested)
                {
                    VisualStateManager.GoToState(this, "SearchVisible", true);
                    SearchStatusTextBlock.Text = "Couldn't find that place. Try using different spelling or keywords.";
                }
            }
        }

        public void StopCompassAndUserLocationTracking()
        {
            stickToUserLocation = false;
            StopCompass();
        }

        private void HideSearch()
        {
            actionVisisible = false;
            // reenable holding
            //TouchPanel.Visibility = Visibility.Collapsed;
            VisualStateManager.GoToState(this, "SearchHidden", true);
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
                SearchAddress(searchCancellationToken.Token);
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



        private async Task<MapRouteFinderResult> FindRoute(Geopoint startPoint, Geopoint endPoint)
        {
            return await MapRouteFinder.GetWalkingRouteAsync(
                    startPoint,
                    endPoint
                    );
        }
        MapRoute previousMapRoute;
        Geopoint previousRouteStartingPoint;
        Geopoint previousRouteEndingPoint;
        public async void GetRouteWithToken(Geopoint endPoint, CancellationToken token, Favorite favorite = null, int retry = 0)
        {
            if (favorite != null)
            {
                ShowSearchLocationPoint(endPoint, favorite.Name);
                if (userLastLocation == null)
                {
                    //var dialog = new MessageDialog(
                    //    "To get there, the phone must find your location first. Please wait a bit an try again.");
                    //await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                    //{
                    //    await dialog.ShowAsync();
                    //});

                    await MapCtrl.TrySetViewAsync(endPoint, 15, 0, null);
                }
                else
                {
                    // Fit the MapControl to the route.
                    await MapCtrl.TrySetViewBoundsAsync(MapExtensions.GetAreaFromLocations(new List<Geopoint>() { userLastLocation, endPoint }),
                        new Thickness(40, 40, 40, 40), MapAnimationKind.None);
                }
                return;

            }

            if (userLastLocation == null)
                return;

            if (previousRouteEndingPoint == endPoint && userLastLocation == previousRouteStartingPoint)
            {
                Debug.WriteLine("Skip route finder : same location provided.");
                return;
            }
            previousRouteStartingPoint = userLastLocation;
            previousRouteEndingPoint = endPoint;
            var task = FindRoute(userLastLocation, endPoint);
            if (task != await Task.WhenAny(task, Task.Delay(2000, token)))
            {
                MapCtrl.Routes.Clear();
                // timout case 
                Debug.WriteLine("get route TIMEOUT or CANCELED !");
                // BUG : apparently MapRouteFinder.GetWalkingRouteAsync is on UI thread. don't recall it again
                //  if (!token.IsCancellationRequested && retry < 5)
                //       GetRouteWithToken(endPoint, token, null, ++retry);
                return;
            }
            var routeResult = task.Result;
            //var routeResult = task.Result;
            Debug.WriteLine("get route ended with result : " + routeResult.Status);
            if (routeResult.Status == MapRouteFinderStatus.Success)
            {

                // Use the route to initialize a MapRouteView.
                MapRouteView viewOfRoute = new MapRouteView(routeResult.Route);
                viewOfRoute.RouteColor = new SolidColorBrush((Application.Current.Resources["PhoneAccentBrush"] as SolidColorBrush).Color).Color;
                viewOfRoute.OutlineColor = Colors.Black;


                if (previousMapRoute == null || previousMapRoute.LengthInMeters != routeResult.Route.LengthInMeters)
                {
                    MapCtrl.Routes.Clear();
                    Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                    {
                        // Add the new MapRouteView to the Routes collection
                        // of the MapControl.
                        MapCtrl.Routes.Add(viewOfRoute);

                        ShowUserLocation();

                    });
                }
                previousMapRoute = routeResult.Route;
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

        public void ShowFlyout(object item)
        {
            VelibFlyout.ShowAt(this);
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
                if (PreviousSelectedItem == SearchLocationPoint && !skipFlyout)
                    ShowFlyout(this);
                LastSearchGeopoint = SearchLocationPoint.GetValue(MapControl.LocationProperty) as Geopoint;
                ReverseGeocode(true);
                VisualStateManager.GoToState(this, "SearchLocationNormal", true);
            }else{

            }


          

            if (item is VelibControl)
            {
                var control = item as VelibControl;
               
                //VisualStateManager.GoToState(control, "ShowSelected", true);
                
                
                
                var velib = control.Velibs.FirstOrDefault();
                if (velib != null)
                {
                    LastSearchGeopoint = velib.Location;
                    ReverseGeocode();
                    if (PreviousSelectedVelibStation != null)
                    {
                        var prevControl = PreviousSelectedVelibStation.VelibControl as Control;
                        //if (prevControl != null && prevControl != control)
                        //    VisualStateManager.GoToState(prevControl, "HideSelected", true);


                    }
                  
                }

            }

            // Show the route if the user is at least 3 KM from the selected item
            if (userLastLocation != null && LastSearchGeopoint != null && LastSearchGeopoint.Position.GetDistanceKM(userLastLocation.Position) < 4)
            {
                GetRoute(LastSearchGeopoint);
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
                VisualStateManager.GoToState(this, "SearchLocationNormal", true);
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

            LastSearchGeopoint = new Geopoint(new BasicGeoposition() { Latitude = point.Position.Latitude, Longitude = point.Position.Longitude });

            ShowSearchLocationPoint(LastSearchGeopoint, string.Empty);

            //MapCtrl.TrySetViewAsync(LastSearchGeopoint,null,null,null,MapAnimationKind.None);

            //ReverseGeocode(true);
        }
        private void ReverseGeocode(bool fromSearch = false)
        {
            if (reverseGeocodeCancellationTokenSource == null)
                reverseGeocodeCancellationTokenSource = new CancellationTokenSource();
            else
                reverseGeocodeCancellationTokenSource.Cancel();
            reverseGeocodeCancellationTokenSource = new CancellationTokenSource();

            ReverseGeocode(fromSearch, reverseGeocodeCancellationTokenSource.Token);
            
        }



        private async Task<MapLocationFinderResult> FindLocationsAt(Geopoint geoPoint)
        {
            return await MapLocationFinder.FindLocationsAtAsync(geoPoint);
        }

        private string lastAddressFound;
        private Geopoint previousGeopoint;
        private async void ReverseGeocode(bool fromSearch, CancellationToken token, int retry = 0 )
        {
            lastAddressFound = string.Empty;
            if (LastSearchGeopoint == previousGeopoint)
            {
                Debug.WriteLine("Skip reverse geocode : same location provided.");
                return;
            }
            previousGeopoint = LastSearchGeopoint;
            Debug.WriteLine("searching adress...");
            var task = FindLocationsAt(LastSearchGeopoint);

            if (task != await Task.WhenAny(task, Task.Delay(2000, token)))
            {
                if (retry > 2)
                {
                    Debug.WriteLine("searching adress ended : reached retry max!");
                    return;
                }
                    
                // timout case 
                Debug.WriteLine("searching adress TIMEOUT or CANCELED !");
                if(!token.IsCancellationRequested)
                    ReverseGeocode(fromSearch, token, ++retry);
                return;
            }
            
            var result = task.Result;
            if (token.IsCancellationRequested)
            {
                Debug.WriteLine("searching adress cancelled");
                return;
            }
            // TODO WTF : sometime it sends indexfailure.. retry !
            if (result.Status == MapLocationFinderStatus.IndexFailure)
            {
                new Task(async () => {
                    await Task.Delay(500);
                    ReverseGeocode(fromSearch, token);
                    
                }).Start();
                
                return;
            }

            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (result.Status == MapLocationFinderStatus.Success && result.Locations.Count > 0)
                {
                    lastAddressFound = result.ParseMapLocationFinderResultAddress().Trim();
                    Debug.WriteLine("adress found ! : " + lastAddressFound);
                    if (fromSearch)
                    {
                        if (string.IsNullOrWhiteSpace(lastAddressFound))
                            SearchLocationText.Text = "No address found.";
                        else
                            SearchLocationText.Text = lastAddressFound;


                        stickToUserLocation = false;
                        VisualStateManager.GoToState(this, "SearchAddressPinSearchedFinished", true);
                    }
                }
                else
                {
                    if (fromSearch)
                    {
                        stickToUserLocation = false;
                        SearchLocationText.Text = "Unable to find the address.";
                        VisualStateManager.GoToState(this, "SearchAddressPinSearchedFinished", true);
                        Debug.WriteLine("Unable to find the address");
                    }
                }
            });

            
            
        }

        double previousAngleUserLoc;
        #endregion
        public void UpdateUserLocationElementAngle( double angle)
        {
            if (angle == previousAngleUserLoc)
                return;
            if (compassMode)
            {
               UserLocationStoryboard.Stop();
               UserLocationRotationAnimation.From = 0;
                UserLocationRotationAnimation.To = 0;
                UserLocationStoryboard.Begin();
                previousAngleUserLoc = angle;
            }
            else
            {
                if (angle < previousAngleUserLoc)
                {
                    if (Math.Abs(angle - previousAngleUserLoc) > 300)
                        UserLocationRotationAnimation.From = previousAngleUserLoc - 360;
                    else
                        UserLocationRotationAnimation.From = null;

                }
                else if (angle > previousAngleUserLoc)
                {
                    if (Math.Abs(previousAngleUserLoc - angle) > 300)
                        UserLocationRotationAnimation.From = previousAngleUserLoc + 360;
                    else
                        UserLocationRotationAnimation.From = null;

                }
                UserLocationRotationAnimation.To = angle;
                UserLocationStoryboard.Begin();
                previousAngleUserLoc = angle;
            }
            
        }   
        
        
        #region north element

        double previousAngle;
        double angleCorrection;
        double prevModifiedAngle;
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

            NorthIndicatorRotationAnimation.From = prevModifiedAngle;
            NorthIndicatorRotationAnimation.To = -modifiedAngle;

            NorthIndicatorStoryboard.Begin();


            prevModifiedAngle = -modifiedAngle;
            previousAngle = angle;
        }   

        private void NorthIndicator_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            StopCompass();
            
            if (Map.Heading != 0)
                MapCtrl.TrySetViewAsync(MapCtrl.Center, null, 0, null, MapAnimationKind.Linear);
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
            else FavoriteNameTextBox.Text = string.Empty;
            
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
                    FavoritsViewModel.AddFavorit(new Favorite { Latitude = velib.Latitude, Longitude = velib.Longitude, Name = name, Address = velib.Address });
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

       
        

        private void SearchLocationPoint_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            SelectItem(SearchLocationPoint, false);
            //
            //VelibFlyout.ShowAt(this);
        }

        public bool appLaunchedFromProtocolUri = false;
        // trigger a map center changed to refresh the view
        internal async void DataSourceLoaded()
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                if (!appLaunchedFromProtocolUri)
                {
                    if (VelibDataSource.StaticVelibs.Count == 0)
                    {
                        var dialog = new MessageDialog("It's a bit lonely on this map, let's check if your city is in the current list.");
                        dialog.Commands.Add(new UICommand("Ok", null));
                        await dialog.ShowAsync();
                        Frame.Navigate(typeof(ContractsPage));
                        return;
                    }
                    Map.Center = new Geopoint(new BasicGeoposition() { Longitude = Map.Center.Position.Longitude + 0.000001, Latitude = Map.Center.Position.Latitude });
                }
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



        private async void ShareByMailButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            EmailMessage mail = new EmailMessage();
            mail.Subject = "Check this location"; //+ version;

            mail.Body = FormatShareLocationMessage();
            await EmailManager.ShowComposeNewEmailAsync(mail);
        }
        private async void ShareByTextButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await ChatMessageManager.ShowComposeSmsMessageAsync(new ChatMessage
            {
                Body = FormatShareLocationMessage()
            });
        }

        private string FormatShareLocationMessage()
        {
            var latitude = Math.Round(LastSearchGeopoint.Position.Latitude, 6).ToString(CultureInfo.InvariantCulture);
            var longitude = Math.Round(LastSearchGeopoint.Position.Longitude, 6).ToString(CultureInfo.InvariantCulture);
            string body = "Check out this location :\r\n";
            if (!string.IsNullOrWhiteSpace(lastAddressFound))
            {
                body += lastAddressFound + "\r\n";
            }
            body += "easybike://to/?lt=" + latitude + "&ln=" + longitude;

            body += "\r\n\r\nCan't open this location ? Check out \"Easy Bike\" for Windows Phone ";
            body += "http://www.windowsphone.com/s?appid=" + CurrentApp.AppId;

            body += "\r\n\r\nHave an IPhone ? Check out ";
            body += "http://maps.apple.com/?q=" + latitude + "," + longitude + "&z=17";

            body += "\r\n\r\nOtherwise check out ";
            body += "https://maps.google.com/maps?q=loc:" + latitude + ","+longitude+"&z=17";
            return body;
        }

        public async void SetViewToLocation(double lat, double lon)
        {
            LastSearchGeopoint = new Geopoint(new BasicGeoposition() { Latitude = lat, Longitude = lon });
            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                Map.TrySetViewAsync(LastSearchGeopoint, 14.5, 0, null, MapAnimationKind.None);
            });
            StopCompassAndUserLocationTracking();
            appLaunchedFromProtocolUri = true;
            ShowSearchLocationPoint(LastSearchGeopoint, string.Empty);
            
        }

        
    }
}
