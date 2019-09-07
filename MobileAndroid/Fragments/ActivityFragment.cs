using Android.OS;
using Android.Support.V4.App;
using Android.Views;

namespace MobileAndroid.Fragments
{
    public class ActivityFragment : Fragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.activity_fragment, container, false);

            return view;
        }
    }

    
}