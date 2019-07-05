using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace googlemapsforms.Droid
{
    [Activity(Label = "googlemapsforms", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());

            ActivityLifecycleNotifier.OnCreate(savedInstanceState);
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnStart()
        {
            base.OnStart();

            ActivityLifecycleNotifier.OnStart();
        }
        protected override void OnResume()
        {
            base.OnResume();
            ActivityLifecycleNotifier.OnResume();
        }
        protected override void OnPause()
        {
            base.OnPause();
            ActivityLifecycleNotifier.OnPause();
        }
        protected override void OnStop()
        {
            base.OnStop();
            ActivityLifecycleNotifier.OnStop();
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            ActivityLifecycleNotifier.OnDestroy();
        }
        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            ActivityLifecycleNotifier.OnSaveInstanceState(outState);
        }
        public override void OnLowMemory()
        {
            base.OnLowMemory();
            ActivityLifecycleNotifier.OnLowMemory();
        }
    }
}