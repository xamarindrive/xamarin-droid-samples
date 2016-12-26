using Android.App;
using Android.Widget;
using Android.OS;
using PanoramaImageView.Lib;
using System;

namespace PanoramaImageView
{
    [Activity(Label = "PanoramaImageView", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private GyroscopeObserver gyroscopeObserver;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView (Resource.Layout.Main);

            gyroscopeObserver = new GyroscopeObserver();
            gyroscopeObserver.setMaxRotateRadian(Math.PI / 9);

            PanoramaImageView.Lib.PanoramaImageView image = FindViewById<PanoramaImageView.Lib.PanoramaImageView>(Resource.Id.panoramaImageView1);
            image.SetOnPanoramaScrollListener(new PanoramaScrollListener()
            {

            });
            image.mEnablePanoramaMode = true;            
            image.SetEnableScrollbar(true);
            image.SetInvertScrollDirection(false);
            image.SetGyroscopeObserver(gyroscopeObserver);
             
        }

        protected override void OnResume()
        {
            base.OnResume();
            gyroscopeObserver.Register(this);
        }


        protected override void OnPause()
        {
            base.OnPause();
            gyroscopeObserver.UnRegister();
        }
    }
}

