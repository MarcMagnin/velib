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

namespace Velib.VelibContext
{
    public class VelibControl : ContentControl
    {

        public  VelibCluster Cluster = new VelibCluster();

        public List<VelibModel> Velibs = new List<VelibModel>();

        public void AddVelib(VelibModel velib)
        {
            velib.VelibControl = this;
            Velibs.Add(velib);
            NeedRefresh = true;

        }
       

        public void FinaliseUiCycle()
        {
            if (Velibs.Count == 0)
            {
                Hide();
                return;
            }
            this.SetValue(MapControl.LocationProperty, GetLocation());
            this.DataContext = Velibs.First();

            if (Velibs.Count == 1)
            {
                ShowVelib();
            }
            else
            {
                ShowCluster();
            }
            NeedRefresh = false;
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
            VisualStateManager.GoToState(this, "BeforeLoaded", false);
            VisualStateManager.GoToState(this, "Loaded", false);
            VisualStateManager.GoToState(this, "ShowCluster", false);
        }
        public void ShowVelib()
        {
            VisualStateManager.GoToState(this, "BeforeLoaded", false);
            VisualStateManager.GoToState(this, "Loaded", false);
            VisualStateManager.GoToState(this, "ShowVelib", false);
        }

        public void Hide()
        {
           VisualStateManager.GoToState(this, "Hide", false);
        }

        protected override void OnApplyTemplate()
        {
           // VisualStateManager.GoToState(this, "BeforeLoaded", false);
            base.OnApplyTemplate();
          
        }

        public Geopoint Location;

        internal Geopoint GetLocation()
        {
            if (Velibs.Count == 1)
                return Location= Velibs[0].Location;

            if(Location== null){
                double x = 0;
                double y = 0;
                foreach (var velib in Velibs)
                {

                    x += velib.Location.Position.Latitude;
                    y += velib.Location.Position.Longitude;
                }
                Location = new Geopoint(new BasicGeoposition { Latitude = x / Velibs.Count, Longitude = y / Velibs.Count });
            }
            return Location;
        }

        // store the offsetLocation in order to reuse it for each draw cycle
        public Point OffsetLocation;
        public Point GetOffsetLocation(MapControl _map)
        {
            if(OffsetLocation.X==0)
            _map.GetOffsetFromLocation(this.GetLocation(), out OffsetLocation);
            return OffsetLocation;
        }

    }
}
