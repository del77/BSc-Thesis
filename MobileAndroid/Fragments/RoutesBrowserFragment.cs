﻿using Android.Gms.Maps;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Support.V4.View;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Core.Model;
using MobileAndroid.Adapters;

namespace MobileAndroid.Fragments
{
    public class RoutesBrowserFragment : Fragment
    {
        private View _routeBrowserView;
        private RecyclerView _routesRecyclerView;
        private Button _filterButton;
        private Spinner _terrainLevelSelect;
        private ActivityFragment _trainingFragment;
        private ViewPager _viewPager;
        private RecyclerView.LayoutManager _routeLayoutManager;
        private RoutesAdapter _routesAdapter;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _routeBrowserView = inflater.Inflate(Resource.Layout.routes_browser_fragment, container, false);

            FindViews();
            BindData();
            LinkEventHandlers();

            

            _routeLayoutManager = new LinearLayoutManager(inflater.Context);
            var orientation = ((LinearLayoutManager) _routeLayoutManager).Orientation;
            DividerItemDecoration dividerItemDecoration = new DividerItemDecoration(_routesRecyclerView.Context, orientation);
            dividerItemDecoration.SetDrawable(ContextCompat.GetDrawable(Context, Resource.Drawable.RecyclerViewDivider));

            _routesRecyclerView.AddItemDecoration(dividerItemDecoration);
            _routesRecyclerView.SetLayoutManager(_routeLayoutManager);
            _routesAdapter = new RoutesAdapter(inflater.Context) {};
            _routesAdapter.RouteClick += _routesAdapter_RouteClick;
            _routesRecyclerView.SetAdapter(_routesAdapter);


            return _routeBrowserView;
        }



        private void FindViews()
        {
            _filterButton = _routeBrowserView.FindViewById<Button>(Resource.Id.refreshRoutesButton);
            _terrainLevelSelect = _routeBrowserView.FindViewById<Spinner>(Resource.Id.terrainLevelSelect);
            _trainingFragment = (ActivityFragment)FragmentManager.FindFragmentByTag(MainPagerAdapter.GetFragmentTag(0));
            _viewPager = Activity.FindViewById<ViewPager>(Resource.Id.mainPager);
            _routesRecyclerView = _routeBrowserView.FindViewById<RecyclerView>(Resource.Id.routesRecycler);
        }

        private void BindData()
        {
            var terrainSelectAdapter = ArrayAdapter.CreateFromResource(Activity.ApplicationContext, Resource.Array.terrain_options, Android.Resource.Layout.SimpleSpinnerItem);
            terrainSelectAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            _terrainLevelSelect.Adapter = terrainSelectAdapter;
        }

        private void LinkEventHandlers()
        {
            _filterButton.Click += Button_Click;
        }



        private void _routesAdapter_RouteClick(object sender, Route e)
        {
            _trainingFragment.SetRoute(e);

            _viewPager.SetCurrentItem(0, true);
        }

        private void Button_Click(object sender, System.EventArgs e)
        {
            _routesAdapter.UpdateData();
        }
    }
}