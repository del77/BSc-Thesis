using System;
using Android.Content;
using Android.Runtime;
using Android.Support.V4.App;
using MobileAndroid.Fragments;

namespace MobileAndroid.Adapters
{
    public class MainPagerAdapter : FragmentPagerAdapter
    {
        private readonly Context _context;
        private readonly Fragment[] _fragments;

        public MainPagerAdapter(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public MainPagerAdapter(Context context, FragmentManager fm) : base(fm)
        {
            _context = context;
            _fragments = new Fragment[] { new ActivityFragment(), new RoutesBrowserFragment() };
            Count = _fragments.Length;
        }

        public override int Count { get; }
        public override Fragment GetItem(int position)
        {
            return _fragments[position];
        }

        public override Java.Lang.ICharSequence GetPageTitleFormatted(int position)
        {
            if (position == 0)
                return new Java.Lang.String(_context.GetString(Resource.String.training_tab));
            else
                return new Java.Lang.String(_context.GetString(Resource.String.routes_tab));
        }
        
        public static string GetFragmentTag(int pos)
        {
            return "android:switcher:" + Resource.Id.mainPager + ":" + pos;
        }
    }
}