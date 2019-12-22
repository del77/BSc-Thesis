using System.Linq;
using Android.Content;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;
using Core.Extensions;
using Core.Model;
using MobileAndroid.ViewHolders;

namespace MobileAndroid.Adapters
{
    public class RankingAdapter : RecyclerView.Adapter
    {
        private Route _route;
        private int _currentCheckpointIndex;
        private Context _context;

        public void UpdateRoute(Route route)
        {
            _route = route;

            if (route.Ranking.Any())
            {
                _route.Ranking = _route.Ranking.OrderBy(rr => rr.FinalResult).ToList();
                _currentCheckpointIndex = route.Ranking.First().CheckpointsTimes.Count() - 1;
            }

            NotifyDataSetChanged();
        }

        public void ShowDataForNextCheckpoint()
        {
            _currentCheckpointIndex = _route.Ranking.First(rr => rr.CurrentTry).CheckpointsTimes.Count - 1;

            NotifyDataSetChanged();
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (holder is RankingViewHolder rankingViewHolder)
            {
                var rankingRecord = _route.Ranking.ElementAt(position);
                rankingViewHolder.Nickname.Text = $"{position + 1}. ";
                if (rankingRecord.IsMine)
                {
                    rankingViewHolder.Nickname.SetTextColor(Color.Green);
                    rankingViewHolder.Nickname.Text += rankingRecord.User;
                }
                else if (rankingRecord.CurrentTry)
                {
                    rankingViewHolder.Nickname.SetTextColor(Color.Blue);
                    rankingViewHolder.Nickname.Text += _context.GetString(Resource.String.current_try);
                }
                else
                {
                    rankingViewHolder.Nickname.SetTextColor(Color.Black);
                    rankingViewHolder.Nickname.Text += rankingRecord.User;
                }


                var currentTime = rankingRecord.CheckpointsTimes[_currentCheckpointIndex].SecondsToStopwatchTimeString();
                rankingViewHolder.Time.Text = currentTime;
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            _context = parent.Context;
            View itemView =
                LayoutInflater.From(_context).Inflate(Resource.Layout.ranking_viewholder, parent, false);

            var rankingViewHolder = new RankingViewHolder(itemView);
            return rankingViewHolder;
        }

        public override int ItemCount => _route.Ranking.Count();
    }
}