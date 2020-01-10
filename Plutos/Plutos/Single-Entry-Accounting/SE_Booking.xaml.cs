using Plutos.ClientCommunication;
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
using Newtonsoft.Json;
using Windows.UI.Popups;

// Die Elementvorlage "Leere Seite" wird unter https://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace Plutos.Single_Entry_Accounting
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class SE_Booking : Page
    {
        public SE_Booking()
        {
            this.InitializeComponent();
            LoadSettings();
        }

        void LoadSettings()
        {
                ExpenseTypeCB.ItemsSource = new string[17] { "Wareneinkauf (Handelswaren, Roh-, Hilfs-, Verbrauchsmaterial)",
                "Personalkosten (Löhne, Gehälter, Lohnnebenkosten, wie gesetzlicher Sozialaufwand, Kommunalsteuer, Dienstgeberbeitrag, U-Bahn-Steuer)",
                "Energiebezug (Strom, Beheizung)", "Pflichtbeiträge zur Sozialversicherung der Gewerblichen Wirtschaft","Abschreibungen(AfA)","Geringwertige Wirtschaftsgüter",
                "Fahrt-und Reisespesen (Inland, Ausland, Tages-und Nächtigungsgelder)", "Gebühren (Umlagen, Gemeinde, Post, Telefon", "Honorare (Anwalt, Notar, Steuerberater, Buchhaltung",
                "Miete, Pacht (Gebäude, Maschinen, Telefon, Leasing", "Kfz-Betriebskosten", "Reparaturen (an Betriebsgebäuden, Maschinen, Betriebsausstattung)",
                "Material (Büro, Reinigung, Verpackung, Dekoration", "Steuern und Abgaben (Grundsteuer, Alkoholabgabe, Werbeabgabe . Ausgenommen Einkommenssteuer)",
                "Versicherungen(betrieblicheSachversicherungen, Pflichtversicherungen","Werbung","Bankzinsen und Geldspesen (bei rein betrieblich genutztemBankkonto" };
            TypeCB.SelectedIndex = 0;
        }

        private void TransactionTypeChanged(object sender, SelectionChangedEventArgs e)
        {
            switch(TypeCB.SelectedIndex)
            {
                case 0:
                    ExpenseTypeCB.Visibility = Visibility.Collapsed;
                    break;
                case 1:
                    ExpenseTypeCB.Visibility = Visibility.Visible;
                    break;
            }
        }

        private async void CreateTransaction(object sender, RoutedEventArgs e)
        {
            Windows.UI.Popups.MessageDialog mD = new Windows.UI.Popups.MessageDialog("Please select an expense type");

            string command = String.Empty;
            int expenseType = -1;
            string date = HelperMethods.GetDate();
            Transaction transaction = null;
            if (TypeCB.SelectedIndex.Equals(1)) // User selected type expense 
            {
                if (ExpenseTypeCB.SelectedItem != null)
                {
                    expenseType = ExpenseTypeCB.SelectedIndex;
                }
                else
                {
                    await mD.ShowAsync();
                    return;
                }
            }
            double tax = 0;
            switch(TaxCB.SelectedIndex)
            {
                case 0:
                    tax = Convert.ToDouble(AmountTB.Text) * 0.1; // 10%
                    break;
                case 1:
                    tax = Convert.ToDouble(AmountTB.Text) * 0.13; // 13%
                    break;
                case 2:
                    tax = Convert.ToDouble(AmountTB.Text) * 0.20; // 20%
                    break;
            }
            transaction = new Transaction(DescrTB.Text, Convert.ToDouble(AmountTB.Text), date, expenseType, tax);
            string jsonObject = JsonConvert.SerializeObject(transaction);
            SocketMethods.ConnectToServer();
            SocketMethods.SendMessage($"8;{jsonObject}");
            SocketMethods.ReceiveMessage();
            

            if (SocketMethods.response.Equals(1))
            {
                MessageDialog md = new MessageDialog("Transaction failed");
                await  md.ShowAsync();
            }
            SocketMethods.client.Shutdown(System.Net.Sockets.SocketShutdown.Both);
            SocketMethods.Disconnect();
        }
    }
}
