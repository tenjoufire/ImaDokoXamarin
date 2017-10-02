using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Plugin.Connectivity;
using System.Linq;
using System.Collections.Generic;

namespace ImaDoko
{
    public partial class TodoList : ContentPage
    {
        ImaDokoManager manager;
        //Dictionary<string, string> wifiPlace = new Dictionary<string, string>();

        public TodoList()
        {
            InitializeComponent();

            manager = ImaDokoManager.DefaultManager;

            /*
            wifiPlace = new Dictionary<string, string>()
            {
                {"sumilab-wlan-g", "1F卒研スペース付近" },
                {"turtlebot", "エレ工か院生室" },
                {"eduroam", "学内" },
                {"free-wifi", "学内" },
                {"fun-wifi", "学内" },
                {String.Empty , "Wifi圏外もしくは学外"}
            };
            */

            //wifiPlace.Add("sumilab-wlan-g", "1F卒研スペース付近");


            // OnPlatform<T> doesn't currently support the "Windows" target platform, so we have this check here.
            /*if (manager.IsOfflineEnabled && Device.OS == TargetPlatform.Windows)
            {
                var syncButton = new Button
                {
                    Text = "Sync items",
                    HeightRequest = 30
                };
                syncButton.Clicked += OnSyncItems;

                buttonsPanel.Children.Add(syncButton);
            }*/
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Set syncItems to true in order to synchronize the data on startup when running in offline mode
            await RefreshItems(true, syncItems: true);
        }

        // Data methods
        async Task AddItem(ImaDoko item)
        {
            await manager.SaveImadokoTaskAsync(item);
            todoList.ItemsSource = await manager.GetImadokoItemsAsync();
        }

        async Task CompleteItem(ImaDoko item)
        {
            item.HasGone = true;
            await manager.SaveImadokoTaskAsync(item);
            todoList.ItemsSource = await manager.GetImadokoItemsAsync();
        }

        public async void OnAdd(object sender, EventArgs e)
        {
            var todo = new ImaDoko { Name = newItemName.Text, Place = GetCurrentPlaceFromSSID() };
            await AddItem(todo);

            newItemName.Text = string.Empty;
            newItemName.Unfocus();
            newPlace.Text = string.Empty;
            newPlace.Unfocus();
        }

        // Event handlers
        public async void OnSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var todo = e.SelectedItem as ImaDoko;
            if (Device.OS != TargetPlatform.iOS && todo != null)
            {
                // Not iOS - the swipe-to-delete is discoverable there
                if (Device.OS == TargetPlatform.Android)
                {
                    /*
                     *  for debug
                    var wifiInfo = DependencyService.Get<IWificonnection>();
                    Console.WriteLine(wifiInfo.GetSSID());
                    */
                    await DisplayAlert(todo.Name, "Press-and-hold to complete task " + todo.Name, "Got it! ");
                }
                else
                {
                    // Windows, not all platforms support the Context Actions yet
                    if (await DisplayAlert("Mark completed?", "Do you wish to complete " + todo.Name + "?", "Complete", "Cancel"))
                    {
                        await CompleteItem(todo);
                    }
                }
            }

            // prevents background getting highlighted
            todoList.SelectedItem = null;
        }

        // http://developer.xamarin.com/guides/cross-platform/xamarin-forms/working-with/listview/#context
        public async void OnComplete(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            var todo = mi.CommandParameter as ImaDoko;
            await CompleteItem(todo);
        }

        // http://developer.xamarin.com/guides/cross-platform/xamarin-forms/working-with/listview/#pulltorefresh
        public async void OnRefresh(object sender, EventArgs e)
        {
            var list = (ListView)sender;
            Exception error = null;
            try
            {
                await RefreshItems(false, true);
            }
            catch (Exception ex)
            {
                error = ex;
            }
            finally
            {
                list.EndRefresh();
            }

            if (error != null)
            {
                await DisplayAlert("Refresh Error", "Couldn't refresh data (" + error.Message + ")", "OK");
            }
        }

        public async void OnSyncItems(object sender, EventArgs e)
        {
            await RefreshItems(true, true);
        }

        private async Task RefreshItems(bool showActivityIndicator, bool syncItems)
        {
            using (var scope = new ActivityIndicatorScope(syncIndicator, showActivityIndicator))
            {
                todoList.ItemsSource = await manager.GetImadokoItemsAsync(syncItems);
            }
        }

        private class ActivityIndicatorScope : IDisposable
        {
            private bool showIndicator;
            private ActivityIndicator indicator;
            private Task indicatorDelay;

            public ActivityIndicatorScope(ActivityIndicator indicator, bool showIndicator)
            {
                this.indicator = indicator;
                this.showIndicator = showIndicator;

                if (showIndicator)
                {
                    indicatorDelay = Task.Delay(2000);
                    SetIndicatorActivity(true);
                }
                else
                {
                    indicatorDelay = Task.FromResult(0);
                }
            }

            private void SetIndicatorActivity(bool isActive)
            {
                this.indicator.IsVisible = isActive;
                this.indicator.IsRunning = isActive;
            }

            public void Dispose()
            {
                if (showIndicator)
                {
                    indicatorDelay.ContinueWith(t => SetIndicatorActivity(false), TaskScheduler.FromCurrentSynchronizationContext());
                }
            }

        }

        private string GetCurrentPlaceFromSSID()
        {
            var ssid = DependencyService.Get<IWificonnection>().GetSSID();
            var wifiPlace = new Dictionary<string, string>()
            {
                ["sumilab-wlan-g"] = "1F卒研スペース付近",
                ["turtlebot"] = "エレ工か院生室",
                ["eduroam"] = "学内",
                ["fun-wifi"] = "学内",
                ["free-wifi"] = "学内",
                [String.Empty] = "Wifi圏外か学外",
            };
            if (wifiPlace.ContainsKey(ssid))
            {
                return wifiPlace[ssid];
            }

            return "対象外のネットワーク";
        }

        //wifiのSSIDを取得するクラスのインターフェース
        public interface IPlatformService
        {
            string GetSSID();
        }
    }
}

