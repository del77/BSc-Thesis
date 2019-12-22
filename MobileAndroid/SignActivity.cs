using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Core.Repositories;
using Core.Services;

namespace MobileAndroid
{
    [Activity(Label = "SignActivity", MainLauncher = true)]
    public class SignActivity : Activity
    {
        private EditText _usernameEditText;
        private EditText _passowrdEditText;
        private Button _registerButton;
        private Button _loginButton;
        private TextView _signResultTextView;
        private ProgressBar _signProgressBar;

        private UserRepository _userRepository;
        private UserService _userService;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.sign_view);

            _userRepository = new UserRepository();
            _userService = new UserService();

            var userData = _userRepository.GetUserData();
            if (userData == null)
            {
                FindViews();
                LinkEventHandlers();
            }
            else
            {
                OpenMainScreen();
            }
        }

        private void FindViews()
        {
            _usernameEditText = FindViewById<EditText>(Resource.Id.username_input);
            _passowrdEditText = FindViewById<EditText>(Resource.Id.password_input);
            _registerButton = FindViewById<Button>(Resource.Id.register_button);
            _loginButton = FindViewById<Button>(Resource.Id.login_button);
            _signResultTextView = FindViewById<TextView>(Resource.Id.sign_result_information);
            _signProgressBar = FindViewById<ProgressBar>(Resource.Id.sign_progressbar);
        }

        private void LinkEventHandlers()
        {
            _registerButton.Click += RegisterButtonOnClick;
            _loginButton.Click += LoginButtonOnClick;
        }

        private async void LoginButtonOnClick(object sender, EventArgs e)
        {
            _signProgressBar.Visibility = ViewStates.Visible;

            string username = _usernameEditText.Text;
            string password = _passowrdEditText.Text;

            var result = await _userService.Login(username, password);

            if (result.Item1)
            {
                OpenMainScreen();
            }
            else
            {
                _signResultTextView.Text = Resources.GetString(Resources.GetIdentifier(result.Item2, "string", PackageName));
                _signResultTextView.Visibility = ViewStates.Visible;
            }
        }

        private async void RegisterButtonOnClick(object sender, EventArgs e)
        {
            _signProgressBar.Visibility = ViewStates.Visible;

            string username = _usernameEditText.Text;
            string password = _passowrdEditText.Text;
            
            var result = await _userService.RegisterAccount(username, password);
            _signProgressBar.Visibility = ViewStates.Invisible;

            _signResultTextView.Text = Resources.GetString(Resources.GetIdentifier(result, "string", PackageName));
            _signResultTextView.Visibility = ViewStates.Visible;
        }

        private void OpenMainScreen()
        {
            var routeDetailsIntent = new Intent(this, typeof(MainActivity));
            StartActivity(routeDetailsIntent);
        }
    }
}