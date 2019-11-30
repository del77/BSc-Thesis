using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using Xamarin.RangeSlider;

namespace MobileAndroid
{
    [Activity(Label = "RouteDetailsFillingActivity")]
    public class RouteDetailsFillingActivity : Activity
    {
        private Spinner _terrainLevelSelect;
        private RangeSliderControl _routeSurfaceSlider;
        private EditText _routeNameTextBox;
        private TextView _routeDistanceTextView;
        private Button _resolveParametersButton;
        private Button _addRouteButton;
        private Button _cancelRouteButton;
        private ProgressBar _resolveParametersProgressBar;
        private RoutesService _routesService;
        private Route _route;

        private const int DefaultSurfacePavement = 50;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.route_details_filling_view);

            _routesService = new RoutesService();
            _route = Intent.GetExtra<Route>("route");

            CalculateDistance();
            FindViews();
            BindData();
            LinkEventHandlers();
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

            if (Math.Abs(startAltitude.Value - finishAltitude.Value) >= 10)
            {
                _route.Properties.HeightAboveSeaLevel = startAltitude > finishAltitude
                    ? HeightAboveSeaLevel.Decreasing
                    : HeightAboveSeaLevel.Increasing;
                _terrainLevelSelect.SetSelection((int)_route.Properties.HeightAboveSeaLevel);

            }
        }

        private async Task ResolveSurface()
        {
            var osmService = new OsmService();
            var pavedPercentage = await osmService.ResolveRouteSurfaceTypeAsync(_route);

            _route.Properties.PavedPercentage = pavedPercentage;
            RunOnUiThread(() => _routeSurfaceSlider.SetSelectedMaxValue(pavedPercentage));
        }

        private void CalculateDistance()
        {
            var totalDistance = 0d;
            for (int i = 0; i < _route.Checkpoints.Count - 1; i++)
            {
                totalDistance += Point.Distance(_route.Checkpoints[i], _route.Checkpoints[i + 1], 'K');
            }

            _route.Properties.Distance = Math.Round(totalDistance, 2);
        }

        private void FindViews()
        {
            _routeNameTextBox = FindViewById<EditText>(Resource.Id.routeNameText);
            _resolveParametersButton = FindViewById<Button>(Resource.Id.resolve_parameters_button);
            _addRouteButton = FindViewById<Button>(Resource.Id.addRouteButton);
            _cancelRouteButton = FindViewById<Button>(Resource.Id.cancelRouteButton);
            _routeSurfaceSlider = FindViewById<RangeSliderControl>(Resource.Id.surface_type_slider_filling_view);
            _terrainLevelSelect = FindViewById<Spinner>(Resource.Id.terrainLevelSelect_filling_view);
            _routeDistanceTextView = FindViewById<TextView>(Resource.Id.route_length_filling_view);
            _resolveParametersProgressBar = FindViewById<ProgressBar>(Resource.Id.resolve_parameters_progressbar);
        }

        private void BindData()
        {
            var terrainSelectAdapter = ArrayAdapter.CreateFromResource(this,
                Resource.Array.terrain_options_filling_view, Android.Resource.Layout.SimpleSpinnerItem);
            terrainSelectAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            _terrainLevelSelect.Adapter = terrainSelectAdapter;

            _routeSurfaceSlider.SetSelectedMaxValue(DefaultSurfacePavement);

            _routeDistanceTextView.Text = Resources.GetString(Resource.String.route_length) + ": " + _route.Properties.Distance;
        }

        private void LinkEventHandlers()
        {
            _resolveParametersButton.Click += ResolveParametersButtonOnClick;
            _addRouteButton.Click += _addRouteButton_Click;
            _cancelRouteButton.Click += _cancelRouteButton_Click;
        }

        private async void ResolveParametersButtonOnClick(object sender, EventArgs e)
        {
            _resolveParametersProgressBar.Visibility = ViewStates.Visible;

            ResolveTerrainLevel();
            await ResolveSurface();

            _resolveParametersProgressBar.Visibility = ViewStates.Invisible;
            Toast.MakeText(Application.Context, Resources.GetText(Resource.String.parameters_resolved), ToastLength.Long).Show();
        }

        private async void _addRouteButton_Click(object sender, EventArgs e)
        {
            _route.Properties.Name = _routeNameTextBox.Text;
            _route.Properties.HeightAboveSeaLevel = (HeightAboveSeaLevel)_terrainLevelSelect.SelectedItemPosition + 1;
            _route.Properties.PavedPercentage = (int)_routeSurfaceSlider.GetSelectedMaxValue();
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