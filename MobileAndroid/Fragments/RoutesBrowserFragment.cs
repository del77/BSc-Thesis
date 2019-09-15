using Android.OS;
using Android.Support.V4.App;
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
        private View _singleRouteView;
        private RecyclerView _routesRecyclerView;
        private Button _filterButton;
        private ActivityFragment _trainingFragment;
        private ViewPager _viewPager;
        private RecyclerView.LayoutManager _routeLayoutManager;
        private RoutesAdapter _routesAdapter;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _singleRouteView = inflater.Inflate(Resource.Layout.routes_browser_fragment, container, false);

            FindViews();
            BindData();
            LinkEventHandlers();

            _routeLayoutManager = new LinearLayoutManager(inflater.Context);
            _routesRecyclerView.SetLayoutManager(_routeLayoutManager);
            _routesAdapter = new RoutesAdapter() {};
            _routesAdapter.RouteClick += _routesAdapter_RouteClick;
            _routesRecyclerView.SetAdapter(_routesAdapter);

            return _singleRouteView;
        }



        private void FindViews()
        {
            _filterButton = _singleRouteView.FindViewById<Button>(Resource.Id.button1);
            _trainingFragment = (ActivityFragment)FragmentManager.FindFragmentByTag(MainPagerAdapter.GetFragmentTag(0));
            _viewPager = Activity.FindViewById<ViewPager>(Resource.Id.mainPager);
            _routesRecyclerView = _singleRouteView.FindViewById<RecyclerView>(Resource.Id.routesRecycler);
            
        }

        private void BindData()
        {
            
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