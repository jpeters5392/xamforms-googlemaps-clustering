using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using DynamicData;
using googlemapsforms.Models;
using Xamarin.Forms;

namespace googlemapsforms
{
    public class FormsMapView : ContentView
    {
        public static readonly BindableProperty MarkersProperty = BindableProperty.Create("Markers", typeof(ISourceCache<FormsClusterMarker, string>), typeof(FormsMapView), new SourceCache<FormsClusterMarker, string>(x => x.Id));

        public ISourceCache<FormsClusterMarker, string> Markers
        {
            get { return (ISourceCache<FormsClusterMarker, string>)GetValue(MarkersProperty); }
            set { SetValue(MarkersProperty, value); }
        }

        private Subject<long> _mapReady;
        public IObservable<long> MapReady => _mapReady.AsObservable();

        public FormsMapView()
        {
            _mapReady = new Subject<long>();
        }

        public void SetMapReady(long ticks)
        {
            _mapReady.OnNext(ticks);
        }
    }
}

