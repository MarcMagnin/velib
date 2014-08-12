using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Velib.VelibContext;
using VelibContext;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Media;


namespace Velib.Common.Cluster
{
   public class ClustersGenerator
   {
        double MAXDISTANCE = 100;
        const int MAX_CONTROLS = 40;
        private readonly List<VelibControl> velibControls = new List<VelibControl>(MAX_CONTROLS);
        public readonly List<VelibModel> Items = new List<VelibModel>();
        private readonly List<VelibCluster> clusters = new List<VelibCluster>();
        private MapControl _map;
        private TimeSpan throttleTime = TimeSpan.FromMilliseconds(100);
        private CoreDispatcher dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;
        public ControlTemplate ItemTemplate { get; set; }
        public CancellationTokenSource cts = new CancellationTokenSource();

        GeoboundingBox mapArea = null;
        BasicGeoposition leftCornerLocation;
        List<Geopoint> mapLocations = null;
        double zoomLevel=20;
        



        public ClustersGenerator(MapControl map, ControlTemplate itemTemplate)
        {
            _map = map;
            GenerateMapItems();
            this.ItemTemplate = itemTemplate;
            // maps event
            var mapObserver = Observable.FromEventPattern(map, "CenterChanged");
            mapObserver
                .Do((e)=>{

                      cts.Cancel();
                    cts = new CancellationTokenSource();
                })
                .Throttle(throttleTime)
                // .ObserveOn(Scheduler.CurrentThread)
                // .SubscribeOn(Scheduler.Default)
                .Select(async x =>
                {
                    
                    if (VelibDataSource.StaticVelibs == null)
                        return null;

                  
                    await dispatcher.RunAsync(CoreDispatcherPriority.Low, () => {
                        //mapArea = _map.GetViewArea();
                        mapLocations = _map.GetViewLocations();
                        if (mapLocations != null)
                            leftCornerLocation = mapLocations.First().Position;
                        zoomLevel = _map.ZoomLevel;

                        // TESTING only 
                        //velibControls[0].SetValue(MapControl.LocationProperty, new Geopoint(new BasicGeoposition()
                        //    {
                        //        Latitude = mapArea.NorthwestCorner.Latitude,
                        //        Longitude = mapArea.NorthwestCorner.Longitude,
                        //    }));
                        //velibControls[0].ShowVelibStation();
                        //velibControls[1].SetValue(MapControl.LocationProperty, new Geopoint(new BasicGeoposition()
                        //{
                        //    Latitude = mapArea.SoutheastCorner.Latitude,
                        //    Longitude = mapArea.SoutheastCorner.Longitude,
                        //}));
                        //velibControls[1].ShowVelibStation();

                    });
                    //return null;
                    // that could happend is the zoom is really low and the map is turned
                    if (mapLocations == null)
                        return null;

                    var collection = new VelibAddRemoveCollection();
                    collection.ToAdd = VelibDataSource.StaticVelibs.Where(t => !Items.Contains(t) 
                        && MapExtensions.Contains(mapLocations, t.Location)).Take(MAX_CONTROLS).ToList();
                    if (Items.Count > 50)
                        collection.ToAdd.Clear();
                    collection.ToRemove = Items.Where(t => !MapExtensions.Contains(mapLocations, t.Location)).ToList();


                    // precalculate the items offset (that deffer well calculation)
                    foreach (var velib in collection.ToAdd)
                    {
                        velib.GetOffsetLocation2(leftCornerLocation, zoomLevel);
                    }

                    

                    return collection;
                })
                .Switch()
                .Subscribe(async x =>
                {

                    if (x == null)
                        return;

                    RefreshView(x, cts.Token);

                });

            // requester
            new Task(async () =>
            {
                while (true)
                {
                    RequestAvailability();
                    await Task.Delay(5000);
                }

            }).Start();
            
        }
        private void GenerateMapItems()
        {
            dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
             {
                 for (int i = 0; i < MAX_CONTROLS; i++)
                 {
                     var item = new VelibControl(_map);
                    // item.CacheMode = new BitmapCache();
                     item.Template = ItemTemplate;
                     item.Opacity = 0;
                     velibControls.Add(item);
                     _map.Children.Add(item);
                 }
             });
        }


