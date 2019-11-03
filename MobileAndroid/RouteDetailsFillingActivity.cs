using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Core.Model;
using Core.OpenStreetMap;
using Core.Repositories;
using Core.Services;
using MobileAndroid.Extensions;

namespace MobileAndroid
{
    [Activity(Label = "RouteDetailsFillingActivity")]
    public class RouteDetailsFillingActivity : Activity
    {
        private EditText _routeNameTextBox;
        private Button _addRouteButton;
        private Button _cancelRouteButton;
        private RoutesService _routesService;
        private Route _route;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.route_details_filling_view);

            _routesService = new RoutesService();
            _route = Intent.GetExtra<Route>("route");

            FindViews();
            LinkEventHandlers();

            RetrieveRoute();
        }

        private void RetrieveRoute()
        {
            CalculateDistance();
            ResolveTerrainLevel();
            ResolveSurface();
        }

        private void ResolveTerrainLevel()
        {
            var startAltitude = _route.Checkpoints.First().Altitude;
            var finishAltitude = _route.Checkpoints.Last().Altitude;

            if (startAltitude == null || finishAltitude == null)
            {
                _route.Properties.HeightAboveSeaLevel = HeightAboveSeaLevel.Close;
                return;
            }

            if (Math.Abs(startAltitude.Value - finishAltitude.Value) > 50)
                _route.Properties.HeightAboveSeaLevel = startAltitude > finishAltitude
                    ? HeightAboveSeaLevel.Decreasing
                    : HeightAboveSeaLevel.Increasing;

        }

        private async void ResolveSurface()
        {
            var osmService = new OsmService();
            _route.Properties.PavedPercentage = await osmService.ResolveRouteSurfaceTypeAsync(_route);
        }

        private void CalculateDistance()
        {
            var totalDistance = 0d;
            for (int i = 0; i < _route.Checkpoints.Count - 1; i++)
            {
                totalDistance += Point.Distance(_route.Checkpoints[i], _route.Checkpoints[i + 1], 'K');
            }

            _route.Properties.Distance = totalDistance;
        }

        private void FindViews()
        {
            _routeNameTextBox = FindViewById<EditText>(Resource.Id.routeNameText);
            _addRouteButton = FindViewById<Button>(Resource.Id.addRouteButton);
            _cancelRouteButton = FindViewById<Button>(Resource.Id.cancelRouteButton);
        }

        private void LinkEventHandlers()
        {
            _addRouteButton.Click += _addRouteButton_Click;
            _cancelRouteButton.Click += _cancelRouteButton_Click;
        }

        private async void _addRouteButton_Click(object sender, EventArgs e)
        {
            _route.Properties.Name = _routeNameTextBox.Text;
            await _routesService.CreateRoute(_route);
            Toast.MakeText(Application.Context, Resources.GetText(Resource.String.route_created), ToastLength.Long).Show();

            Finish();
        }
        private void _cancelRouteButton_Click(object sender, EventArgs e)
        {
            Finish();
        }
    }
}