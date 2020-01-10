using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using MySql.Data.MySqlClient;
using Windows.UI.Xaml.Documents;
using Plutos.Models;
using Plutos.ViewModels;
using System.Collections.ObjectModel;

// Die Elementvorlage "Leere Seite" wird unter https://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace Plutos
{
    
    
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class CompanySetup : Page
    {
        ObservableCollection<Account> accounts = new ObservableCollection<Account>();
        public CompanySetup()
        {
            this.InitializeComponent();
            accounts.Add(new Account { Name = "Bank" });
            accounts.Add(new Account { Name = "Kassa" });
            accounts.Add(new Account { Name = "Kundenforderungen" });
            accounts.Add(new Account { Name = "Lieferverbindlichkeiten" });
            accounts.Add(new Account { Name = "Darlehen" });
            accounts.Add(new Account { Name = "Material" });
            accounts.Add(new Account { Name = "Waren" });
        }

        private void ModeChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (BookingModeCB.SelectedIndex)
            {
                case 0:
                    StandardAccountsTB.Visibility = Visibility.Collapsed;
                    AccountLine.Visibility = Visibility.Collapsed;
                    AccountList.Visibility = Visibility.Collapsed;
                    break;
                case 1:
                    StandardAccountsTB.Visibility = Visibility.Visible;
                    AccountLine.Visibility = Visibility.Visible;
                    AccountList.Visibility = Visibility.Visible;
                    break;
            }
        }
    }
}
