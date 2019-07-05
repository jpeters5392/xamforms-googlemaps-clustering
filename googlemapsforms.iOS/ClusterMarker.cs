using System;
using CoreLocation;
using Google.Maps;
using Google.Maps.Utils;

namespace googlemapsforms.iOS
{
    public class ClusterMarker : Marker, IGMUClusterItem
    {
        public ClusterMarker(CLLocationCoordinate2D position, string title, string snippet)
        {
            Title = title;
            Position = position;
            Snippet = snippet;
        }
    }
}
