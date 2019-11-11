using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace MobileAndroid.ViewHolders
{
    public class RankingViewHolder : RecyclerView.ViewHolder
    {
        private TextView _nickname;

        public TextView Nickname
        {
            get => _nickname;
            set
            {
                _nickname = value;
                if(_nickname.Text == "You")
                    Nickname.SetTextColor(Color.Red);
            }
        }

        public TextView Time { get; set; }
        public RankingViewHolder(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public RankingViewHolder(View itemView) : base(itemView)
        {
            FindViews();
        }

        private void FindViews()
        {
            Nickname = ItemView.FindViewById<TextView>(Resource.Id.ranking_nickname);
            Time = ItemView.FindViewById<TextView>(Resource.Id.ranking_time);
        }
    }
}