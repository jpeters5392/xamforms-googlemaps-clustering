using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using DynamicData;
using googlemapsforms.Models;
using Xamarin.Forms;

namespace googlemapsforms
{
    public class FormsMapPage : ContentPage
    {
        private const int Number = 20;
        private static double currentLat = 45.4473;
        private static double currentLon = -96.7283;
        private double[] lats = Enumerable.Range(0, Number)
                .Select(i => {
                    var newLat = currentLat;
                    currentLat += 0.1;
                    return newLat;
                }).ToArray();
        private double[] lons = Enumerable.Range(0, Number)
                .Select(i => {
                    var newLon = currentLon;
                    currentLon += 0.1;
                    return newLon;
                }).ToArray();
        private FormsMapView formsMapView;

        public FormsMapPage()
        {
            formsMapView = new FormsMapView();
            formsMapView.MapReady.Subscribe(tick =>
            {
                formsMapView.Markers.AddOrUpdate(CreateMarkers());

                Observable.Interval(TimeSpan.FromSeconds(1)).Take(10).Subscribe(_ =>
                {
                    formsMapView.Markers.AddOrUpdate(UpdateMarkers());
                });

                Observable.Timer(TimeSpan.FromMilliseconds(11000)).Subscribe(_ =>
                {
                    formsMapView.Markers.RemoveKeys(DeleteMarkers());
                });
            });

            Content = formsMapView;
        }

        private IEnumerable<FormsClusterMarker> CreateMarkers()
        {
            var markers = new List<FormsClusterMarker>();

            return Enumerable.Range(0, Number)
                .Select(i => {
                    var newLat = lats[i];
                    var newLon = lons[i];
                    return new FormsClusterMarker
                    {
                        Latitude = newLat,
                        Longitude = newLon,
                        Id = i.ToString(),
                        Title = $"Marker{i}",
                        Snippet = $"Marker{i} Snippet"
                    };
                });
        }

        private IEnumerable<FormsClusterMarker> UpdateMarkers()
        {
            var markers = new List<FormsClusterMarker>();

            return Enumerable.Range(1, Number - 1)
                .Select(i => {
                    var item = formsMapView.Markers.Items.FirstOrDefault(x => x.Id == i.ToString());
                    if (item != null)
                    {
                        return new FormsClusterMarker
                        {
                            Latitude = item.Latitude + 0.1,
                            Longitude = item.Longitude - 0.1,
                            Id = i.ToString(),
                            Title = $"Marker{i}",
                            Snippet = $"Marker{i} Snippet"
                        };
                    }

                    return null;
                })
                .Where(x => x != null);
        }

        private IEnumerable<string> DeleteMarkers()
        {
            var markers = new List<FormsClusterMarker>();

            return Enumerable.Range(0, Number)
                .Where(x => x % 2 == 0)
                .Select(i => i.ToString());
        }
    }
}

