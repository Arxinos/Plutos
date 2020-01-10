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
using Windows.UI.Xaml.Media.Imaging;
using MySql.Data.MySqlClient;
using System.Collections.ObjectModel;
using WinRTXamlToolkit.Controls.Data;
using Plutos.ViewModels;
using System.Threading.Tasks;
using WinRTXamlToolkit.Controls.DataVisualization.Charting;
// Die Elementvorlage "Leere Seite" wird unter https://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace Plutos
{
    /// <summary>
    /// Contains graphics and charts about the the finances of a foundation
    /// </summary>
    public sealed partial class Dashboard : Page
    {
        DispatcherTimer dispatcherTimer = new DispatcherTimer();
        public Dashboard()
        {
            this.InitializeComponent();
            this.DataContext = new CompanyVM();
            CompanySelect.SelectedIndex = 0;
        }       

        public void NewCompany(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(CompanySetup));
            
        }

        private void LoadCharts(object sender, RoutedEventArgs e)
        {
            List<Record> records = new List<Record>();
            records.Add(new Record()
            {
                        Name = "Income", Amount = App.chartData.income
            });
                    records.Add(new Record()
            {
                        Name = "Expense", Amount = App.chartData.expense
            });
                    records.Add(new Record()
            {
                        Name = "IncomeTax", Amount = App.chartData.incometax
            });
                    records.Add(new Record()
            {
                        Name = "ExpenseTax", Amount = App.chartData.expensetax
            });
            (PieDataChart.Series[0] as PieSeries).ItemsSource = null;
        }
    }
}
