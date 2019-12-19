﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Location;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Core.Model;
using MobileAndroid.Adapters;
using MobileAndroid.Extensions;
using Xamarin.Essentials;
using Bitmap = Android.Graphics.Bitmap;
using Fragment = Android.Support.V4.App.Fragment;

namespace MobileAndroid.Fragments
{
    public class ActivityFragment : Fragment, IOnMapReadyCallback
    {
        private TextView _routeName;
        private TextView _timer;
        private Button _removeCurrentRouteButton;
        private Button _timerButton;
        private RecyclerView _rankingRecyclerView;

        private GoogleMap _googleMap;
        private PolylineOptions _polylineOptions;
        private Polyline _routePolyline;
        private readonly List<Marker> _checkpointMarkers = new List<Marker>();

        private View _view;
        private RankingAdapter _rankingAdapter;
        private RecyclerView.LayoutManager _rankingLayoutManager;
        private FusedLocationProviderClient _locationProvider;
        private Context _context;
        public static Route CurrentRoute { get; set; }
        public TrainingBase Training { get; set; }


        public void OnMapReady(GoogleMap googleMap)
        {
            var permissions = new[] { Manifest.Permission.AccessFineLocation };
            ActivityCompat.RequestPermissions(Activity, permissions, 1);
            if (ContextCompat.CheckSelfPermission(Activity, Manifest.Permission.AccessFineLocation)
                == Permission.Granted)
            {
                _googleMap = googleMap;

                _googleMap.UiSettings.MyLocationButtonEnabled = true;
                _googleMap.MyLocationEnabled = true;

                _polylineOptions = new PolylineOptions();
                _routePolyline = _googleMap.AddPolyline(_polylineOptions);
                MoveCameraToCurrentUserPosition();
            }
            else
            {
                System.Environment.Exit(0);
            }
        }

        public async Task<Tuple<double, double, double?>> GetCurrentLocation()
        {
            var location = await Geolocation.GetLastKnownLocationAsync();
            //var location = await _locationProvider.GetLastLocationAsync();


            //var location = await Geolocation.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.Best));
            Activity.RunOnUiThread(() =>
            {
                AddToPolyline(location.Latitude, location.Longitude);
            });

            //var altitude = location.HasAltitude ? location.Altitude : (double?) null;
            return Tuple.Create(location.Latitude, location.Longitude, location.Altitude);
        }


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            _context = inflater.Context;
            _locationProvider = LocationServices.GetFusedLocationProviderClient(inflater.Context);
            _view = inflater.Inflate(Resource.Layout.activity_fragment, container, false);
            FindViews();
            LinkEventHandlers();

            var mapFragment = (SupportMapFragment)ChildFragmentManager.FindFragmentById(Resource.Id.googleMap);
            mapFragment.GetMapAsync(this);

            //_rankingAdapter = new RankingAdapter(inflater.Context, ChildFragmentManager) { };
            _rankingLayoutManager = new LinearLayoutManager(inflater.Context);

            _rankingAdapter = new RankingAdapter();
            _rankingRecyclerView.SetLayoutManager(_rankingLayoutManager);
            _rankingRecyclerView.SetAdapter(_rankingAdapter);
            SetNewRoute();
            BindData();

            return _view;
        }

        private void PlayCurrentPosition(int position)
        {
            TextToSpeech.SpeakAsync($"{Resources.GetString(Resource.String.your_position, position)}");
        }

        private void PlayPositionsEarned(int positions)
        {
            TextToSpeech.SpeakAsync($"{Resources.GetString(Resource.String.positions_earned, positions)}");
        }

        private void PlayPositionsLost(int positions)
        {
            TextToSpeech.SpeakAsync($"{Resources.GetString(Resource.String.positions_lost, positions)}");
        }


        private void FindViews()
        {
            _routeName = _view.FindViewById<TextView>(Resource.Id.routeName);
            _timer = _view.FindViewById<TextView>(Resource.Id.timer);
            _removeCurrentRouteButton = _view.FindViewById<Button>(Resource.Id.removeCurrentRouteButton);
            _timerButton = _view.FindViewById<Button>(Resource.Id.timerButton);
            _rankingRecyclerView = _view.FindViewById<RecyclerView>(Resource.Id.rankingRecycler);
        }

        private void BindData()
        {
            _routeName.Text = CurrentRoute.Properties.Name;
            if (!CurrentRoute.Checkpoints.Any())
                Training = new InitialTraining(CurrentRoute, UpdateTimer, GetCurrentLocation);
            else
            {
                Training = new RaceTraining(CurrentRoute, UpdateTimer, NextCheckpointReached, GetCurrentLocation, 
                    StopTraining, PlayCurrentPosition, PlayPositionsLost, PlayPositionsEarned,
                    ShowProgressBar, HideProgressBar, ShowUpdateResult);
            }
        }

        private void LinkEventHandlers()
        {
            _removeCurrentRouteButton.Click += RemoveCurrentRouteButtonClick;
            _timerButton.Click += _timerButton_Click;
        }

