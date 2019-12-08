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
        private Route _route;
        private int _checkpointIndexToDisplay;
        private int _nextCheckpointIndex;
        private int _nextCheckpointIndex2;

        public RankingAdapter()
        {
        }

        public void UpdateRoute(Route route)
        {
            _route = route;

            if (route.Ranking.Any())
            {
                _route.Ranking = _route.Ranking.OrderBy(rr => rr.FinalResult).ToList();
                _checkpointIndexToDisplay = route.Ranking.First().CheckpointsTimes.Count() - 1;
                _nextCheckpointIndex2 = route.Ranking.First().CheckpointsTimes.Count() - 1;
            }

            NotifyDataSetChanged();

            _nextCheckpointIndex = 0;
        }

        public void ShowDataForNextCheckpoint()
        {
            _nextCheckpointIndex2 = _route.Ranking.First(rr => rr.CurrentTry).CheckpointsTimes.Count;

            _checkpointIndexToDisplay = _nextCheckpointIndex;
            //if (_nextCheckpointIndex > 0)
            //     _route.Ranking = _route.Ranking.OrderBy(x => x.CheckpointsTimes[_nextCheckpointIndex - 1]).ToList();
            NotifyDataSetChanged();
            _nextCheckpointIndex++;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (holder is RankingViewHolder rankingViewHolder)
            {
                var rankingRecord = _route.Ranking.ElementAt(position);
                if (rankingRecord.IsMine)
                {
                    rankingViewHolder.Nickname.SetTextColor(Color.Red);
                    rankingViewHolder.Nickname.Text = rankingRecord.User;
                }
                else if (rankingRecord.CurrentTry)
                {
                    rankingViewHolder.Nickname.SetTextColor(Color.Blue);
                    rankingViewHolder.Nickname.Text = "Current try";
                }
                else
                {
                    rankingViewHolder.Nickname.SetTextColor(Color.Black);
                    rankingViewHolder.Nickname.Text = rankingRecord.User;
                }

                rankingViewHolder.Time.Text = rankingRecord.CurrentTry ? 
                    "" : rankingRecord.CheckpointsTimes[_nextCheckpointIndex2].ToString();
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView =
                LayoutInflater.From(parent.Context).Inflate(Resource.Layout.ranking_viewholder, parent, false);

            var rankingViewHolder = new RankingViewHolder(itemView);
            return rankingViewHolder;
        }

        public override int ItemCount => _route.Ranking.Count();
    }
}