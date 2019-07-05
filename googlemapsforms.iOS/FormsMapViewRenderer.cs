using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using CoreGraphics;
using CoreLocation;
using DynamicData;
using Foundation;
using Google.Maps;
using Google.Maps.Utils;
using googlemapsforms;
using googlemapsforms.iOS;
using googlemapsforms.Models;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(FormsMapView), typeof(FormsMapViewRenderer))]
namespace googlemapsforms.iOS
{
    public class FormsMapViewRenderer : ViewRenderer<FormsMapView, MapView>
    {
        private GMUClusterManager clusterManager;
        private MapDelegate mapDelegate;
        private readonly CompositeDisposable markerSubscription = new CompositeDisposable();

        private readonly Dictionary<string, ClusterMarker> renderedMarkers = new Dictionary<string, ClusterMarker>();

        protected override void OnElementChanged(ElementChangedEventArgs<FormsMapView> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                // Unsubscribe from event handlers and cleanup any resources
            }

            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    // Instantiate the native control and assign it to the Control property with
                    // the SetNativeControl method
                    var camera = CameraPosition.FromCamera(latitude: 45.4473, longitude: -96.7283, zoom: 6);
                    var mapView = MapView.FromCamera(CGRect.Empty, camera);
                    mapView.MyLocationEnabled = true;
                    mapView.Settings.CompassButton = true;
                    mapView.Settings.ZoomGestures = true;

                    SetNativeControl(mapView);
                }

                // Configure the control and subscribe to event handlers
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                markerSubscription.Clear();

                clusterManager?.Dispose();
                mapDelegate?.Dispose();

                clusterManager = null;
                mapDelegate = null;

                foreach (var item in renderedMarkers)
                {
                    item.Value.Dispose();
                }

                renderedMarkers.Clear();
            }
        }

        public override void MovedToWindow()
        {
            base.MovedToWindow();

            if (mapDelegate == null)
            {
                mapDelegate = new MapDelegate(Control);
            }

            if (clusterManager == null)
            {
                var iconGenerator = new GMUDefaultClusterIconGenerator();
                var algorithm = new GMUNonHierarchicalDistanceBasedAlgorithm();
                var renderer = new GMUDefaultClusterRenderer(Control, iconGenerator) { WeakDelegate = mapDelegate };
                clusterManager = new GMUClusterManager(Control, algorithm, renderer);
                clusterManager.SetDelegate(mapDelegate, mapDelegate);

                SubscribeToMarkers();

                Element.SetMapReady(DateTime.UtcNow.Ticks);
            }
        }

        private void SubscribeToMarkers()
        {
            markerSubscription.Clear();
            markerSubscription.Add(
                Element.Markers.Connect()
                .Synchronize()
                .Subscribe(HandleMarkerUpdates)
            );
        }

        private void HandleMarkerUpdates(IChangeSet<FormsClusterMarker, string> changes)
        {

            Device.BeginInvokeOnMainThread(() =>
            {
                
                foreach (var change in changes)
                {
                    if (change.Reason == ChangeReason.Add)
                    {
                        var clusterMarker = ClusterMarker.FromFormsClusterMarker(change.Current);
                        renderedMarkers.Add(change.Current.Id, clusterMarker);
                        clusterManager.AddItem(clusterMarker);
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
                        var clusterMarker = ClusterMarker.FromFormsClusterMarker(change.Current);
                        renderedMarkers[change.Key].Position = clusterMarker.Position;
                    }
                }

                clusterManager.Cluster();
            });
        }

        internal class MapDelegate : MapViewDelegate, IGMUClusterManagerDelegate, IGMUClusterRendererDelegate
        {
            private MapView mapView;

            public MapDelegate(MapView mapView)
            {
                this.mapView = mapView;
            }
            public override void DidTapAtCoordinate(MapView mapView, CLLocationCoordinate2D coordinate)
            {
                Console.WriteLine(string.Format("Tapped at location: ({0}, {1})", coordinate.Latitude, coordinate.Longitude));
            }

            public override bool TappedMarker(MapView mapView, Marker marker)
            {
                if (marker.UserData is ClusterMarker)
                {
                    Console.WriteLine("Did tap marker for cluster item " + ((ClusterMarker)marker.UserData).Title);
                }
                else
                {
                    Console.WriteLine("Did tap a normal marker");
                }
                return false;
            }

            [Export("clusterManager:didTapCluster:")]
            public bool DidTapCluster(GMUClusterManager clusterManager, IGMUCluster cluster)
            {
                // zoom to expand the cluster
                var bounds = new CoordinateBounds(cluster.Position, cluster.Position);
                foreach (var item in cluster.Items)
                {
                    bounds = bounds.Including(item.Position);
                }

                try
                {
                    var cameraUpdate = CameraUpdate.FitBounds(bounds, 100);
                    mapView.MoveCamera(cameraUpdate);
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }

                return true;
            }

            [Export("renderer:willRenderMarker:")]
            public void WillRenderMarker(IGMUClusterRenderer renderer, Marker marker)
            {
                if (marker.UserData is ClusterMarker)
                {
                    marker.Title = ((ClusterMarker)marker.UserData).Title;
                }
            }
        }
    }
}
