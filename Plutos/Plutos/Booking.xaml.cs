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
using MySql.Data.MySqlClient;
using System.Collections.ObjectModel;
using Plutos.ViewModels;
using Windows.UI.Popups;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

// Die Elementvorlage "Leere Seite" wird unter https://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace Plutos {
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class Booking : Page
    {
        ObservableCollection<HistoryEntry> entrys_;

        public Booking()
        {
            
            this.InitializeComponent();

            LoadSettings();

            //  Income_Account_ComboBox.ItemsSource = Company.selectedCompany.accounts[0].ReturnAllAccountNames(); // ItemSource for the booking account from income and expense 
            // Expenses_Account_ComboBox.ItemsSource = Company.selectedCompany.accounts[0].ReturnAllAccountNames();
        }

        private void LoadSettings()
        {
            Windows.UI.Popups.MessageDialog md = new Windows.UI.Popups.MessageDialog("Could not load settings");

            string connStr = "Server=den1.mysql3.gear.host;Database=plutos;Uid=plutos;Pwd=Ez7L!3P93e_S;SslMode=None";
            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();
            List<string> accountNames = AccountVM.GetNames(conn);
            CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                if (accountNames != null)
                {
                    Income_Account_ComboBox.ItemsSource = accountNames;
                }
                List<string> customerNames = Customer.GetNames(conn);
                if (customerNames != null)
                {
                    customerBox.ItemsSource = customerNames;
                }
                conn.Close();
                incomeTypeCB.ItemsSource = new List<string> { "Sale", "Invoice", "Subscription" };
            });
           
           
        }
        private async void CreateTransaction(object sender, RoutedEventArgs e)
        {
            MessageDialog md = new MessageDialog("");
            double amount;
            bool worked = true;

            if (Income_Account_ComboBox.SelectedItem == null)
            {
                md.Content = "Please select an account";
                await md.ShowAsync();
                return;
            }
            if (String.IsNullOrWhiteSpace(amountTxtBx.Text) || !double.TryParse(amountTxtBx.Text, out amount))
            {
                md.Content = "Wrong or empty value for amount. Please use numbers only";
                await md.ShowAsync();
                return;
            }
            
            Button transBtn = sender as Button; // We need the tag of the button so we can check after if we have a expense, income or an investment
            string connStr = "Server=den1.mysql3.gear.host;Database=plutos;Uid=plutos;Pwd=Ez7L!3P93e_S;SslMode=None";
            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();

            int customerID = GetCustomerID(conn);
            if(customerID == 0)
                return;

            switch (incomeTypeCB.SelectedIndex)
            {
                case 0:
                    worked = NewSale(conn,transBtn.Tag.ToString(),md,amount,customerID);
                    break;
                case 1:
                    worked = NewInvoice(conn,transBtn.Tag.ToString(),md,amount, customerID);
                    break;
                case 2:
                    worked = NewRecurringInvoice(conn,transBtn.Tag.ToString(),md,amount, customerID);
                    break;
            }
            conn.Close();
            if(!worked)
            {
                md.Content = "Could not transfer money";
                await md.ShowAsync();
            }
        }

        private bool NewHistoryEntry(MySqlConnection conn, double amount, int accID, string bookdate, string transactiondate, int customerID)
        {
            MySqlCommand newEntry = new MySqlCommand("INSERT INTO plutos.sales(accID,amount,bookdate, transactiondate, custID) VALUES(@accID,@amount,@bookdate, @transactiondate, @custID)", conn);
            newEntry.Parameters.AddWithValue("@accID", accID);
            newEntry.Parameters.AddWithValue("@amount", amount);
            newEntry.Parameters.AddWithValue("@bookdate", bookdate);
            newEntry.Parameters.AddWithValue("@transactiondate", transactiondate);
            newEntry.Parameters.AddWithValue("@custID", customerID);
            return Convert.ToBoolean(newEntry.ExecuteNonQuery());
        }

        private void LoadHistory(FrameworkElement sender, object args)
        {
            string connStr = "Server=den1.mysql3.gear.host;Database=plutos;Uid=plutos;Pwd=Ez7L!3P93e_S;SslMode=None";
            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();
            entrys_ = HistoryEntry.HistoryEntryList(conn);
            conn.Close();
        }

        private void IncomeTypeChanged(object sender, SelectionChangedEventArgs e)
        {  
            switch ((sender as ComboBox).SelectedItem)
            {
                // the use of  x:DeferLoadStrategy="Lazy" should be considered here
                case "Sale":
                   if(datePicker.Visibility == Visibility.Visible)
                    {
                        datePicker.Visibility = Visibility.Collapsed;
                    }
                   if(paymentPerionCB.Visibility == Visibility.Visible)
                    {
                        paymentPerionCB.Visibility = Visibility.Collapsed;
                    }
                    RelativePanel.SetBelow(amountTxtBx, customerBox);
                    break;
                case "Invoice":
                    datePicker.Header = "Date of Transaction";
                    RelativePanel.SetBelow(datePicker, customerBox);
                    datePicker.Margin = new Thickness(0, 0, 0, 10);
                    RelativePanel.SetBelow(amountTxtBx, datePicker);
                    if(datePicker.Visibility == Visibility.Collapsed)
                    {
                        datePicker.Visibility = Visibility.Visible;
                    }
                    if(paymentPerionCB.Visibility == Visibility.Visible)
                    {
                        paymentPerionCB.Visibility = Visibility.Collapsed;
                    }
                    break;
                case "Subscription":
                    datePicker.Header = "Start of Subscription";
                    RelativePanel.SetBelow(datePicker, customerBox);
                    datePicker.Margin = new Thickness(0, 0, 0, 10);
                    RelativePanel.SetBelow(paymentPerionCB, datePicker);
                    RelativePanel.SetBelow(amountTxtBx, paymentPerionCB);
                    paymentPerionCB.Margin = new Thickness(0, 10, 0, 10);
                    if(datePicker.Visibility == Visibility.Collapsed)
                    {
                        datePicker.Visibility = Visibility.Visible;
                    }
                    if(paymentPerionCB.Visibility == Visibility.Collapsed)
                    {
                        paymentPerionCB.Visibility = Visibility.Visible;
                    }
                    break;
            }
        }

        private async System.Threading.Tasks.Task<bool> LoadRecurringIncomes(MySqlConnection conn)
        {
            string command = $"SELECT amount,transactiondate,custID,account from plutos.sales WHERE transactiondate LIKE '{HelperMethods.GetDate()}' AND compid= {App.compID}";
            string errorMessage = "Could not load recurring incomes";

            List<string> dueIncomes = MySQLCommands.SELECTCOMMALIST(command, errorMessage, conn);
            if (dueIncomes != null)
            {
                foreach (string dueincome in dueIncomes)
                {
                    double amount = Convert.ToDouble(dueincome.Split(';')[0]);
                    DateTime bookDate = DateTime.ParseExact(dueincome.Split(';')[1], "dd.MM.yyyy", null);
                    string customerID = dueincome.Split(';')[2];
                    string accountName = dueincome.Split(';')[3];

                    MySqlCommand selectAccount = new MySqlCommand("SELECT name,amount FROM plutos.accounts WHERE name LIKE '" + accountName, conn);
                    MySqlDataReader readAccount = selectAccount.ExecuteReader();
                    while (readAccount.Read())
                    {
                        amount += Convert.ToDouble(readAccount[1]);
                    }
                    readAccount.Close();

                    // Update amount
                    MySqlCommand updateAmount = new MySqlCommand("UPDATE plutos.accounts SET amount=@amount WHERE name LIKE '" + accountName + "'", conn);
                    updateAmount.Parameters.AddWithValue("@amount", amount);

                    if (updateAmount.ExecuteNonQuery() == 0)
                    {
                        return false;
                    }

                    // Set new date
                    MySqlCommand setNewDate = new MySqlCommand("UPDATE plutos.sales SET transactiondate=@transactiondate WHERE name LIKE '" + accountName + "'", conn);
                    // Calculate difference and get next transaction date
                    int year = DateTime.Now.Year; //Or any year you want
                    DateTime newBookDate = new DateTime(bookDate.Year, bookDate.Month, bookDate.Day).AddDays(30); // Add 30 days so we get our next transaction date
                    string newDate = newBookDate.ToString("dd.MM.yyyy");   // The date in requested format
                    setNewDate.Parameters.AddWithValue("@transactiondate", newDate);

                    if (setNewDate.ExecuteNonQuery() == 0)
                    {
                        return false;
                    }
                    // Create new HistoryEntry
                }
            }
            return true;
        }
        /// <summary>
        /// Replace old amount with new calculated amount
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="amount"></param>
        /// <param name="accID"></param>
        /// <returns></returns>
        private bool SetNewAmount(MySqlConnection conn, double amount, int accID) {
            MySqlCommand updateAmount = new MySqlCommand("UPDATE plutos.accounts SET amount=@amount WHERE accID=" + accID, conn);
            updateAmount.Parameters.AddWithValue("@amount", amount);

            return Convert.ToBoolean(updateAmount.ExecuteNonQuery());
        }
        /// <summary>
        /// Adds new sale to same named table
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="transactionType"></param>
        /// <param name="md"></param>
        /// <param name="amount"></param>
        /// <param name="customerID"></param>
        /// <returns></returns>
        private bool NewSale(MySqlConnection conn, string transactionType,MessageDialog md, double amount, int customerID)
        {
            string command = "SELECT accID,amount FROM plutos.accounts WHERE (compid=" + App.compID + " AND name LIKE '" + Income_Account_ComboBox.SelectedItem.ToString() + "')";
            string errorMessage = "Could not select account data. Please check your connection";

            List<string> values = MySQLCommands.SELECTCOMMALIST(command, errorMessage, conn); // get the name and amount of th accounts
          
            if (transactionBtn.Tag.ToString() == "Income") // Check which transaction we have here
            {
                amount += Convert.ToDouble(values[0].Split(';')[1]); // Add existing account amount to entered amount
            }
            else if (transactionBtn.Tag.ToString() == "Expense")
            {
                amount -= Convert.ToDouble(values[0].Split(';')[1]);
            }
            int accID = Convert.ToInt32(values[0].Split(';')[0]);
            
            if (!SetNewAmount(conn, amount, accID))
            {
                return false;
            }
            
            string bookdate = HelperMethods.GetDate();
            string transactiondate = bookdate;
            string customerName = "No customer";
            if(customerBox.SelectedItem != null)
            {
                customerName = customerBox.SelectedItem.ToString();
            }
            entrys_.Add(new HistoryEntry { customer = customerBox.SelectedItem.ToString(), amount = amount, bookdate = HelperMethods.GetDate(), account = Income_Account_ComboBox.SelectedItem.ToString() });

            return NewHistoryEntry(conn, Convert.ToDouble(amountTxtBx.Text), Convert.ToInt32(values[0].Split(';')[0]), HelperMethods.GetDate(), transactiondate, customerID); // amount,accid,bookdate,transactiondate,customerID   
        }
        /// <summary>
        /// Adds new invoice to invoice table
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="transactionType"></param>
        /// <param name="md"></param>
        /// <param name="amount"></param>
        /// <param name="customerID"></param>
        /// <returns></returns>
        public bool NewInvoice(MySqlConnection conn, string transactionType, MessageDialog md, double amount,int customerID)
        {
            return InsertInvoice(conn, transactionType, md, amount, customerID, true,"plutos.invoices");
        }
        /// <summary>
        /// Adds new recurring invoice to table
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="transactionType"></param>
        /// <param name="md"></param>
        /// <param name="amount"></param>
        /// <param name="customerID"></param>
        /// <returns></returns>
        public bool NewRecurringInvoice(MySqlConnection conn, string transactionType, MessageDialog md, double amount,int customerID)
        {
            return InsertInvoice(conn, transactionType, md, amount, customerID, false,"plutos.recurringinvoices");
        }
        /// <summary>
        /// Creates new invoice and adds it to the invoice or recurring invoice table 
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="transactionType"></param>
        /// <param name="md"></param>
        /// <param name="amount"></param>
        /// <param name="customerID"></param>
        /// <param name="addEntry"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private bool InsertInvoice(MySqlConnection conn, string transactionType, MessageDialog md, double amount, int customerID, bool addEntry, string tableName)
        {
            string command = "SELECT accID,amount FROM plutos.accounts WHERE (compid=" + App.compID + " AND name LIKE '" + Income_Account_ComboBox.SelectedItem.ToString() + "')";
            string errorMessage = "Could not select account data. Please check your connection";
            string transactiondate = datePicker.Date.ToString("dd.MM.yyyy");

            List<string> accountdata = MySQLCommands.SELECTCOMMALIST(command, errorMessage, conn);

            command = "INSERT INTO " + tableName + " (accid,amount,bookdate,transactiondate,custID) VALUES(@accid,@amount,@bookdate,@transactiondate,@custID)";
            MySqlCommand newInvoice = new MySqlCommand(command, conn);
            newInvoice.Parameters.AddWithValue("@accid", accountdata[0].Split(';')[0]);
            newInvoice.Parameters.AddWithValue("@amount", amount);
            newInvoice.Parameters.AddWithValue("@bookdate", HelperMethods.GetDate());
            newInvoice.Parameters.AddWithValue("@transactiondate", transactiondate);
            newInvoice.Parameters.AddWithValue("@custID", customerID);

            if (newInvoice.ExecuteNonQuery() == 0)
            {
                return false;
            }
            if (addEntry)
            {
                return NewHistoryEntry(conn, Convert.ToDouble(accountdata[0].Split(';')[1]), Convert.ToInt32(accountdata[0].Split(';')[0]), HelperMethods.GetDate(), transactiondate, customerID); // amount,accid,bookdate,transactiondate,customerID   
            }
            return true;
        }
        /// <summary>
        /// Returns customer id by searching for the selected customer name
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        private int GetCustomerID(MySqlConnection conn)
        {
            string firstName = customerBox.SelectedItem.ToString().Split(' ')[0];
            string lastName = customerBox.SelectedItem.ToString().Split(' ')[1];
            string command = "SELECT custID from plutos.customers WHERE firstName LIKE '" + firstName + "' AND lastName LIKE '" + lastName + "'";
            string errorMessage = "Could not find customer";
            return Convert.ToInt32(MySQLCommands.SELECTSINGLELIST(command,errorMessage,conn)[0]);
        }

    }
}