        private async void _timerButton_Click(object sender, System.EventArgs e)
        {
            //var xD = new OsmService();
            //await xD.ResolveRouteSurfaceTypeAsync(null);
            //await TextToSpeech.SpeakAsync("1");

            //await TextToSpeech.SpeakAsync("1.");

            if (!Training.IsStarted)
            {
                Training.Start();

                _timerButton.Text = "Stop";
            }
            else
            {
                Training.Stop();
                _timerButton.Text = "Start";
                _timer.Text = "0:0";
                RemovePolyline();
                RemoveAllCheckPoints();

                if (Training is InitialTraining)
                {
                    var routeDetailsIntent = new Intent(Context, typeof(RouteDetailsFillingActivity));
                    routeDetailsIntent.PutExtra("route", CurrentRoute);
                    StartActivity(routeDetailsIntent);
                    SetNewRoute();
                }
                else
                {
                    SetRoute(CurrentRoute);
                }
            }
        }

        private void StopTraining()
        {
            Activity.RunOnUiThread(() =>
            {
                _timerButton.Text = "Start";
                _timer.Text = "0:0";


                SetRoute(CurrentRoute);
            });
        }

        private void AddToPolyline(double latitude, double longitude)
        {
            var newLocation = new LatLng(latitude, longitude);
            _polylineOptions.Points.Add(newLocation);
            _routePolyline.Points = _polylineOptions.Points;
        }

        private void RemovePolyline()
        {
            _polylineOptions.Points.Clear();
            _routePolyline.Points = _polylineOptions.Points;
        }

        private void ShowProgressBar()
        {
            ((MainActivity)Activity).ShowProgressBar();
        }

        public void HideProgressBar()
        {
            ((MainActivity)Activity).HideProgressBar();
        }

        public void ShowUpdateResult(bool isSuccessful)
        {
            Activity.RunOnUiThread(() =>
            {
                if (isSuccessful)
                    Toast.MakeText(Application.Context, Resources.GetText(Resource.String.try_created), ToastLength.Long).Show();
                else
                    Toast.MakeText(Application.Context, Resources.GetText(Resource.String.result_not_saved), ToastLength.Long).Show();
            });
        }

        private void UpdateTimer()
        {
            Activity.RunOnUiThread(() =>
            {
                var time = TimeSpan.FromSeconds(Training.Seconds);
                //var time = Training != null ? TimeSpan.FromSeconds(Training.Seconds) : TimeSpan.Zero;
                _timer.Text = $"{time.Minutes:D2}:{time.Seconds:D2}";
            });
        }

        private void NextCheckpointReached()
        {
            Activity.RunOnUiThread(() =>
            {
                _rankingAdapter.ShowDataForNextCheckpoint();
                _checkpointMarkers[0].Remove();
                _checkpointMarkers.RemoveAt(0);
                MoveCameraToNextCheckpoint();
            });
        }

        private void RemoveCurrentRouteButtonClick(object sender, System.EventArgs e)
        {
            _removeCurrentRouteButton.Visibility = ViewStates.Invisible;
            RemoveAllCheckPoints();
            SetNewRoute();
            BindData();
        }

        private void RemoveAllCheckPoints()
        {
            _checkpointMarkers.ForEach(cp => cp.Remove());
        }

        private void SetNewRoute()
        {
            CurrentRoute = Route.GetNewRoute();
            //_rankingRecyclerView.RemoveAllViewsInLayout();
            _rankingAdapter.UpdateRoute(CurrentRoute);
        }

        public void SetRoute(Route route)
        {
            CurrentRoute = route;
            _rankingAdapter.UpdateRoute(CurrentRoute);
            _removeCurrentRouteButton.Visibility = ViewStates.Visible;
            BindData();

            Bitmap.Config conf = Bitmap.Config.Argb8888;
            var bmp = BitmapFactory.DecodeResource(Resources, Resource.Drawable.checkpoint);
            Canvas canvas1 = new Canvas();
            canvas1.DrawBitmap(bmp, 0, 0, null);

            RemovePolyline();
            RemoveAllCheckPoints();
            foreach (var routeCheckpoint in route.Checkpoints)
            {
                var marker = _googleMap.AddMarker(new MarkerOptions()
                    .SetPosition(new LatLng(routeCheckpoint.Latitude, routeCheckpoint.Longitude))
                    .SetIcon(BitmapDescriptorFactory.FromBitmap(bmp))
                    .Anchor(0.5f, 0.5f));
                _checkpointMarkers.Add(marker);
            }
            MoveCameraToNextCheckpoint();
        }

        private void MoveCameraToNextCheckpoint()
        {
            var location = _checkpointMarkers.First().Position;

            MoveCamera(location);
        }

        private async void MoveCameraToCurrentUserPosition()
        {
            var location = await Geolocation.GetLastKnownLocationAsync();

            if (location != null)
            {
                var latlng = new LatLng(location.Latitude, location.Longitude);
                MoveCamera(latlng);
            }
        }

        private void MoveCamera(LatLng position)
        {
            CameraUpdate userZoom = CameraUpdateFactory.NewLatLngZoom(position, 19);
            _googleMap.AnimateCamera(userZoom);
        }
    }

}