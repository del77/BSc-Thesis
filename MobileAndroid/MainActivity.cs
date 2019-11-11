using System.Collections.Generic;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Preferences;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Support.V4.View;
using Android.Telephony;
using Android.Views;
using Android.Widget;
using Core.Model;
using Core.Repositories;
using MobileAndroid.Adapters;
using AlertDialog = Android.App.AlertDialog;

namespace MobileAndroid
{
    [Activity(Label = "@string/app_name", Theme = "@style/Theme.AppCompat.Light.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private ISharedPreferences _sharedPreferences;
        private EditText _nicknameEditText;
        private AlertDialog _alertDialog;
        private const string UserDetailsKey = "userDetails";
        private UserRepository _userRepository;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            _userRepository = new UserRepository();
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.ReadPhoneState)
                == Permission.Denied)
            {
                var permissions = new[] { Manifest.Permission.ReadPhoneState };
                ActivityCompat.RequestPermissions(this, permissions, 1);
            }

            var pager = FindViewById<ViewPager>(Resource.Id.mainPager);
            pager.Adapter = new MainPagerAdapter(this, SupportFragmentManager);

            _sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(this);


        }

        protected override void OnResume()
        {
            //var userData = _sharedPreferences.GetStringSet(UserDetailsKey, null);
            var userData = _userRepository.GetUserData();
            if (userData == null)
            {
                if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.ReadPhoneState)
                    == Permission.Granted)
                {

                    TelephonyManager tMgr = (TelephonyManager)GetSystemService(TelephonyService);
                    string mPhoneNumber = tMgr.Line1Number;

                    var alertDialogBuilder = new AlertDialog.Builder(this);
                    _nicknameEditText = new EditText(this);

                    alertDialogBuilder.SetView(_nicknameEditText);

                    alertDialogBuilder.SetMessage("Enter Your nickname");
                    alertDialogBuilder.SetPositiveButton("Ok", SetUserDetails);

                    _alertDialog = alertDialogBuilder.Create();
                    _alertDialog.Show();

                }
                else
                {
                    FinishActivity(1);
                }
            }
            else
            {
                //var preferencesEditor = _sharedPreferences.Edit();
                //preferencesEditor.PutStringSet(UserDetailsKey, null).Commit();
            }
            base.OnResume();

        }

        private void SetUserDetails(object sender, DialogClickEventArgs e)
        {
            TelephonyManager tMgr = (TelephonyManager)GetSystemService(TelephonyService);
            string phoneNumber = tMgr.Line1Number;

            var userData = new UserData(phoneNumber, _nicknameEditText.Text);
            _userRepository.CreateUserData(userData);
        }


        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}