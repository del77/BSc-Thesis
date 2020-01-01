using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Core.Model;
using Core.Services;
using MobileAndroid.ViewHolders;

namespace MobileAndroid.Adapters
{
    public class RoutesAdapter : RecyclerView.Adapter
    {
        private readonly Context _context;
        public event EventHandler<Route> RouteClick;
        private List<Route> _routes = new List<Route>();
        private readonly RoutesService _routesService;
        public RoutesAdapter(Context context)
        {
            _context = context;

            _routesService = new RoutesService();
        }

        public async Task UpdateData(RoutesFilterQuery query)
        {
            _routes = (await _routesService.GetRoutes(query)).ToList();
            NotifyDataSetChanged();
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (holder is RouteViewHolder routeViewHolder)
            {
                var routeProperties = _routes[position].Properties;
                routeViewHolder.RouteName.Text = routeProperties.Name;
                routeViewHolder.RouteDistance.Text = routeProperties.Distance + "km";
                routeViewHolder.RouteTerrainLevel.Text =
                    _context.Resources.GetStringArray(Resource.Array.terrain_options)[(int)routeProperties.TerrainLevel];
                routeViewHolder.RouteSurface.Text = routeProperties.PavedPercentage + "%";
                routeViewHolder.Route = _routes[position];
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView =
                LayoutInflater.From(parent.Context).Inflate(Resource.Layout.route_viewholder, parent, false);

            var routeViewHolder = new RouteViewHolder(itemView, OnClick, _context);
            return routeViewHolder;
        }

        private void OnClick(int position)
        {
            var route = _routes[position];

            RouteClick?.Invoke(this, route);
        }

        public override int ItemCount => _routes.Count;
    }
}