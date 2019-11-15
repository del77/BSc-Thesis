using System.Collections.Generic;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Preferences;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Support.V4.View;
using Android.Telephony;
using Android.Views;
using Android.Widget;
using Core.Model;
using Core.Repositories;
using MobileAndroid.Adapters;
using AlertDialog = Android.App.AlertDialog;

namespace MobileAndroid
{
    [Activity(Label = "@string/app_name", Theme = "@style/Theme.AppCompat.Light.NoActionBar", MainLauncher = false)]
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            var pager = FindViewById<ViewPager>(Resource.Id.mainPager);
            pager.Adapter = new MainPagerAdapter(this, SupportFragmentManager);
        }
    }
}