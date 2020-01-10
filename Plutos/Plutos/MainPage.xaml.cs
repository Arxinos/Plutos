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
using Windows.UI.ViewManagement;
using Windows.Graphics.Display;
using Windows.UI.Popups;

using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Plutos.ViewModels;
using System.Threading;
using Plutos.Single_Entry_Accounting;

// Die Elementvorlage "Leere Seite" wird unter https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x407 dokumentiert.

namespace Plutos
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>

    public sealed partial class MainPage : Page
{
        DispatcherTimer dispatcherTimer = new DispatcherTimer();
        
        public MainPage()
        {
            this.InitializeComponent();
            PrepareWindow();
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += SocketMethods.SendPackage;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 5);
            dispatcherTimer.Start();
        }
        bool sidebarmaxed = false;

        private void PrepareWindow()
        {
            var view = DisplayInformation.GetForCurrentView();

            // Get the screen resolution (APIs available from 14393 onward).
            var resolution = new Size(view.ScreenWidthInRawPixels, view.ScreenHeightInRawPixels);

            // Calculate the screen size in effective pixels. 
            // Note the height of the Windows Taskbar is ignored here since the app will only be given the maxium available size.
            var scale = view.ResolutionScale == ResolutionScale.Invalid ? 1 : view.RawPixelsPerViewPixel;
            var bounds = new Size(resolution.Width / scale, resolution.Height / scale);

            ApplicationView.PreferredLaunchViewSize = new Size(bounds.Width, bounds.Height);
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;

            var titlebar = ApplicationView.GetForCurrentView().TitleBar;
            titlebar.BackgroundColor = Windows.UI.Color.FromArgb(255, 51, 51, 51);
            titlebar.ButtonBackgroundColor = Windows.UI.Color.FromArgb(255, 51, 51, 51);
            MainFrame.Navigate(typeof(Dashboard));
        }
        /// <summary>
        /// Toggles the sidebar length between open and closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangeMode(object sender, RoutedEventArgs e)
        {
            if (sidebarmaxed)
            {
                Sidebar.Width = 70;
                sidebarmaxed = false;
                CompanyNameTextBlock.Visibility = Visibility.Collapsed;
            }
            else
            {
                Sidebar.Width = 200;
                sidebarmaxed = true;
                CompanyNameTextBlock.Visibility = Visibility.Visible;
            }
        } 

        private async void Tap(object sender, TappedRoutedEventArgs e)
        {
            ListViewItem clickedItem = sender as ListViewItem;
            switch (clickedItem.Name)
            {
                case "Dashboard":
                    MainFrame.Navigate(typeof(Dashboard));
                    break;
                case "Accounts":
                    MainFrame.Navigate(typeof(Accounts));
                    break;
                case "Booking":
                    MainFrame.Navigate(typeof(Single_Entry_Accounting.SE_Booking));
                    break;
                case "Customers":
                    MainFrame.Navigate(typeof(CustomersClass));
                    break;
            }
        }

        private void LogOut(object sender, TappedRoutedEventArgs e)
        {
            App.userID = App.compID = 0;
            Frame.Navigate(typeof(Login));
        }
    }
}