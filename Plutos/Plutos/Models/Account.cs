using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using MySql.Data.MySqlClient;
using System.ComponentModel;

namespace Plutos.Models
{
    class Account : INotifyPropertyChanged
    {
        private string name;

        public string Name {
            get { return name; }
            set 
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }

        private string description;

        public string Description {
            get { return description; }
            set 
            {
                description = value;
                OnPropertyChanged("Description");
            }
        }

        private double amount;

        public double Amount {
            get { return amount; }
            set 
            {
                amount = value;
                OnPropertyChanged("Amount");
            }
        }

        private int accID;

        public int AccID
        {
            get { return accID; }
            set
            {
                accID = value;
                OnPropertyChanged("AccID");
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
