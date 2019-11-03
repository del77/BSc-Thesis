using System;
using System.Linq;
using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Core.Model;

namespace MobileAndroid.ViewHolders
{
    public class RouteViewHolder : RecyclerView.ViewHolder, IOnMapReadyCallback
    {
        private readonly Context _context;
        public TextView RouteName { get; set; }
        public TextView RouteDistance { get; set; }
        public TextView RouteSurface { get; set; }
        public TextView RouteTerrainLevel { get; set; }
        public GoogleMap GoogleMap { get; set; }
        public MapView MapView { get; set; }
        public Route Route { get; set; }

        public RouteViewHolder(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public RouteViewHolder(View itemView, Action<int> listener, Context context) : base(itemView)
        {
            _context = context;
            RouteName = itemView.FindViewById<TextView>(Resource.Id.list_route_name);
            RouteDistance = itemView.FindViewById<TextView>(Resource.Id.list_route_distance);
            RouteSurface = itemView.FindViewById<TextView>(Resource.Id.list_route_surface);
            RouteTerrainLevel = itemView.FindViewById<TextView>(Resource.Id.list_route_terrain);

            MapView = (MapView)itemView.FindViewById(Resource.Id.googleMap2);
            if (MapView != null)
            {
                MapView.OnCreate(null);
                MapView.OnResume();
                MapView.GetMapAsync(this);
            }


            itemView.Click += (sender, e) => listener(base.LayoutPosition);
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            MapsInitializer.Initialize(_context);
            GoogleMap = googleMap;
            AddCheckpointsToMap();
            GoogleMap.MoveCamera(CameraUpdateFactory.NewCameraPosition(CameraPosition.FromLatLngZoom(new LatLng(Route.Checkpoints.First().Latitude, Route.Checkpoints.First().Longitude), 1)));
        }

        private void AddCheckpointsToMap()
        {
            Bitmap.Config conf = Bitmap.Config.Argb8888;
            var bmp = BitmapFactory.DecodeResource(_context.Resources, Resource.Drawable.checkpoint);
            Canvas canvas1 = new Canvas();
            canvas1.DrawBitmap(bmp, 0, 0, null);


            foreach (var routeCheckpoint in Route.Checkpoints)
            {
                var marker = GoogleMap.AddMarker(new MarkerOptions()
                    .SetPosition(new LatLng(routeCheckpoint.Latitude, routeCheckpoint.Longitude))
                    .SetIcon(BitmapDescriptorFactory.FromBitmap(bmp))
                    .Anchor(0.5f, 0.5f));
            }
        }
    }
}