using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using CoreGraphics;
using CoreLocation;
using Foundation;
using Google.Maps;
using Google.Maps.Utils;
using googlemapsforms;
using googlemapsforms.iOS;
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

                Task.Delay(2000).ContinueWith(OnTimerElapsed);                

                // Configure the control and subscribe to event handlers
            }
        }

        private void OnTimerElapsed(Task task)
        {
            try
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    mapDelegate = new MapDelegate(Control);
                    var iconGenerator = new GMUDefaultClusterIconGenerator();
                    var algorithm = new GMUNonHierarchicalDistanceBasedAlgorithm();
                    var renderer = new GMUDefaultClusterRenderer(Control, iconGenerator) { WeakDelegate = mapDelegate };
                    clusterManager = new GMUClusterManager(Control, algorithm, renderer);
                    clusterManager.SetDelegate(mapDelegate, mapDelegate);

                    var markers = CreateMarkers();
                    clusterManager.AddItems(markers);
                    clusterManager.Cluster();
                });
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private IGMUClusterItem[] CreateMarkers()
        {
            var startingLat = 45.4473;
            var startingLon = -96.7283;
            var markers = new List<IGMUClusterItem>();

            for (var i = 0; i < 20; i++)
            {
                markers.Add(new ClusterMarker(new CLLocationCoordinate2D(startingLat, startingLon), $"Marker{i}", $"Marker{i} Snippet"));
                startingLat += 0.1;
                startingLon -= 0.1;
            }

            return markers.ToArray();
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
