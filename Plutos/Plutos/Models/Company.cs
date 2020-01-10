using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using System.ComponentModel;

namespace Plutos.Models
{
    class Company : INotifyPropertyChanged
    {
        private int compID;
        public int CompID {
            get { return compID; }
            set 
            {
                compID = value;
                OnPropertyChanged("CompID");
            }
        }

        private string compName;
        public string CompName {
            get { return compName; }
            set 
            {
                compName = value;
                OnPropertyChanged("CompName");
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if(PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
