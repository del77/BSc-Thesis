using System;
using System.Runtime.Remoting.Contexts;
using System.Threading;
using System.Threading.Tasks;
using Android.Gms.Maps;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Support.V4.View;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Core.Model;
using MobileAndroid.Adapters;
using Xamarin.Essentials;
using Xamarin.RangeSlider;
using Xamarin.RangeSlider.Common;

namespace MobileAndroid.Fragments
{
    public class RoutesBrowserFragment : Fragment
    {
        private View _routeBrowserView;
        private RecyclerView _routesRecyclerView;
        private Android.Content.Context _context;

        private Button _filterButton;
        private Spinner _terrainLevelSelect;
        private RangeSliderControl _routeLengthSlider;
        private RangeSliderControl _routeSurfaceSlider;
        private RangeSliderControl _searchRadiusSlider;

        private ActivityFragment _trainingFragment;
        private ViewPager _viewPager;
        private RecyclerView.LayoutManager _routeLayoutManager;
        private RoutesAdapter _routesAdapter;

        private const float LastFiniteRouteLength = 21;
        private const float DefaultRouteSearchRange = 5;


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _routeBrowserView = inflater.Inflate(Resource.Layout.routes_browser_fragment, container, false);
            _context = inflater.Context;
            FindViews();
            BindData();
            LinkEventHandlers();

            _routeLayoutManager = new LinearLayoutManager(inflater.Context);
            var orientation = ((LinearLayoutManager)_routeLayoutManager).Orientation;
            DividerItemDecoration dividerItemDecoration = new DividerItemDecoration(_routesRecyclerView.Context, orientation);
            dividerItemDecoration.SetDrawable(ContextCompat.GetDrawable(Context, Resource.Drawable.RecyclerViewDivider));

            _routesRecyclerView.AddItemDecoration(dividerItemDecoration);
            _routesRecyclerView.SetLayoutManager(_routeLayoutManager);
            _routesAdapter = new RoutesAdapter(inflater.Context) { };
            _routesAdapter.RouteClick += _routesAdapter_RouteClick;
            _routesRecyclerView.SetAdapter(_routesAdapter);

            return _routeBrowserView;
        }



        private void FindViews()
        {
            _filterButton = _routeBrowserView.FindViewById<Button>(Resource.Id.refreshRoutesButton);
            _terrainLevelSelect = _routeBrowserView.FindViewById<Spinner>(Resource.Id.terrainLevelSelect);
            _routeLengthSlider = _routeBrowserView.FindViewById<RangeSliderControl>(Resource.Id.route_length_slider);
            _routeSurfaceSlider = _routeBrowserView.FindViewById<RangeSliderControl>(Resource.Id.surface_type_slider);
            _searchRadiusSlider = _routeBrowserView.FindViewById<RangeSliderControl>(Resource.Id.search_range_select);
            _trainingFragment = (ActivityFragment)FragmentManager.FindFragmentByTag(MainPagerAdapter.GetFragmentTag(0));
            _viewPager = Activity.FindViewById<ViewPager>(Resource.Id.mainPager);
            _routesRecyclerView = _routeBrowserView.FindViewById<RecyclerView>(Resource.Id.routesRecycler);
        }

        private void BindData()
        {
            var terrainSelectAdapter = ArrayAdapter.CreateFromResource(Activity.ApplicationContext,
                Resource.Array.terrain_options, Android.Resource.Layout.SimpleSpinnerItem);
            terrainSelectAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            _terrainLevelSelect.Adapter = terrainSelectAdapter;

            _routeLengthSlider.SetSelectedMaxValue(LastFiniteRouteLength);
            _routeLengthSlider.NotifyWhileDragging = true;
            _routeLengthSlider_UpperValueChanged(null, null);
            
            _searchRadiusSlider.SetSelectedMaxValue(DefaultRouteSearchRange);
            _routeSurfaceSlider.SetSelectedMaxValue(100);
        }

        private void LinkEventHandlers()
        {
            _filterButton.Click += Button_Click;
            _routeLengthSlider.UpperValueChanged += _routeLengthSlider_UpperValueChanged;
            _routeLengthSlider.DragStarted += _routeLengthSlider_DragStarted;
        }

        private void _routeLengthSlider_DragStarted(object sender, EventArgs e)
        {
            //_routeLengthSlider.MaxThumbTextHidden = false;
        }

        private void _routeLengthSlider_UpperValueChanged(object sender, EventArgs e)
        {
            if (_routeLengthSlider.GetSelectedMaxValue() == LastFiniteRouteLength)
            {
                _routeLengthSlider.MaxThumbTextHidden = true;
                _routeLengthSlider.FormatLabel = (thumb, f) => _context.Resources.GetString(Resource.String.from) + " " + f;
            }
            else
            {
                _routeLengthSlider.MaxThumbTextHidden = false;
                _routeLengthSlider.FormatLabel = (thumb, f) => f.ToString();
            }
        }

        private void _routesAdapter_RouteClick(object sender, Route e)
        {
            _trainingFragment.SetRoute(e);

            _viewPager.SetCurrentItem(0, true);
        }

        private async void Button_Click(object sender, System.EventArgs e)
        {
            ((MainActivity)Activity).ShowProgressBar();

            var location = await Geolocation.GetLastKnownLocationAsync();

            var query = new RoutesFilterQuery
            {
                RouteLengthFrom = (int)_routeLengthSlider.GetSelectedMinValue(),
                RouteLengthTo = (int)_routeLengthSlider.GetSelectedMaxValue(),
                SurfacePavedPercentageFrom = (int)_routeSurfaceSlider.GetSelectedMinValue(),
                SurfacePavedPercentageTo = (int)_routeSurfaceSlider.GetSelectedMaxValue(),
                SurfaceLevel = (HeightAboveSeaLevel)_terrainLevelSelect.SelectedItemPosition,
                SearchRadiusInMeters = (int)_searchRadiusSlider.GetSelectedMaxValue() * 1000,
                CurrentLatitude = location.Latitude,
                CurrentLongitude = location.Longitude
            };

            _routesAdapter.UpdateData(query);

            ((MainActivity)Activity).HideProgressBar();
        }
    }
}