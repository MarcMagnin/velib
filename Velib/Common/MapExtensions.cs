using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Velib.VelibContext;
using VelibContext;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Maps;

namespace Velib.Common
{
    /// <summary>
    /// Helper methods to easily set and get view area in a more or less consistent way
    /// </summary>
    public static class MapExtensions
    {
        

         public static bool IsLocationVisible(this MapControl map, Point point)
        {
             bool isInView;
             Geopoint location;
             map.GetLocationFromOffset(point, out location);
             map.IsLocationInView(location, out isInView);
             return isInView;
        }



         public static Point  GetOffsetLocation(this VelibModel velib,MapControl map)
         {
             if (velib.OffsetLocation.X == 0)
                 map.GetOffsetFromLocation(velib.Location, out velib.OffsetLocation);
             return velib.OffsetLocation;
         }
        

        public static double GetDistanceTo(this Point p1, Point p2)
        {
            return Math.Sqrt((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y));
        }

        public static GeoboundingBox GetViewArea(this MapControl map)
        {
            Geopoint p1, p2;
            map.GetLocationFromOffset(new Point(0, 0), out p1);
            map.GetLocationFromOffset(new Point(map.ActualWidth, map.ActualHeight), out p2);
            return new GeoboundingBox(p1.Position, p2.Position);
        }

        public static void SetViewArea(this MapControl map, Geopoint p1, Geopoint p2)
        {
            var b = GeoboundingBox.TryCompute(new[] { p1.Position, p2.Position });

            map.TrySetViewBoundsAsync(b, new Thickness(1.0), MapAnimationKind.Bow);
        }

        public static bool Contains(Geopoint location,GeoboundingBox geoBox)
        {
            return (location.Position.Longitude >= geoBox.NorthwestCorner.Longitude &&
                     location.Position.Longitude <= geoBox.SoutheastCorner.Longitude &&
                     location.Position.Latitude <= geoBox.NorthwestCorner.Latitude &&
                     location.Position.Latitude >= geoBox.SoutheastCorner.Latitude);
        }
    }
}