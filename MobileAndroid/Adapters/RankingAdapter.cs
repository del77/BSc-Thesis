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
        private int _nextCheckpointIndexToDisplay;


        public RankingAdapter()
        {
        }

        public void UpdateRoute(ICollection<RankingRecord> rankingRecords)
        {
            _rankingRecords = rankingRecords;

            if(rankingRecords.Any())
                _checkpointIndexToDisplay = rankingRecords.First().CheckpointsTimes.Count() - 1;

            NotifyDataSetChanged();

            _nextCheckpointIndexToDisplay = 0;
        }

        public void ShowDataForNextCheckpoint()
        {
            _checkpointIndexToDisplay = _nextCheckpointIndexToDisplay;
            _rankingRecords = _rankingRecords.OrderBy(x => x.CheckpointsTimes[_checkpointIndexToDisplay]);
            NotifyDataSetChanged();
            _nextCheckpointIndexToDisplay++;
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