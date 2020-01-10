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
using MySql.Data;
using MySql.Data.MySqlClient;
using Windows.UI.Input;
using System.Net.Sockets;
using System.Globalization;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.UI.Popups;
// Die Elementvorlage "Leere Seite" wird unter https://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace Plutos
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class Login : Page
    {

        public Login()
        {
            this.InitializeComponent();
            LanguageBox.ItemsSource = new List<string>{"Deutsch","English" };
        }

        private void RegisterUser(object sender, PointerRoutedEventArgs e)
        {
            Frame.Navigate(typeof(Register));
        }

        private async void LoginUser(object sender, RoutedEventArgs e)
        {
            Windows.UI.Popups.MessageDialog md = new Windows.UI.Popups.MessageDialog("");

            if (!CheckForInternetConnection())
            {
                md.Content = "No internet acess. Please check your connection";
                await md.ShowAsync();
                return;
            }
            
            if (String.IsNullOrEmpty(EmailTextBox.Text))
            {
                md.Content = "No email entered";
                await md.ShowAsync();
                return;
            }
            if (String.IsNullOrEmpty(PWDB.Password.ToString()))
            {
                md.Content = "No password entered";
                await md.ShowAsync();
                return;
            }
            try
            {
                if(await SocketMethods.StartClient(EmailTextBox.Text, PWDB.Password) == true)
                    Frame.Navigate(typeof(MainPage));
            }
            catch
            {
                md.Content = "Invalid E-Mail or password";
                await md.ShowAsync();
            }
           
        }

        public static bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new System.Net.WebClient())
                using (var stream = client.OpenRead("http://www.google.com"))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }


        private void EnterPressed(object sender, KeyRoutedEventArgs e)
        {
            if(e.Key == Windows.System.VirtualKey.Enter)
            {
                LoginUser(this, new RoutedEventArgs());
            }
        }

        private void ChangeLanguage(object sender, SelectionChangedEventArgs e)
        {
            switch(LanguageBox.SelectedIndex)
            {
                case 0:
                    Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = "de";
                    break;
                case 1:
                    Windows.Globalization.ApplicationLanguages.PrimaryLanguageOverride = "en-US";
                    break;
            }
            Frame.Navigate(this.GetType());
        }
    }
}
