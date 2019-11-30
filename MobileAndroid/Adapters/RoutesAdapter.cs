using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Content.Res;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;
using Core.Model;
using Core.Repositories;
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
            //_routes = new List<Route>
            //{
            //    new Route { Name = "pierwsza"},
            //    new Route { Name = "druga"},
            //    new Route { Name = "trzecia"},
            //    new Route { Name = "czwarta"},
            //    new Route { Name = "piata"},
            //    new Route { Name = "szosta"},
            //    new Route { Name = "siodma"},
            //    new Route { Name = "osma"},
            //    new Route { Name = "dziewiata"},
            //};

            _routesService = new RoutesService();
            //UpdateData();
        }

        public async void UpdateData(RoutesFilterQuery query)
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
                    _context.Resources.GetStringArray(Resource.Array.terrain_options)[(int)routeProperties.HeightAboveSeaLevel];
                //routeViewHolder.RouteTerrainLevel.Text = "Wyrównany";
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