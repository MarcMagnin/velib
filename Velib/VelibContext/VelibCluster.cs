using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VelibContext;
using Windows.Foundation;
using Windows.UI.Xaml;

namespace Velib.VelibContext
{
  public class VelibCluster
    {

      public VelibModel FirstItem
      {
          get
          {
              return GetFirstItem();
          }
      }

        public VelibModel BindedModel { get { return GetFirstItem(); } }

        public List<VelibModel> Items = new List<VelibModel>();
        public Point MapLocation { get; set; }
        public int Count
        {
            get
            {
                return Items.Count();
            }
        }
        public VelibCluster(VelibModel pushpin, Point location)
        {
            Items.Add(pushpin);
            MapLocation = location;
        }
        public VelibCluster()
        {
        }
        public void Add(VelibModel pushpin)
        {
            Items.Add(pushpin);
        }


        public VelibModel GetFirstItem()
        {
            return Items.First();
        }

        public void IncludeGroup(VelibCluster group)
        {
            foreach (var pin in group.Items)
                Items.Add(pin);
        }
    }
}