        private void AddToCollection(VelibModel velib)
        {
            bool added = false;
            // merge to other velib cluster if required
            // otherwise create a new cluster
            foreach (var allreadyAddedVelib in Items.ToList())
            {
                var control = allreadyAddedVelib.VelibControl;
                if (control == null)
                    continue;
                double distance = control.GetOffsetLocation2(leftCornerLocation, zoomLevel)
                    .GetDistanceTo(velib.GetOffsetLocation2(leftCornerLocation, zoomLevel));
                if (distance < MAXDISTANCE)
                {
                    control.AddVelib(velib);
                    added = true;
                    break;
                }
            }
            if (!added)
            {
                var mapItem = velibControls.FirstOrDefault(t => t.Velibs.Count == 0);
                // no more remaining controls
                if (mapItem == null)
                    return;
                mapItem.AddVelib(velib);
            }
            Items.Add(velib);
        }

        private void RemoveOutOfViewItems()
        {

        }

        double previousZoom;
        private async void RefreshView(VelibAddRemoveCollection addRemoveCollection, CancellationToken token)
        {
            
                if (token.IsCancellationRequested)
                {
                    return;
                }

                if (zoomLevel > 15)
                    MAXDISTANCE = 1;
                else
                    MAXDISTANCE = 100;
                
                // remove out of view items
                foreach (var velib in Items.Where(t => addRemoveCollection.ToRemove.Contains(t)).ToList())
                {
                    if (velib.VelibControl != null)
                        velib.VelibControl.RemoveVelib(velib);
                    Items.Remove(velib);
                    if (token.IsCancellationRequested)
                    {
                        return;
                    }
                }
                
                // refresh clusters by removing them from current cluster if required
                // and send them back to the ToAddPool to be retreated
                // (when zoom in)
                if (previousZoom < zoomLevel)
                {
                    foreach (var control in velibControls.Where(t => t.Velibs.Count > 1))
                    {
                        foreach (var velib in control.Velibs.Where(t => t.GetOffsetLocation2(leftCornerLocation, zoomLevel)
                            .GetDistanceTo(control.GetOffsetLocation2(leftCornerLocation, zoomLevel)) > MAXDISTANCE).ToList())
                        {
                                
                            addRemoveCollection.ToAdd.Add(velib);
                            Items.Remove(velib);
                            control.RemoveVelib(velib);

                            if (token.IsCancellationRequested)
                            {
                                return;
                            }
                        }
                    }
                }
                // (when dezoom)
                // refresh clusters clustering clusters
                if (previousZoom > zoomLevel) { 
                    foreach (var alreadyAddedVelib in Items.ToList())
                    {
                        if (alreadyAddedVelib.VelibControl == null)
                            continue;

                        Point locationOffset = alreadyAddedVelib.GetOffsetLocation2(leftCornerLocation, zoomLevel);
                        foreach (var alreadyAddedVelib2 in Items.Where(t=>t.Number != alreadyAddedVelib.Number ).ToList())
                        {
                            if (alreadyAddedVelib2.VelibControl == null)
                                continue;
                            var loc = alreadyAddedVelib2.GetOffsetLocation2(leftCornerLocation, zoomLevel);
                            double distance = loc.GetDistanceTo(locationOffset);
                            if (distance < MAXDISTANCE && alreadyAddedVelib.VelibControl != alreadyAddedVelib2.VelibControl)
                            {
                                // add the velib the ToAdd collection to be handled again
                                addRemoveCollection.ToAdd.Add(alreadyAddedVelib2);
                                Items.Remove(alreadyAddedVelib2);
                                alreadyAddedVelib2.VelibControl.RemoveVelib(alreadyAddedVelib2);

                            }
                            if (token.IsCancellationRequested)
                            {
                                return;
                            }
                        }
                   
                    }

                }

                

                previousZoom = zoomLevel;

                foreach (var velib in addRemoveCollection.ToAdd)
                {
                    AddToCollection(velib);
                    if (token.IsCancellationRequested)
                    {
                        return;
                    }
                }


       

            // finalise the ui cycle
            foreach (var control in velibControls.Where(c=>c.NeedRefresh))
            {
                await Task.Delay(TimeSpan.FromSeconds(0.02));
                if (token.IsCancellationRequested) {
                    break;
                }
                await dispatcher.RunAsync(CoreDispatcherPriority.Normal,() => control.FinaliseUiCycle(dispatcher,token));
            }

            foreach (var velib in Items)
            {
                 // reinit calculated location to refresh it next UI cycle
                    velib.OffsetLocation.X =0;
                    velib.VelibControl.Location = null;
                    velib.VelibControl.OffsetLocation.X = 0;
            }
            
        }
        public async void RequestAvailability()
        {
            foreach (var velib in velibControls.Where(c=>c.Velibs.Count == 1).Select(c=>c.Velibs.FirstOrDefault()).ToList())
            {
                velib.GetAvailableBikes(dispatcher);
            }
        }

    }
}
