using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plutos.Models;
using System.Diagnostics;
using Plutos.Commands;
using Windows.UI.Popups;
using Newtonsoft.Json;
using Plutos.ClientCommunication;
using Windows.UI.Xaml;

namespace Plutos.ViewModels
{
    class CompanyVM : INotifyPropertyChanged
    {
        DispatcherTimer dispatcherTimer = new DispatcherTimer();
        public CompanyVM()
        {
            //ExecuteLoadCompanies();
            CompanyChangedCommand = new CommandAsync(SetCompID);
            dispatcherTimer.Tick += Refresh;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
        }

        private void Refresh(object state, object e)
        {
            if(SocketMethods.data.Companies != null)
            {
                Companies = SocketMethods.data.Companies;
                SocketMethods.data.Companies = null;
            }
            else
            {
                return;
            }
        }

        private void ExecuteLoadCompanies()
        {
            LoadCompanies();
        }

        private ObservableCollection<Company> companies;                 
        public ObservableCollection<Company> Companies 
        {
            get { return companies; }
            set 
            {
                companies = value;
                OnPropertyChanged("Companies");
            }
        }

        private int selectedCompanyID;

        public int SelectedCompanyID {
            get { return selectedCompanyID; }
            set 
            {
                selectedCompanyID = value;
                OnPropertyChanged("SelectedCompanyID");
            }
        }

        private string selectedCompanyName;

        public string SelectedCompanyName
        {
            get { return selectedCompanyName; }
            set
            {
                selectedCompanyName = value;
                OnPropertyChanged("SelectedCompanyName");
            }
        }



        public CommandAsync CompanyChangedCommand { get; set; }

        public async Task LoadCompanies()
        {
            await Task.Run(() => SocketMethods.GetData());
            Companies = SocketMethods.data.Companies;
        }


        private async Task SetCompID()
        {
            App.compID = SelectedCompanyID;
            await Task.Run(() => SocketMethods.ChangeCompany()); 
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if(PropertyChanged != null)
            {
                PropertyChanged(typeof(CompanyVM), new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
