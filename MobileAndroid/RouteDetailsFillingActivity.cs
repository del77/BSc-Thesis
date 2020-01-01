using System;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Core.Model;
using Core.OpenStreetMap;
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

            
            FindViews();
            BindData();
            LinkEventHandlers();
        }

        private async Task ResolveSurface()
        {
            var osmService = new OsmService();
            int pavedPercentage;
            try
            {
                pavedPercentage = await osmService.ResolveRouteSurfaceTypeAsync(_route);
            }
            catch (Exception)
            {
                pavedPercentage = DefaultSurfacePavement;
            }

            _route.Properties.PavedPercentage = pavedPercentage;
            RunOnUiThread(() => _routeSurfaceSlider.SetSelectedMaxValue(pavedPercentage));
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
            ShowProgressBar();

            ResolveTerrainLevel();
            await ResolveSurface();

            HideProgressBar();
            Toast.MakeText(Application.Context, Resources.GetText(Resource.String.parameters_resolved), ToastLength.Long).Show();
        }

        private void ResolveTerrainLevel()
        {
            var resolvedTerrainLevel = _route.ResolveTerrainLevel();
            _terrainLevelSelect.SetSelection((int)resolvedTerrainLevel);
        }

        private void ShowProgressBar()
        {
            _resolveParametersProgressBar.Visibility = ViewStates.Visible;

        }

        private void HideProgressBar()
        {
            _resolveParametersProgressBar.Visibility = ViewStates.Invisible;
        }

        private async void _addRouteButton_Click(object sender, EventArgs e)
        {
            ShowProgressBar();
            _route.Properties.Name = _routeNameTextBox.Text;
            _route.Properties.TerrainLevel = (TerrainLevel)_terrainLevelSelect.SelectedItemPosition + 1;
            _route.Properties.PavedPercentage = (int)_routeSurfaceSlider.GetSelectedMaxValue();
            var isSuccessful = await _routesService.CreateRoute(_route);

            HideProgressBar();
            if(isSuccessful)
                Toast.MakeText(Application.Context, Resources.GetText(Resource.String.route_created), ToastLength.Long).Show();
            else
                Toast.MakeText(Application.Context, Resources.GetText(Resource.String.result_not_saved), ToastLength.Long).Show();
            Finish();
        }
        private void _cancelRouteButton_Click(object sender, EventArgs e)
        {
            Finish();
        }
    }
}