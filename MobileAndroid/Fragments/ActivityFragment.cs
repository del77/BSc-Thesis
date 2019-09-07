using Android;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Maps;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using Core.Model;
using MobileAndroid.Adapters;

namespace MobileAndroid.Fragments
{
    public class ActivityFragment : Fragment, IOnMapReadyCallback
    {
        private TextView _routeName;
        private Button _removeCurrentRouteButton;

        private GoogleMap _googleMap;

        private View _view;
        public static Route CurrentRoute { get; set; }

        public void OnMapReady(GoogleMap googleMap)
        {
            if (ContextCompat.CheckSelfPermission(Activity, Manifest.Permission.AccessFineLocation)
                == Permission.Granted)
            {
                _googleMap = googleMap;

                _googleMap.UiSettings.MyLocationButtonEnabled = true;
                _googleMap.MyLocationEnabled = true;
            }
            else
            {
                var permissions = new[] { Manifest.Permission.AccessFineLocation };
                ActivityCompat.RequestPermissions(Activity, permissions, 1);
                OnMapReady(googleMap);
            }


        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            SetNewRoute();
            _view = inflater.Inflate(Resource.Layout.activity_fragment, container, false);
            FindViews();
            BindData();
            LinkEventHandlers();

            var mapFragment = (SupportMapFragment)ChildFragmentManager.FindFragmentById(Resource.Id.googleMap);
            mapFragment.GetMapAsync(this);

            return _view;
        }

        private void FindViews()
        {
            _routeName = _view.FindViewById<TextView>(Resource.Id.routeName);
            _removeCurrentRouteButton = _view.FindViewById<Button>(Resource.Id.removeCurrentRouteButton);
        }

        private void BindData()
        {
            _routeName.Text = CurrentRoute.Name;
        }

        private void LinkEventHandlers()
        {
            _removeCurrentRouteButton.Click += RemoveCurrentRouteButtonClick;
        }

        private void RemoveCurrentRouteButtonClick(object sender, System.EventArgs e)
        {
            _removeCurrentRouteButton.Visibility = ViewStates.Invisible;
            SetNewRoute();
            BindData();
        }

        private void SetNewRoute()
        {
            CurrentRoute = Route.GetNewRoute();
        }

        public void SetRoute(Route route)
        {
            CurrentRoute = route;
            _removeCurrentRouteButton.Visibility = ViewStates.Visible;
            BindData();
        }


    }


}