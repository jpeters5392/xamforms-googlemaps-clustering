using System;
using System.Collections.Generic;
using System.Linq;
using Android.OS;

namespace googlemapsforms.Droid
{
    public interface IReceiveActivityLifecycleEvents
    {
        void OnCreate(Bundle savedInstanceState);
        void OnStart();
        void OnResume();
        void OnPause();
        void OnStop();
        void OnDestroy();
        void OnSaveInstanceState(Bundle outState);
        void OnLowMemory();
    }

    public static class ActivityLifecycleNotifier
    {
        public static Bundle LastActivityCreateBundle { get; private set; }
        private static List<IReceiveActivityLifecycleEvents> listeners = new List<IReceiveActivityLifecycleEvents>();
        public static void Register(IReceiveActivityLifecycleEvents listener)
        {
            // TODO: create a replayable subscription that gets reset when OnPause/OnDestroy/OnStop are invoked
            // that way each new subscriber will receive the latest create/start/resume when they subscribe
            listeners.Add(listener);
        }

        public static void Unregister(IReceiveActivityLifecycleEvents listener)
        {
            var oldListener = listeners.FirstOrDefault(x => x == listener);
            if (oldListener != null)
            {
                listeners.Remove(oldListener);
            }
        }

        public static void OnCreate(Bundle savedInstanceState)
        {
            LastActivityCreateBundle = savedInstanceState;
            listeners.ForEach(listener => listener.OnCreate(savedInstanceState));
        }
        public static void OnStart()
        {
            listeners.ForEach(listener => listener.OnStart());
        }
        public static void OnResume()
        {
            listeners.ForEach(listener => listener.OnResume());
        }
        public static void OnPause()
        {
            listeners.ForEach(listener => listener.OnPause());
        }
        public static void OnStop()
        {
            listeners.ForEach(listener => listener.OnStop());
        }
        public static void OnDestroy()
        {
            listeners.ForEach(listener => listener.OnDestroy());
        }
        public static void OnSaveInstanceState(Bundle outState)
        {
            listeners.ForEach(listener => listener.OnSaveInstanceState(outState));
        }
        public static void OnLowMemory()
        {
            listeners.ForEach(listener => listener.OnLowMemory());
        }
    }
}
