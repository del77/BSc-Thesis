using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Android;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using Core.Model;
using MobileAndroid.Adapters;
using Xamarin.Essentials;

namespace MobileAndroid.Fragments
{
    public class ActivityFragment : Fragment, IOnMapReadyCallback
    {
        private TextView _routeName;
        private TextView _timer;
        private Button _removeCurrentRouteButton;
        private Button _timerButton;

        private GoogleMap _googleMap;
        private PolylineOptions _polylineOptions;
        private Polyline _routePolyline;
        private readonly List<Circle> _checkpointsCircles = new List<Circle>();

        private View _view;
        public static Route CurrentRoute { get; set; }
        public Training Training { get; set; }
        

        public void OnMapReady(GoogleMap googleMap)
        {
            if (ContextCompat.CheckSelfPermission(Activity, Manifest.Permission.AccessFineLocation)
                == Permission.Granted)
            {
                _googleMap = googleMap;

                _googleMap.UiSettings.MyLocationButtonEnabled = true;
                _googleMap.MyLocationEnabled = true;
                _googleMap.MyLocationChange += _googleMap_MyLocationChange;

                _polylineOptions = new PolylineOptions();
                _routePolyline = _googleMap.AddPolyline(_polylineOptions);
                _googleMap_MyLocationChange(null, null);
            }
            else
            {
                var permissions = new[] { Manifest.Permission.AccessFineLocation };
                ActivityCompat.RequestPermissions(Activity, permissions, 1);
                OnMapReady(googleMap);
            }


        }

        private async void _googleMap_MyLocationChange(object sender, GoogleMap.MyLocationChangeEventArgs e)
        {
            var location = await Geolocation.GetLastKnownLocationAsync();
            //if (location != null)
            //{
            //    var latlng = new LatLng(location.Latitude, location.Longitude);
            //    CameraUpdate userZoom = CameraUpdateFactory.NewLatLngZoom(latlng, 19);
            //    _googleMap.AnimateCamera(userZoom);
            //}
        }

        public async Task<Tuple<double, double>> GetCurrentLocation()
        {
            var location = await Geolocation.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.Best));
            return Tuple.Create(location.Latitude, location.Longitude);
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
            _timer = _view.FindViewById<TextView>(Resource.Id.timer);
            _removeCurrentRouteButton = _view.FindViewById<Button>(Resource.Id.removeCurrentRouteButton);
            _timerButton = _view.FindViewById<Button>(Resource.Id.timerButton);
        }

        private void BindData()
        {
            _routeName.Text = CurrentRoute.Name;
            Training = new Training(CurrentRoute);

        }

        private void LinkEventHandlers()
        {
            _removeCurrentRouteButton.Click += RemoveCurrentRouteButtonClick;
            _timerButton.Click += _timerButton_Click;
        }

        private async void _timerButton_Click(object sender, System.EventArgs e)
        {
            if (!Training.IsStarted)
            {
                var l = await Geolocation.GetLocationAsync();
                
                AddToPolyline(l.Latitude, l.Longitude);

                //Training.Points.Add(Tuple.Create(l.Longitude, l.Latitude));
                Training.Start(UpdateUi, GetCurrentLocation);
                _timerButton.Text = "Stop";
            }
            else
            {
                Training.Stop();
                _timerButton.Text = "Start";
            }
        }

        private void AddToPolyline(double latitude, double longitude)
        {
            var newLocation = new LatLng(latitude, longitude);
            _polylineOptions.Points.Add(newLocation);
            _routePolyline.Points = _polylineOptions.Points;
        }

        private void UpdateUi()
        {
            Activity.RunOnUiThread(() =>
            {
                _timer.Text = $"{Training.Seconds / 60}:{Training.Seconds % 60}";
                var lastPoint = Training.Points.Last();
                AddToPolyline(lastPoint.Latitude, lastPoint.Longitude);
            });
        }

        private void RemoveCurrentRouteButtonClick(object sender, System.EventArgs e)
        {
            _removeCurrentRouteButton.Visibility = ViewStates.Invisible;
            _checkpointsCircles.ForEach(cp => cp.Remove());
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

            foreach (var routeCheckpoint in route.Checkpoints)
            {
                var circle = _googleMap.AddCircle(new CircleOptions()
                    .InvokeCenter(new LatLng(routeCheckpoint.Latitude, routeCheckpoint.Longitude))
                    .InvokeRadius(3)
                    .InvokeStrokeWidth(1f)
                    .InvokeFillColor(0X66FF0000)
                );
                _checkpointsCircles.Add(circle);
            }
        }


    }


}