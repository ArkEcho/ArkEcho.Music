using Android.App;
using Android.Widget;
using Android.OS;

using System;
using System.Threading.Tasks;

using ArkEcho.Core;
using ArkEcho.App.Connection;
using System.Collections.Generic;
using Android.Content;
using System.IO;

namespace ArkEcho.App
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
            //ArkEchoRest rest = new ArkEchoRest();
            //string test = rest.getWeather();

            //setElementsEnabled(false);
            //string address = FindViewById<TextView>(Resource.Id.teAddress).Text;

            //if (!ArkEchoWebSocket.checkIfURIAddressIsCorrect(address))
            //{
            //    showMessageBoxEmptyWrongAddressField();
            //    setElementsEnabled(true);
            //    return;
            //}

            //connectAndOpenPlayer(address);

            string sdCardMusicFolder = GetMusicSDFolderPath();

            string pathnew = $"{sdCardMusicFolder}Alligatoah/Triebwerke/Alligatoah - Amnesie.mp3";
            MusicFile file = new MusicFile(pathnew);
            file.LocalFileName = pathnew;

            AppModel.Instance.StartPlayer(new List<MusicFile> { file },0);
        }

        public static string GetMusicSDFolderPath()
        {
            string baseFolderPath = string.Empty;
            try
            {
                Context context = Application.Context;
                                
                Java.IO.File[] dirs = context.GetExternalFilesDirs(null);

                foreach (Java.IO.File folder in dirs)
                {
                    bool IsRemovable = Android.OS.Environment.InvokeIsExternalStorageRemovable(folder);
                    bool IsEmulated = Android.OS.Environment.InvokeIsExternalStorageEmulated(folder);

                    if (IsRemovable && !IsEmulated)
                    {
                        baseFolderPath = folder.Path.Substring(0, folder.Path.IndexOf("Android/") + 8);
                        baseFolderPath += "Music/";
                    }
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine("GetBaseFolderPath caused the follwing exception: {0}", ex);
            }

            return baseFolderPath;

            
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
            await Task.Delay(10);
            setElementsEnabled(true);

            //if (ArkEchoWebSocket.checkIfConnectionIsOpen())
            //{
            //    Vibrator vibrator = (Vibrator)this.GetSystemService(VibratorService);
            //    vibrator.Vibrate(VibrationEffect.CreateOneShot(500, 10));
            //    StartActivity(typeof(PlayerActivity));
            //}
            //else
            //    showMessageBoxNoConnection();
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

