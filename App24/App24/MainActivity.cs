using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Content.PM;
using Android.Telephony;
using System;
using Android;
using Android.Content;
using Android.Support.V4.Content;
using Android.Support.V4.App;
using Android.Util;
using Android.Support.Design.Widget;
using Android.Views;

namespace App24
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, ActivityCompat.IOnRequestPermissionsResultCallback 
    {
        static readonly int REQUEST_SENDSMS = 0;
        View layout;
        private SmsManager _smsManager;
        private BroadcastReceiver _smsSentBroadcastReceiver, _smsDeliveredBroadcastReceiver;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.layout1);
            layout = FindViewById(Resource.Id.sample_main_layout);
            Button smsBtn = FindViewById<Button>(Resource.Id.btnSend);
            EditText phoneNum = FindViewById<EditText>(Resource.Id.phoneNum);
            EditText sms = FindViewById<EditText>(Resource.Id.txtSMS);
            _smsManager = SmsManager.Default;

            smsBtn.Click += (s, e) =>
            {
                var phone = phoneNum.Text;
                var message = sms.Text;

                var piSent = PendingIntent.GetBroadcast(this, 0, new Intent("SMS_SENT"), 0);
                var piDelivered = PendingIntent.GetBroadcast(this, 0, new Intent("SMS_DELIVERED"), 0);

                if (ActivityCompat.CheckSelfPermission(this, Manifest.Permission.SendSms) != (int)Permission.Granted)
                {
                    // Permission is not granted. If necessary display rationale & request.
                    RequestSendSMSPermission();
                }
                else
                {
                    // We have permission, go ahead and send SMS.
                    _smsManager.SendTextMessage(phone, null, message, piSent, piDelivered);
                }


                
            };
        }
        protected override void OnResume()
        {
            base.OnResume();

            _smsSentBroadcastReceiver = new SMSSentReceiver();
            _smsDeliveredBroadcastReceiver = new SMSDeliveredReceiver();

            RegisterReceiver(_smsSentBroadcastReceiver, new IntentFilter("SMS_SENT"));
            RegisterReceiver(_smsDeliveredBroadcastReceiver, new IntentFilter("SMS_DELIVERED"));
        }

        protected override void OnPause()
        {
            base.OnPause();

            UnregisterReceiver(_smsSentBroadcastReceiver);
            UnregisterReceiver(_smsDeliveredBroadcastReceiver);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }


        void RequestSendSMSPermission()
        {
            Log.Info("MainActivity", "CAMERA permission has NOT been granted. Requesting permission.");

            if (ActivityCompat.ShouldShowRequestPermissionRationale(this, Manifest.Permission.SendSms))
            {
                // Provide an additional rationale to the user if the permission was not granted
                // and the user would benefit from additional context for the use of the permission.
                // For example if the user has previously denied the permission.
                Log.Info("MainActivity", "Displaying camera permission rationale to provide additional context.");
                //Activity activity = CrossCurrentActivity.Current.Activity;
                //Android.Views.View activityRootView = activity.FindViewById(Android.Resource.Id.Content);
                Snackbar.Make(layout, "Camera permission is needed to show the camera preview.",
                    Snackbar.LengthIndefinite).SetAction("OK", new Action<View>(delegate (View obj) {
                        ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.SendSms }, REQUEST_SENDSMS);
                    })).Show();
            }
            else
            {
                // Camera permission has not been granted yet. Request it directly.
                ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.SendSms }, REQUEST_SENDSMS);
            }
        }
    }







    [BroadcastReceiver]
    public class SMSSentReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            switch ((int)ResultCode)
            {
                case (int)Result.Ok:
                    Toast.MakeText(Application.Context, "SMS has been sent", ToastLength.Short).Show();
                    break;
                case (int)SmsResultError.GenericFailure:
                    Toast.MakeText(Application.Context, "Generic Failure", ToastLength.Short).Show();
                    break;
                case (int)SmsResultError.NoService:
                    Toast.MakeText(Application.Context, "No Service", ToastLength.Short).Show();
                    break;
                case (int)SmsResultError.NullPdu:
                    Toast.MakeText(Application.Context, "Null PDU", ToastLength.Short).Show();
                    break;
                case (int)SmsResultError.RadioOff:
                    Toast.MakeText(Application.Context, "Radio Off", ToastLength.Short).Show();
                    break;
            }
        }
    }

    [BroadcastReceiver]
    public class SMSDeliveredReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            switch ((int)ResultCode)
            {
                case (int)Result.Ok:
                    Toast.MakeText(Application.Context, "SMS Delivered", ToastLength.Short).Show();
                    break;
                case (int)Result.Canceled:
                    Toast.MakeText(Application.Context, "SMS not delivered", ToastLength.Short).Show();
                    break;
            }
        }           

        
    }
}