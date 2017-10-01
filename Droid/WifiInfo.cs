using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Net.Wifi;
using Xamarin.Forms;

[assembly: Dependency (typeof(ImaDoko.Droid.WifiInfo))]
namespace ImaDoko.Droid
{
    public class WifiInfo : IWificonnection
    {
        /// <summary>
        /// Get current wifi SSID
        /// </summary>
        /// <returns>returns SSID (if wifi is not available, it returns String.Enpty.)</returns>
        public string GetSSID()
        {
            var wifiManager = (WifiManager)Android.App.Application.Context.GetSystemService(Context.WifiService);

            var ssid = wifiManager.ConnectionInfo.SSID;
            var bssid = wifiManager.ConnectionInfo.BSSID;

            if (!String.IsNullOrEmpty(ssid) && ssid != "<unknown ssid>")
            {
                return ssid.Trim(' ', '"');
            }

            if (!String.IsNullOrEmpty(bssid))
            {
                return bssid.Trim(' ', '"');
            }

            return String.Empty;
        }
    }
}