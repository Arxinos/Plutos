using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plutos.Models
{
    class User : INotifyPropertyChanged
    {
        private int id;
        public int ID {
            get { return id; }
            set {
                id = value;
                OnPropertyChanged("ID");
            }
        }

        private string email;

        public string Email {
            get { return email; }
            set {
                email = value;
                OnPropertyChanged("Email");
            }
        }

        private string password;

        public string Password {
            get { return password; }
            set {
                password = value;
                OnPropertyChanged("Password");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
