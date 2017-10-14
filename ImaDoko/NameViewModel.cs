using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImaDoko;

namespace ImaDoko
{
    class NameViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// INotifyPropertyChangedインターフェイスの実装
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private string myName;

        public string MyName
        {
            get
            {
                return myName;
            }
            set
            {
                if(myName != value)
                {
                    myName = value;
                    OnPropertyChanged("MyName");
                }
            }
        }

        // プロパティ値の変更を通知します
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
