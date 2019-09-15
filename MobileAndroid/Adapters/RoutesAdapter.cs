﻿using System;
using System.Collections.Generic;
using System.Linq;
using Android.Support.V7.Widget;
using Android.Views;
using Core.Model;
using Core.Repositories;
using MobileAndroid.ViewHolders;

namespace MobileAndroid.Adapters
{
    public class RoutesAdapter : RecyclerView.Adapter
    {
        public event EventHandler<Route> RouteClick;
        private List<Route> _routes;
        private readonly RoutesRepository _routesRepository;
        public RoutesAdapter()
        {
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
                routeViewHolder.RouteName.Text = _routes[position].Name;
                routeViewHolder.RouteDistance.Text = _routes[position].Distance.ToString();
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