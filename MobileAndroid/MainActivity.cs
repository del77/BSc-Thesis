﻿using System.Threading.Tasks;
using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using Core.Services;
using MobileAndroid.Adapters;

namespace MobileAndroid
{
    [Activity(Label = "@string/app_name", Theme = "@style/Theme.AppCompat.Light.NoActionBar", MainLauncher = false)]
    public class MainActivity : AppCompatActivity
    {
        private ProgressBar _progressBar;
        private readonly RoutesService _routesService;

        public MainActivity()
        {
            _routesService = new RoutesService();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            FindViews();

            Task.Run(async () => await _routesService.ProcessOverdueData());
        }

        private void FindViews()
        {
            _progressBar = FindViewById<ProgressBar>(Resource.Id.main_progressbar);
            var pager = FindViewById<ViewPager>(Resource.Id.mainPager);
            pager.Adapter = new MainPagerAdapter(this, SupportFragmentManager);
        }

        public void ShowProgressBar()
        {
            RunOnUiThread(() =>
            {
                _progressBar.Visibility = ViewStates.Visible;
            });
        }

        public void HideProgressBar()
        {
            RunOnUiThread(() =>
            {
                _progressBar.Visibility = ViewStates.Invisible;
            });
        }
    }
}