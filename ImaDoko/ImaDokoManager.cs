using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using System.Collections.ObjectModel;
using System.Diagnostics;


namespace ImaDoko
{
    public partial class ImaDokoManager
    {
        static ImaDokoManager defaultInstance = new ImaDokoManager();
        MobileServiceClient client;
        IMobileServiceTable<ImaDoko> imaDokoTable;

        const string offlineDbPath = @"localstore.db";

        private ImaDokoManager()
        {
            this.client = new MobileServiceClient(Constants.ApplicationURL);
            this.imaDokoTable = client.GetTable<ImaDoko>();
        }

        public static ImaDokoManager DefaultManager
        {
            get
            {
                return defaultInstance;
            }
            private set
            {
                defaultInstance = value;
            }
        }

        public async Task<ObservableCollection<ImaDoko>> GetImadokoItemsAsync(bool syncItems = false)
        {
            try
            {
                //現在もいる可能性のある人の情報だけ取得する
                IEnumerable<ImaDoko> items = await imaDokoTable.Where(x => !x.HasGone).ToEnumerableAsync();
                return new ObservableCollection<ImaDoko>(items);
            }
            catch (MobileServiceInvalidOperationException msioe)
            {
                Debug.WriteLine($"Invalid sync operation: {msioe.Message}");
            }
            catch(Exception e)
            {
                Debug.WriteLine($"Sync error: {e.Message}");
            }
            return null;
        }

        public async Task SaveImadokoTaskAsync(ImaDoko item)
        {
            if(item.Id == null)
            {
                await imaDokoTable.InsertAsync(item);
            }
            else
            {
                await imaDokoTable.UpdateAsync(item);
            }
        }
    }
}
