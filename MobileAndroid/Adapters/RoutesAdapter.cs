using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Core.Model;
using Core.Repositories;
using MobileAndroid.ViewHolders;

namespace MobileAndroid.Adapters
{
    public class RoutesAdapter : RecyclerView.Adapter
    {
        private readonly Context _context;
        public event EventHandler<Route> RouteClick;
        private List<Route> _routes;
        private readonly RoutesRepository _routesRepository;
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

            _routesRepository = new RoutesRepository();
            UpdateData();
        }

        public void UpdateData()
        {
            _routes = _routesRepository.GetAll().ToList();
            NotifyDataSetChanged();
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (holder is RouteViewHolder routeViewHolder)
            {
                var routeProperties = _routes[position].Properties;
                routeViewHolder.RouteName.Text = routeProperties.Name;
                routeViewHolder.RouteDistance.Text = _context.GetString(Resource.String.list_route_distance) +
                                                     routeProperties.Distance.ToString() + "km";
                routeViewHolder.RouteTerrainLevel.Text = _context.GetString(Resource.String.list_route_terrain) +
                                                         routeProperties.HeightAboveSeaLevel;
                routeViewHolder.RouteSurface.Text = _context.GetString(Resource.String.list_route_surface) +
                                                    routeProperties.PavedPercentage + "%";
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView =
                LayoutInflater.From(parent.Context).Inflate(Resource.Layout.route_viewholder, parent, false);

            var routeViewHolder = new RouteViewHolder(itemView, OnClick);
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