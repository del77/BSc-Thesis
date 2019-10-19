using System;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace MobileAndroid.ViewHolders
{
    public class RouteViewHolder : RecyclerView.ViewHolder
    {
        public TextView RouteName { get; set; }
        public TextView RouteDistance { get; set; }
        public TextView RouteSurface { get; set; }
        public TextView RouteTerrainLevel { get; set; }

        public RouteViewHolder(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public RouteViewHolder(View itemView, Action<int> listener) : base(itemView)
        {
            RouteName = itemView.FindViewById<TextView>(Resource.Id.list_route_name);
            RouteDistance = itemView.FindViewById<TextView>(Resource.Id.list_route_distance);
            RouteSurface = itemView.FindViewById<TextView>(Resource.Id.list_route_surface);
            RouteTerrainLevel = itemView.FindViewById<TextView>(Resource.Id.list_route_terrain);

            itemView.Click += (sender, e) => listener(base.LayoutPosition);
        }
    }
}