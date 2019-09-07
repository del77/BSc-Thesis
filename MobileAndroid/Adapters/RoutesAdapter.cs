using System.Collections.Generic;
using Android.Support.V7.Widget;
using Android.Views;
using Core.Model;
using MobileAndroid.ViewHolders;

namespace MobileAndroid.Adapters
{
    public class RoutesAdapter : RecyclerView.Adapter
    {
        private List<Route> _routes;

        public RoutesAdapter()
        {
            _routes = new List<Route>
            {
                new Route { Name = "pierwsza"},
                new Route { Name = "druga"},
                new Route { Name = "trzecia"},
                new Route { Name = "czwarta"},
                new Route { Name = "piata"},
                new Route { Name = "szosta"},
                new Route { Name = "siodma"},
                new Route { Name = "osma"},
                new Route { Name = "dziewiata"},
            };
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (holder is RouteViewHolder routeViewHolder)
            {
                routeViewHolder.RouteName.Text = _routes[position].Name;
                routeViewHolder.RouteDistance.Text = _routes[position].Distance;
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView =
                LayoutInflater.From(parent.Context).Inflate(Resource.Layout.route_viewholder, parent, false);

            var routeViewHolder = new RouteViewHolder(itemView);
            return routeViewHolder;
        }

        public override int ItemCount => _routes.Count;
    }
}