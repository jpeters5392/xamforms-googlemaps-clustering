using System;
using System.Collections.Generic;
using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Gms.Maps.Utils.Clustering;
using Xamarin.Forms;

namespace googlemapsforms.Droid
{
    public class NativeMap : Java.Lang.Object, IOnMapReadyCallback, ClusterManager.IOnClusterClickListener, ClusterManager.IOnClusterItemClickListener, ClusterManager.IOnClusterInfoWindowClickListener, ClusterManager.IOnClusterItemInfoWindowClickListener
    {
        private Context _context;
        private GoogleMap googleMap;
        public NativeMap(Context context)
        {
            _context = context;
        }

        public bool OnClusterClick(ICluster p0)
        {
            // zoom to expand the cluster
            LatLngBounds.Builder builder = new LatLngBounds.Builder();
            foreach (var item in p0.Items)
                builder.Include((item as ClusterMarker).Position);
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

        public void OnMapReady(GoogleMap map)
        {
            try
            {
                googleMap = map;
                googleMap.UiSettings.ZoomControlsEnabled = true;
                googleMap.UiSettings.CompassEnabled = true;

                var clusterManager = new ClusterManager(_context, googleMap);
                googleMap.SetOnCameraIdleListener(clusterManager);
                googleMap.SetOnMarkerClickListener(clusterManager);
                googleMap.SetOnInfoWindowClickListener(clusterManager);
                googleMap.MoveCamera(CameraUpdateFactory.NewLatLng(new Android.Gms.Maps.Model.LatLng(45.4473, -96.7283)));

                clusterManager.SetOnClusterClickListener(this);
                clusterManager.SetOnClusterItemClickListener(this);
                clusterManager.SetOnClusterInfoWindowClickListener(this);
                clusterManager.SetOnClusterItemInfoWindowClickListener(this);


                clusterManager.AddItems(CreateMarkers());

                clusterManager.Cluster();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private System.Collections.ICollection CreateMarkers()
        {
            var startingLat = 45.4473;
            var startingLon = -96.7283;
            var markers = new List<ClusterMarker>();

            for (var i = 0; i < 20; i++)
            {
                markers.Add(new ClusterMarker(new LatLng(startingLat, startingLon), $"Marker{i}", $"Marker{i} Snippet"));
                startingLat += 0.1;
                startingLon -= 0.1;
            }

            return markers;
        }
    }
}
