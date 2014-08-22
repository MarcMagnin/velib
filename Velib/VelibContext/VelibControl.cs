using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VelibContext;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Velib.Common;
using Windows.Devices.Geolocation;
using Windows.UI.Core;
using System.Threading;

namespace Velib.VelibContext
{
    public class VelibControl : ContentControl
    {
        MapControl map;

        public VelibControl(MapControl map)
        {
            this.map = map;
            this.Tapped += VelibControl_Tapped;
            this.ManipulationMode = Windows.UI.Xaml.Input.ManipulationModes.All;
        
        }


        private static VelibControl previousVelibTapped;
        void VelibControl_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            try { 

            if (Velibs.Count > 1)
            {
                if(MainPage.mainPage.compassMode){
                    MainPage.mainPage.StopCompassAndUserLocationTracking();
                }
                    map.TrySetViewBoundsAsync(MapExtensions.GetAreaFromLocations(Velibs.Select(s => s.Location).ToList()), new Thickness(20, 20, 20, 20), MapAnimationKind.Default);
                
            }
            else
            {
                Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    MainPage.mainPage.SelectItem(this, false);
                });

            }
            }
            catch (Exception ex) { 
            }

        }

        public  VelibCluster Cluster = new VelibCluster();

        public List<VelibModel> Velibs = new List<VelibModel>();
        public TextBlock ClusterTextBlock;

        protected override void OnApplyTemplate()
        {
            ClusterTextBlock = GetTemplateChild("textBlockClusterNumber") as TextBlock;
        }


        public void AddVelib(VelibModel velib)
        {
            velib.VelibControl = this;
            Velibs.Add(velib);
            NeedRefresh = true;

        }
       

        public void FinaliseUiCycle(CoreDispatcher dispatcher, CancellationToken token)
        {
            if (Velibs.Count == 0)
            {
                Hide();
                return;
            }
            this.SetValue(MapControl.LocationProperty, GetLocation());
            this.DataContext = Velibs.FirstOrDefault();
            this.Opacity = 1;
            if (Velibs.Count == 1)
            {
                new Task(() => Velibs[0].GetAvailableBikes(dispatcher)).Start();
                SwitchModeVelibParking();
                
            }
            else if(Velibs.Count > 1)
            {
                ClusterTextBlock.Text = Velibs.Count.ToString();
                ShowCluster();
            }
            NeedRefresh = false;
        }

        public void SwitchModeVelibParking()
        {
            ShowVelibStation();
            if (Velibs[0].Loaded && MainPage.BikeMode)
            {
                Velibs[0].AvailableStr = Velibs[0].AvailableBikes.ToString();
                ShowColor(Velibs[0].AvailableBikes);
            }
            if (Velibs[0].Loaded && !MainPage.BikeMode)
            {
                Velibs[0].AvailableStr = Velibs[0].AvailableBikeStands.ToString();
                ShowColor(Velibs[0].AvailableBikeStands);
            }
        }

        public bool NeedRefresh;
        public void RemoveVelib(VelibModel velib)
        {
            NeedRefresh = true;
            velib.VelibControl = null;
            Velibs.Remove(velib);
        }

        public Point MapLocation { get; set; }

        public bool alreadyHandled;
        private bool alreadyLoaded;
        public bool Selected { get; set; }

        public VelibControl()
        {
            DefaultStyleKey = typeof(VelibControl);
            
        }

        public void ShowCluster()
        {
            VisualStateManager.GoToState(this, "Clear", false);
            VisualStateManager.GoToState(this, "Loaded", false);
            VisualStateManager.GoToState(this, "ShowCluster", false);
        }
        public void ShowVelibStation()
        {
           

            VisualStateManager.GoToState(this, "Clear", false);
            VisualStateManager.GoToState(this, "Loaded", false);
            VisualStateManager.GoToState(this, "Normal", false);
            //if(velib!= null && velib.Selected)
            //    VisualStateManager.GoToState(this, "ShowSelected", true);
            //else
            //    VisualStateManager.GoToState(this, "HideSelected", true);
         
                
        }
        public void ShowStationColor()
        {
            var station = Velibs.FirstOrDefault();
            if (MainPage.BikeMode)
            {
                station.AvailableStr = station.AvailableBikes.ToString();
                ShowColor(station.AvailableBikes);

            }
            else
            {
                station.AvailableStr = station.AvailableBikeStands.ToString();
                ShowColor(station.AvailableBikeStands);
            }
        }


        public enum VisualStateColor
        {
            notLoaded,
            other
        }
        public VisualStateColor CurrentVisualStateColor;
        private void ShowColor(int velibNumber)
        {
            if (Velibs.Count != 1)
                return;

            if (velibNumber == -1){
                CurrentVisualStateColor = VelibControl.VisualStateColor.notLoaded;
                VisualStateManager.GoToState(this, "Normal", false);
            }
            else{
                CurrentVisualStateColor = VelibControl.VisualStateColor.other;
                if (velibNumber == 0)
                VisualStateManager.GoToState(this, "ShowRedVelib", false);
            else if (velibNumber < 5)
                VisualStateManager.GoToState(this, "ShowOrangeVelib", false);
            else if (velibNumber >= 5)
                VisualStateManager.GoToState(this, "ShowGreenVelib", false);
            }
            
        }

        public void Hide()
        {
           VisualStateManager.GoToState(this, "Hide", false);
        }


        public Geopoint Location;

        internal Geopoint GetLocation()
        {
            if (Velibs.Count == 1)
                return Location= Velibs[0].Location;

                double x = 0;
                double y = 0;
                foreach (var velib in Velibs)
                {
                    x += velib.Location.Position.Latitude;
                    y += velib.Location.Position.Longitude;
                }
                Location = new Geopoint(new BasicGeoposition { Latitude = x / Velibs.Count, Longitude = y / Velibs.Count });
            return Location;
        }

        // store the offsetLocation in order to reuse it for each draw cycC:\Users\Kobs\Source\Repos\velib2\Velib\VelibContext\VelibControl.csle
        public Point OffsetLocation;
        public Point GetOffsetLocation(MapControl _map)
        {
            if(OffsetLocation.X==0)
            _map.GetOffsetFromLocation(this.GetLocation(), out OffsetLocation);
            return OffsetLocation;
        }
        public Point GetOffsetLocation2( BasicGeoposition origin, double zoomLevel)
        {
            if (OffsetLocation.X == 0)
                OffsetLocation = origin.GetOffsetedLocation(this.GetLocation().Position, zoomLevel);
            return OffsetLocation;
        }

    }
}
