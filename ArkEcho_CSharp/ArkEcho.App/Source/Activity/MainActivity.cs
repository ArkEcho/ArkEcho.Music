using Android.App;
using Android.Widget;
using Android.OS;

using System;
using System.Threading.Tasks;

using arkecho_app.source.connection;

namespace arkecho_app.source.activity
{
    [Activity]
    public class MainActivity : ExtendedActivity
    {
        //string qrCodeText;
        //MobileBarcodeScanner scanner_;

        Button connectWithQrButton;
        Button connectManuallyButton;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            setActionBarButtonBackHidden(true);
            setActionBarTitleText(GetString(Resource.String.MainActivityTitle));

            // Connect Buttons
            connectWithQrButton = FindViewById<Button>(Resource.Id.pbConnectWithQr);
            //connectWithQrButton.Click += onPbConnectWithQrClicked;
            connectManuallyButton = FindViewById<Button>(Resource.Id.pbConnectManually);
            connectManuallyButton.Click += onPbConnectManuallyClicked;

            // Scanner initialisieren
            //MobileBarcodeScanner.Initialize(Application);
            //scanner_ = new MobileBarcodeScanner();
            //qrCodeText = "";
        }

        private void setElementsEnabled(bool enabled)
        {
            //connectWithQrButton.Enabled = enabled;
            connectManuallyButton.Enabled = enabled;
            FindViewById<TextView>(Resource.Id.teAddress).Enabled = enabled;
        }

        private void onPbConnectManuallyClicked(object sender, EventArgs e)
        {
            setElementsEnabled(false);
            string address = FindViewById<TextView>(Resource.Id.teAddress).Text;

            if (!ArkEchoWebSocket.checkIfURIAddressIsCorrect(address))
            {
                showMessageBoxEmptyWrongAddressField();
                setElementsEnabled(true);
                return;
            }

            connectAndOpenPlayer(address);
        }
        
        //private async void onPbConnectWithQrClicked(object sender, System.EventArgs e)
        //{
        //    setElementsEnabled(false);
        //    Task scan  = scanQrCode();
        //    await scan;

        //    if (!ArkEchoWebSocket.checkIfURIAddressIsCorrect(qrCodeText))
        //    {
        //        showMessageBoxQrScanFailed();
        //        setElementsEnabled(true);
        //        return;
        //    }

        //    connectAndOpenPlayer(qrCodeText);
        //}

        private async void connectAndOpenPlayer(string address)
        {
            Task connect = ArkEchoWebSocket.connectWebSocket(address);
            await connect;

            setElementsEnabled(true);

            if (ArkEchoWebSocket.checkIfConnectionIsOpen())
            {
                Vibrator vibrator = (Vibrator)this.GetSystemService(VibratorService);
                vibrator.Vibrate(VibrationEffect.CreateOneShot(500, 10));
                StartActivity(typeof(PlayerActivity));
            }
            else
                showMessageBoxNoConnection();
        }

        //private void showMessageBoxQrScanFailed()
        //{
        //    Toast mrToast = Toast.MakeText(this, Resource.String.ToastQrScanFailed, ToastLength.Short);
        //    mrToast.Show();
        //}

        private void showMessageBoxEmptyWrongAddressField()
        {
            Toast mrToast = Toast.MakeText(this, Resource.String.ToastEmptyWrongAddress,ToastLength.Short);
            mrToast.Show();
        }

        private void showMessageBoxNoConnection()
        {
            Toast mrToast = Toast.MakeText(this, Resource.String.ToastNoConnection, ToastLength.Short);
            mrToast.Show();
        }

        //private async Task scanQrCode()
        //{
        //    try
        //    {
        //        scanner_.TopText = GetString(Resource.String.QrScannerTextTop);
        //        scanner_.BottomText = GetString(Resource.String.QrScannerTextBottom);

        //        var result = await scanner_.Scan();
        //        if (result != null) qrCodeText = result.Text;
        //        else qrCodeText = "";
        //    }
        //    catch (Exception ex)
        //    {
        //        ex.ToString();
        //    }
        //}
    }
}

