using System;
using Android.Gms.Maps.Model;
using Android.Gms.Maps.Utils.Clustering;
using googlemapsforms.Models;

namespace googlemapsforms.Droid
{
    public class ClusterMarker : Java.Lang.Object, IClusterItem
    {
        public LatLng Position { get; set; }

        public string Snippet { get; set; }

        public string Title { get; set; }

        public string Id { get; set; }

        
        public ClusterMarker(IntPtr handle, Android.Runtime.JniHandleOwnership transfer)
        : base(handle, transfer)
        {
        }
        
        public ClusterMarker(LatLng position)
        {
            Position = position;
        }

        public ClusterMarker(LatLng position, string id, string title, string snippet) : this(position)
        {
            Id = id;
            Title = title;
            Snippet = snippet;
        }

        public static ClusterMarker FromFormsClusterMarker(FormsClusterMarker marker)
        {
            var position = new LatLng(marker.Latitude, marker.Longitude);
            return new ClusterMarker(position, marker.Id, marker.Title, marker.Snippet);
        }
    }
}
