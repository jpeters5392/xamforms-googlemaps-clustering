using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Gms.Maps.Utils.Clustering;
using Android.OS;
using DynamicData;
using googlemapsforms;
using googlemapsforms.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(FormsMapView), typeof(FormsMapViewRenderer))]
namespace googlemapsforms.Droid
{
    
    public class FormsMapViewRenderer : ViewRenderer<FormsMapView, MapView>, IReceiveActivityLifecycleEvents, IOnMapReadyCallback, ClusterManager.IOnClusterClickListener, ClusterManager.IOnClusterItemClickListener, ClusterManager.IOnClusterInfoWindowClickListener, ClusterManager.IOnClusterItemInfoWindowClickListener
    {
        private GoogleMap googleMap;
        private ClusterManager clusterManager;
        private readonly CompositeDisposable markerSubscription = new CompositeDisposable();

        private readonly Dictionary<string, ClusterMarker> renderedMarkers = new Dictionary<string, ClusterMarker>();

        public FormsMapViewRenderer(Context context) : base(context)
        {
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                markerSubscription.Clear();

                clusterManager?.Dispose();
                clusterManager = null;

                googleMap?.Dispose();
                googleMap = null;

                foreach (var item in renderedMarkers)
                {
                    item.Value.Dispose();
                }

                renderedMarkers.Clear();
            }
        }

        public void OnCreate(Bundle savedInstanceState)
        {
            Control.OnCreate(savedInstanceState);
        }

        public void OnDestroy()
        {
            Control.OnDestroy();
        }

        public void OnLowMemory()
        {
            Control.OnLowMemory();
        }

        public void OnPause()
        {
            Control.OnPause();
        }

        public void OnResume()
        {
            Control.OnResume();
        }

        public void OnSaveInstanceState(Bundle outState)
        {
            Control.OnSaveInstanceState(outState);
        }

        public void OnStart()
        {
            Control.OnStart();
        }

        public void OnStop()
        {
            Control.OnStop();
        }

        protected override void OnElementChanged(ElementChangedEventArgs<FormsMapView> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                // Unsubscribe from event handlers and cleanup any resources
                ActivityLifecycleNotifier.Unregister(this);
            }

            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    // Instantiate the native control and assign it to the Control property with
                    // the SetNativeControl method
                    var mapView = new MapView(Context);
                    SetNativeControl(mapView);
                    //Control.OnCreate(ActivityLifecycleNotifier.LastActivityCreateBundle);
                    //Control.OnResume();                    
                }

                Control.GetMapAsync(this);
                // Configure the control and subscribe to event handlers
                ActivityLifecycleNotifier.Register(this);
                Control.LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent);
            }
        }

        private void HandleMarkerUpdates(IChangeSet<ClusterMarker, string> changes)
        {
            
            Device.BeginInvokeOnMainThread(() =>
            {
                foreach (var change in changes)
                {
                    if (change.Reason == ChangeReason.Add)
                    {
                        renderedMarkers.Add(change.Current.Id, change.Current);
                        clusterManager.AddItem(change.Current);
                    }
                    else if (change.Reason == ChangeReason.Moved)
                    {
                        // since we aren't sorting we can ignore moves
                    }
                    else if (change.Reason == ChangeReason.Refresh)
                    {
                        // ignore refresh
                    }
                    else if (change.Reason == ChangeReason.Remove)
                    {
                        clusterManager.RemoveItem(renderedMarkers[change.Key]);
                        renderedMarkers[change.Key].Dispose();
                        renderedMarkers.Remove(change.Key);
                    }
                    else if (change.Reason == ChangeReason.Update)
                    {
                        var origMarker = clusterManager.MarkerCollection.Markers.FirstOrDefault(x => x.Title == change.Current.Title);
                        if (origMarker != null)
                        {
                            origMarker.Position = change.Current.Position;
                            renderedMarkers[change.Key] = change.Current;
                        }
                        else
                        {
                            clusterManager.RemoveItem(renderedMarkers[change.Key]);
                            renderedMarkers[change.Key] = change.Current;
                            clusterManager.AddItem(change.Current);
                        }
                    }
                }

                clusterManager.Cluster();
            });            
        }

        public bool OnClusterClick(ICluster p0)
        {
            // zoom to expand the cluster
            LatLngBounds.Builder builder = new LatLngBounds.Builder();
            foreach (var item in p0.Items)
            {
                var clusterMarker = item as ClusterMarker;
                if (clusterMarker.Position != null)
                {
                    builder.Include(clusterMarker.Position);
                }
            }
            LatLngBounds bounds = builder.Build();
            try
            {
                googleMap.AnimateCamera(CameraUpdateFactory.NewLatLngBounds(bounds, 100));
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }

            return true;
        }

        public void OnClusterInfoWindowClick(ICluster p0)
        {

        }

        public bool OnClusterItemClick(Java.Lang.Object p0)
        {
            return false;
        }

        public void OnClusterItemInfoWindowClick(Java.Lang.Object p0)
        {

        }

        private void SubscribeToMarkers()
        {
            markerSubscription.Clear();
            markerSubscription.Add(
                Element.Markers.Connect()
                .Transform(change => ClusterMarker.FromFormsClusterMarker(change))
                .Synchronize()
                .Subscribe(HandleMarkerUpdates)
            );
        }

        public void OnMapReady(GoogleMap map)
        {
            try
            {
                googleMap = map;
                googleMap.UiSettings.ZoomControlsEnabled = true;
                googleMap.UiSettings.CompassEnabled = true;

                clusterManager = new ClusterManager(Context, googleMap);
                clusterManager.SetAnimation(false);
                googleMap.SetOnCameraIdleListener(clusterManager);
                googleMap.SetOnMarkerClickListener(clusterManager);
                googleMap.SetOnInfoWindowClickListener(clusterManager);
                googleMap.MoveCamera(CameraUpdateFactory.NewLatLng(new LatLng(45.4473, -96.7283)));

                clusterManager.SetOnClusterClickListener(this);
                clusterManager.SetOnClusterItemClickListener(this);
                clusterManager.SetOnClusterInfoWindowClickListener(this);
                clusterManager.SetOnClusterItemInfoWindowClickListener(this);

                SubscribeToMarkers();

                Element.SetMapReady(DateTime.UtcNow.Ticks);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
