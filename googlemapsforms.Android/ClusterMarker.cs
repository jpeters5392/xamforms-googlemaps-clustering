using System;
using Android.Gms.Maps.Model;
using Android.Gms.Maps.Utils.Clustering;

namespace googlemapsforms.Droid
{
    public class ClusterMarker : Java.Lang.Object, IClusterItem
    {
        public LatLng Position { get; set; }

        public string Snippet { get; set; }

        public string Title { get; set; }

        public ClusterMarker(LatLng position)
        {
            Position = position;
        }

        public ClusterMarker(LatLng position, string title, string snippet) : this(position)
        {
            Title = title;
            Snippet = snippet;
        }
    }
}
