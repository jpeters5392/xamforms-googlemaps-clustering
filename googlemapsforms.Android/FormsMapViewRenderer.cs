using System;
using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Utils.Clustering;
using Android.OS;
using Android.Views;
using googlemapsforms;
using googlemapsforms.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(FormsMapView), typeof(FormsMapViewRenderer))]
namespace googlemapsforms.Droid
{
    
    public class FormsMapViewRenderer : ViewRenderer<FormsMapView, MapView>, IReceiveActivityLifecycleEvents
    {
        public FormsMapViewRenderer(Context context) : base(context)
        {
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
                    Control.GetMapAsync(new NativeMap(Context));
                    //Control.OnCreate(ActivityLifecycleNotifier.LastActivityCreateBundle);
                    //Control.OnResume();                    
                }

                // Configure the control and subscribe to event handlers
                ActivityLifecycleNotifier.Register(this);
                Control.LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent);
            }
        }
    }
}
