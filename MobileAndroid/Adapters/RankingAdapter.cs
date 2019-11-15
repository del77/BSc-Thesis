using System;
using System.Collections.Generic;
using System.Linq;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;
using Core.Model;
using MobileAndroid.ViewHolders;

namespace MobileAndroid.Adapters
{
    public class RankingAdapter : RecyclerView.Adapter
    {
        private IEnumerable<RankingRecord> _rankingRecords;
        private int _checkpointIndexToDisplay;
        public RankingAdapter()
        {
        }

        public void UpdateRoute(ICollection<RankingRecord> rankingRecords)
        {
            _rankingRecords = rankingRecords;
            _checkpointIndexToDisplay = rankingRecords.Count() - 1;

            NotifyDataSetChanged();

            _checkpointIndexToDisplay = 0;
        }

        public void ShowDataForNextCheckpoint()
        {
            _rankingRecords = _rankingRecords.OrderBy(x => x.CheckpointsTimesList[_checkpointIndexToDisplay]);
            NotifyDataSetChanged();
            _checkpointIndexToDisplay++;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (holder is RankingViewHolder rankingViewHolder)
            {
                var rankingRecord = _rankingRecords.ElementAt(position);
                rankingViewHolder.Nickname.Text = rankingRecord.User;
                if(rankingRecord.IsMine)
                    rankingViewHolder.Nickname.SetTextColor(Color.Red);
                else
                    rankingViewHolder.Nickname.SetTextColor(Color.Black);

                rankingViewHolder.Time.Text = rankingRecord.CheckpointsTimes[_checkpointIndexToDisplay].ToString();
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView =
                LayoutInflater.From(parent.Context).Inflate(Resource.Layout.ranking_viewholder, parent, false);
            
            var rankingViewHolder = new RankingViewHolder(itemView);
            return rankingViewHolder;
        }

        public override int ItemCount => _rankingRecords.Count();
    }
}