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


namespace Velib.Common.Cluster
{
   public class ClustersGenerator
   {
       

        double MAXDISTANCE = 100;
        private readonly List<VelibControl> velibControls = new List<VelibControl>(30);
        private readonly List<VelibModel> items = new List<VelibModel>();
        private readonly List<VelibCluster> clusters = new List<VelibCluster>();
        private MapControl _map;
        private TimeSpan throttleTime = TimeSpan.FromMilliseconds(100);
        private CoreDispatcher dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;
        public ControlTemplate ItemTemplate { get; set; }
        public CancellationTokenSource cts = new CancellationTokenSource();

         GeoboundingBox mapArea = null;
                    double zoomLevel=20;

        public ClustersGenerator(MapControl map, ControlTemplate itemTemplate)
        {
            _map = map;
            GenerateMapItems();
            this.ItemTemplate = itemTemplate;
            // maps event
            //_map.ResolveCompleted += (s, e) => GeneratePushpins();
            //_map.ViewChanged += (s, e) => GeneratePushpins();
            //_map.ZoomLevelChanged += (s, e) => GeneratePushpins();
            //_map.CenterChanged += (s, e) => GeneratePushpins();
            var mapObserver = Observable.FromEventPattern(map, "CenterChanged");
            mapObserver
                //.SubscribeOn(TaskPoolScheduler.Default) //TaskPoolScheduler.Default
                //.ObserveOn(Scheduler.Default)
                //.Do<new Action()>()
                //{
                //    // Start the timer to know if user wait more than 3 seconds without results
                //    if (!NetworkInterface.GetIsNetworkAvailable())
                //    {
                //        //Deployment.Current.Dispatcher.BeginInvoke(() =>
                //        //{
                //           // MessageBox.Show("Problème de connexion, veuillez vérifier votre couverture réseau.");
                //        //});
                //        return;
                //    }
                //    if (vm.ServiceAgent != null)
                //    {
                //        //vm.ServiceAgent.QueryEvents(MapMain.TargetBoundingRectangle);
                //        vm.ServiceAgent.QueryEvts(MapMain.TargetBoundingRectangle);

                //    }
                //})
                .Throttle(throttleTime)
                // .ObserveOn(Scheduler.CurrentThread)
                // .SubscribeOn(Scheduler.Default)
                .Select(async x =>
                {
                    if (VelibDataSource.StaticVelibs == null)
                        return null;
                    //await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {

                    //    Debug.WriteLine("ZoomLevel" + _map.ZoomLevel);
                    //    Debug.WriteLine("mapWidth" + _map.ActualWidth);

                    //    var mapArea = ((MapControl)x.Sender).GetViewArea();

                    //    mapArea.NorthwestCorner.


                    //});
                    //Debug.WriteLine("ZoomLevel" + );



                    //await dispatcher.RunAsync(CoreDispatcherPriority.Normal, ()=>((MapControl)x.Sender).GetViewArea()))
                    await dispatcher.RunAsync(CoreDispatcherPriority.Low, () => {
                        mapArea = _map.GetViewArea();
                        zoomLevel = _map.ZoomLevel;
                    });

                    var collection = new VelibAddRemoveCollection();
                    collection.ToAdd = VelibDataSource.StaticVelibs.Where(t => !items.Contains(t) && MapExtensions.Contains(t.Location, mapArea)).Take(20).ToList();
                    if (items.Count > 50)
                        collection.ToAdd.Clear();
                    //collection.ToAdd.RemoveAll(t=> items.Contains(t));
                    //items.AddRange(collection.ToAdd);
                    collection.ToRemove = items.Where(t => !MapExtensions.Contains(t.Location, mapArea)).ToList();


                    // precalculate the items offset (that deffer well calculation)
                    foreach (var velib in collection.ToAdd)
                    {
                        velib.GetOffsetLocation2(mapArea.NorthwestCorner, zoomLevel);
                    }
                    

                    return collection;
                })
                .Switch()
                .Subscribe(async x =>
                {

                    if (x == null)
                        return;


                    // remove out of view items
                    foreach (var velib in items.Where(t => x.ToRemove.Contains(t)).ToList())
                    {
                        if (velib.VelibControl != null)
                            velib.VelibControl.RemoveVelib(velib);
                        items.Remove(velib);
                    }

                    RefreshView(x);

                 

                    //await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    //{
                    //    foreach (var item in x.ToRemove)
                    //    {
                    //        Velibs.Remove(item);
                    //    }
                    //});

                    //foreach (var item in x.ToAdd)
                    //{

                    //    if (!Velibs.Contains(item))
                    //    {
                    //        await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    //        {
                    //            Velibs.Add(item);
                    //            item.GetAvailableBikes();
                    //        });
                    //    }


                    //    await Task.Delay(TimeSpan.FromSeconds(0.02));
                    //    if (Velibs.Count >= 50)
                    //        break;
                    //}

                });
            

        }
        private void GenerateMapItems(int max = 30)
        {
            dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
             {
                 for (int i = 0; i < max; i++)
                 {
                     var item = new VelibControl();
                     item.Template = ItemTemplate;
                     velibControls.Add(item);
                     _map.Children.Add(item);
                 }
             });
        }


