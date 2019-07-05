using System;
using CoreLocation;
using Google.Maps;
using Google.Maps.Utils;
using googlemapsforms.Models;

namespace googlemapsforms.iOS
{
    public class ClusterMarker : Marker, IGMUClusterItem
    {
        public string Id { get; }

        public ClusterMarker(CLLocationCoordinate2D position, string id, string title, string snippet)
        {
            Title = title;
            Position = position;
            Snippet = snippet;
            Id = id;
        }

        public static ClusterMarker FromFormsClusterMarker(FormsClusterMarker marker)
        {
            return new ClusterMarker(new CLLocationCoordinate2D(marker.Latitude, marker.Longitude), marker.Id, marker.Title, marker.Snippet);
        }
    }
}