        private void AddToCollection(VelibModel velib)
        {
            bool added = false;
            new Task(() => velib.GetAvailableBikes(dispatcher)).Start();
            // merge to other velib cluster if required
            // otherwise create a new cluster
            foreach (var allreadyAddedVelib in items)
            {
                double distance = allreadyAddedVelib.VelibControl.GetOffsetLocation(_map).GetDistanceTo(velib.GetOffsetLocation(_map));
                if (distance < MAXDISTANCE)
                {
                    allreadyAddedVelib.VelibControl.AddVelib(velib);
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
            items.Add(velib);
        }

        double previousZoom;
        private async void RefreshView(VelibAddRemoveCollection addRemoveCollection)
        {
            

           //Task.Delay(TimeSpan.FromSeconds(0.02));   
            await dispatcher.RunAsync(CoreDispatcherPriority.Low,() =>
            {
               
                if (_map.ZoomLevel > 16)
                    MAXDISTANCE = 1;
                else
                    MAXDISTANCE = 100;

                
                //foreach (var velib in addRemoveCollection.ToRemove.Where)
                //{
                //    var searchVelib = items.FirstOrDefault(s => s.Number == velib.Number);
                //    if (searchVelib != null && searchVelib.VelibControl != null)
                //    {
                //        searchVelib.VelibControl.RemoveVelib(searchVelib);
                //    }
                //    items.Remove(searchVelib);
                //}
                // refresh clusters by removing them from current cluster if required
                // and send them back to the ToAddPool to be retreated
                // (when zoom in)
                if (previousZoom < _map.ZoomLevel)
                {
                    foreach (var control in velibControls.Where(t => t.Velibs.Count > 1))
                    {
                        foreach (var velib in control.Velibs.Where(t => t.GetOffsetLocation(_map)
                            .GetDistanceTo(control.GetOffsetLocation(_map)) > MAXDISTANCE).ToList())
                        {
                                
                            //Point locationOffset = velib.GetMapLocation(_map);
                            //double distance = control.GetMapLocation(_map).GetDistanceTo(locationOffset);
                            //if (distance > MAXDISTANCE)
                            //{
                            // add the velib the ToAdd collection to be handled again
                            addRemoveCollection.ToAdd.Add(velib);
                            items.Remove(velib);
                            control.RemoveVelib(velib);

                            //}
                        }
                    }
                }
                // (when dezoom)
                // refresh clusters clustering clusters
                if (previousZoom > _map.ZoomLevel) { 
                    foreach (var alreadyAddedVelib in items.ToList())
                    {
                        if (alreadyAddedVelib.VelibControl == null)
                            continue;

                        Point locationOffset = alreadyAddedVelib.GetOffsetLocation(_map);
                        foreach (var alreadyAddedVelib2 in items.Where(t=>t.Number != alreadyAddedVelib.Number ).ToList())
                        {
                            if (alreadyAddedVelib2.VelibControl == null)
                                continue;
                            var loc = alreadyAddedVelib2.GetOffsetLocation(_map);
                            double distance = loc.GetDistanceTo(locationOffset);
                            if (distance < MAXDISTANCE && alreadyAddedVelib.VelibControl != alreadyAddedVelib2.VelibControl)
                            {
                                // add the velib the ToAdd collection to be handled again
                                addRemoveCollection.ToAdd.Add(alreadyAddedVelib2);
                                items.Remove(alreadyAddedVelib2);
                                alreadyAddedVelib2.VelibControl.RemoveVelib(alreadyAddedVelib2);

                            }
                        }
                   
                    }

                }

                

                previousZoom = _map.ZoomLevel;

                foreach (var velib in addRemoveCollection.ToAdd)
                {
                    AddToCollection(velib);
                }
            });


       

            // finalise the ui cycle
            foreach (var control in velibControls.Where(c=>c.NeedRefresh))
            {
                Task.Delay(TimeSpan.FromSeconds(0.02));
                await dispatcher.RunAsync(CoreDispatcherPriority.High,() => control.FinaliseUiCycle());
            }

            foreach (var velib in items)
            {
                 // reinit calculated location to refresh it next UI cycle
                    velib.OffsetLocation.X =0;
                    velib.VelibControl.Location = null;
                    velib.VelibControl.OffsetLocation.X = 0;
            }


            //    if (_map.ZoomLevel > 15)
            //        MAXDISTANCE = 1;
            //    else
            //        MAXDISTANCE = 100;
            //    foreach (var mapItem in mapItemPool.Where(t => t.Items.Count >0))
            //    {
            //        mapItem.Items.RemoveAll(a => addRemoveCollection.ToRemove.Contains(a));
            //    }
         
            //    foreach (var mapItem in mapItemPool.Where(t => t.Items.Count >= 1))
            //    {
            //        // split cluster no more clusters
            //        for (int i = 0; i < mapItem.Items.Count; i++)
            //        {
            //            Point locationOffset;
            //            _map.GetOffsetFromLocation(mapItem.Items[i].Location, out locationOffset);
            //            double distance = mapItem.MapLocation.GetDistanceTo(locationOffset);

            //            if (distance > MAXDISTANCE)
            //            {
            //                addRemoveCollection.ToAdd.Add(mapItem.Items[i]);
            //                mapItem.Items.Remove(mapItem.Items[i]);
                            
                            
            //            }
            //        }



            //    }
             
            //// creation des nouveaux clusters
            //foreach (var velib in addRemoveCollection.ToAdd)
            //{
            //    Point locationOffset;
            //    _map.GetOffsetFromLocation(velib.Location, out locationOffset);
            //    bool added = false;
            //    foreach (var mapItem in mapItemPool.Where(t=>t.Items.Count>=1))
            //    {

            //       mapItem.MapLocation = mapItem.GetCenter(_map);
            //       double distance = mapItem.MapLocation.GetDistanceTo(locationOffset);

            //        if (distance < MAXDISTANCE)
            //        {
            //            mapItem.Items.Add(velib);
            //            added = true;
            //            break;
            //        }
            //    }
            //    if (!added)
            //    {
            //        var mapItem = mapItemPool.First(t => t.Items.Count == 0);
                   
            //        mapItem.Items.Add(velib);
            //    }
            //}
            //    });
            //for (int i = 0; i < mapItemPool.Count; i++)
            //{

            //    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            //    {
            //        if (mapItemPool[i].Items.Count > 0)
            //        {
            //            mapItemPool[i].MapLocation = mapItemPool[i].GetCenter(_map);
            //            Geopoint location;
            //            _map.GetLocationFromOffset(mapItemPool[i].MapLocation, out location);
            //            mapItemPool[i].SetValue(MapControl.LocationProperty, location) ;
            //            //mapItemPool[i].DataContext = pushpinsToAdd[i];
            //            if (mapItemPool[i].Items.Count > 1)
            //            {
            //                mapItemPool[i].ShowCluster();
            //            }
            //            else
            //            {
            //                mapItemPool[i].ShowVelib();
            //                (mapItemPool[i].Items.First() as VelibModel).GetAvailableBikes();
            //            }

            //        }
            //        else
            //        {
            //            // hide remaining map items
            //            mapItemPool[i].Hide();

            //        }
            //    });
            //    Task.Delay(TimeSpan.FromSeconds(0.02));
            //}
            // remove already present items
            //addRemoveCollection.ToAdd.RemoveAll(a => items.Contains(a));
            //items.AddRange(addRemoveCollection.ToAdd);
            //items.RemoveAll(a => addRemoveCollection.ToRemove.Contains(a));

            //List<VelibCluster> clusters = new List<VelibCluster>();
            //// creation des nouveaux clusters
            //foreach (var velib in addRemoveCollection.ToAdd)
            //{
            //    bool addGroup = true;
            //    Point locationOffset;
            //    _map.GetOffsetFromLocation((velib).Location, out locationOffset);

            //    var newGroup = new VelibCluster(velib, locationOffset);

            //    foreach (var pushpinToAdd in clusters)
            //    {
            //        double distance = pushpinToAdd.MapLocation.GetDistanceTo(newGroup.MapLocation);

            //        if (distance < MAXDISTANCE)
            //        {
            //            pushpinToAdd.IncludeGroup(newGroup);
            //            addGroup = false;
            //            break;
            //        }
            //    }

            //    if (addGroup)
            //        clusters.Add(newGroup);
            //}


            //await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            //      {
            //          foreach (var cluster in clusters.ToList())
            //          {
            //              foreach (var velib in cluster.Items.ToList())
            //              {
            //                  foreach (var mapItem in mapItemPool.Where(t => t.Cluster.Items.Count > 0))
            //                  {
            //                      if (mapItem.Cluster.Items.Contains(velib))
            //                      {
            //                          mapItem.alreadyHandled = true;
            //                          cluster.Items.Remove(velib);
            //                          if (cluster.Items.Count == 0)
            //                          {
            //                              clusters.Remove(cluster);
            //                          }
            //                          break;
            //                        // add other velib in cluster where a velib already exist and break
            //                      }
            //                  }
            //              }
            //          }
            //      });
            //int j = 0;
            //for (int i = 0; i < mapItemPool.Count; i++)
            //{
            //    j++;
            //    if (mapItemPool[i].alreadyHandled)
            //    {
            //        mapItemPool[i].alreadyHandled = false;
            //        j--;
            //        continue;
            //    }

            //    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            //      {
            //          // remove controls velibs that requires to be remove
            //        //foreach (var mapItem in mapItemPool.Where(c => c.Cluster.Items.Count > 0))
            //        //{
            //        //    mapItem.Cluster.Items.RemoveAll(t => addRemoveCollection.ToRemove.Contains(t));
            //        //}
            //          if (clusters.Count >j)
            //          {


            //              mapItemPool[i].SetValue(MapControl.LocationProperty, (clusters[j].GetFirstItem() as VelibModel).Location);
            //              mapItemPool[i].DataContext = clusters[j];
            //              mapItemPool[i].Cluster = clusters[j];
            //              if (clusters[j].Count > 1)
            //              {

            //                  mapItemPool[i].ShowCluster();
            //              }
            //              else
            //              {
            //                  mapItemPool[i].ShowVelib();
            //                  (clusters[j].GetFirstItem() as VelibModel).GetAvailableBikes();
            //              }

            //          }
            //          else
            //          {
            //              // hide remaining map items
            //              mapItemPool[i].Hide();

            //          }
            //      });
            //    Task.Delay(TimeSpan.FromSeconds(0.02));
        //}
            
            
            
        }
    }
}
