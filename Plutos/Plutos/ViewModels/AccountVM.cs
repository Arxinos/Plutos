using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MySql.Data.MySqlClient;
using Plutos.Models;
using Windows.UI.Popups;
using System.Data;
using Windows.UI.Xaml.Controls;
using Plutos.Commands;
using Newtonsoft.Json;
using Plutos.ClientCommunication;
using System.Threading;
using Windows.UI.Xaml;
using System.Net.Sockets;

namespace Plutos.ViewModels
{
    class AccountVM : INotifyPropertyChanged
    {
        DispatcherTimer dispatcherTimer = new DispatcherTimer();
        public AccountVM()
        {
            NewAccountCommand = new CommandAsync(NewAccount);
            DeleteAccountCommand = new CommandAsync(DeleteAccount);
            dispatcherTimer.Tick += Refresh;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
        }

        #region Global elements
        MessageDialog md = new MessageDialog("");
        public event PropertyChangedEventHandler PropertyChanged;
        MySqlConnection conn = new MySqlConnection("Server=den1.mysql3.gear.host;Database=plutos;Uid=plutos;Pwd=Ez7L!3P93e_S;SslMode=None");
        #endregion

        #region Properties
        private ObservableCollection<Account> accounts;
        public ObservableCollection<Account> Accounts {
            get { return accounts; }
            set 
            {
                accounts = value;
                OnPropertyChanged("Accounts");
            }
        }

        private string newAccountName;
        public string NewAccountName {
            get { return newAccountName; }
            set 
            {
                if (this.newAccountName != value)
                {
                    newAccountName = value;
                    OnPropertyChanged("NewAccountName");
                }
            }
        }

        private string newAccountDescription;
        public string NewAccountDescription 
        {
            get { return newAccountDescription; }
            set {
                if (this.newAccountDescription != value)
                {
                    newAccountDescription = value;
                    OnPropertyChanged("NewAccountDescription");
                }
            }
        }

        private string newAccountAmount;
        public string NewAccountAmount {
            get { return newAccountAmount; }
            set {
                if (this.newAccountAmount != value)
                {
                    newAccountAmount = value;
                    OnPropertyChanged("NewAccountAmount");
                }
            }
        }

        private Account selectedAccount;
        public Account SelectedAccount {
            get { return selectedAccount; }
            set 
            {
                if (selectedAccount != value)
                {
                    selectedAccount = value;
                }
                OnPropertyChanged("SelectedAccount");
            }
        }


        public CommandAsync DeleteAccountCommand { get; set; }
        public CommandAsync NewAccountCommand { get; set; }
        #endregion

        #region Methods
        private void Refresh(object state, object e)
        {
            if (SocketMethods.data.Accounts != null)
            {
                Accounts = SocketMethods.data.Accounts;
                SocketMethods.data.Accounts = null;
            }
            else
            {
                return;
            }
        }

        ///<summary>
        ///Loads all of the companies accounts
        ///</summary>
        public void LoadAccounts()
        {
            Accounts = SocketMethods.data.Accounts;
        }

        /// <summary>
        /// Adds a new Account to the database
        /// </summary>
        private async Task NewAccount()
        {
            Account newAccount = new Account(){Name = NewAccountName, Description = NewAccountDescription, Amount = Convert.ToDouble(NewAccountAmount.Replace(',','.')) };

            Task.Run(() => SocketMethods.CreateAccount(newAccount));
        }

        /// <summary>
        /// Checks if the user input was in a correct format
        /// </summary>
        /// <returns></returns>
        public async Task<bool> CheckTextBoxEntrys()
        {
            string command = "SELECT name FROM plutos.accounts WHERE compid=" + App.compID;     // selects the names of all of the accounts in the database with the current company ID
            string errorMessage = "Could not select accounts to check for name equivalence";
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            List<string> existingNames = MySQLCommands.SELECTSINGLELIST(command, errorMessage, conn);
            if (conn.State != ConnectionState.Open)
            {
                conn.Close();
            }
            if (String.IsNullOrWhiteSpace(NewAccountName))      // Checks if the Name TextBox is empty
            {
                md.Content = "Your Account has to have a name";
                await md.ShowAsync();
                return false;
            }

            foreach(Account acc in accounts)       //Checks if an account with the same name already exists. ToUpper() converts the text to uppercase. Example "bank" and "Bank" will not be able to coexsit
            {
               if(acc.Name.ToUpper().Equals(newAccountName.ToUpper()))
                {
                    md.Content = "This Account already Exists!";
                    await md.ShowAsync();
                    return false;
                }
            }

            float placeholder = 0.0f;
            if (String.IsNullOrWhiteSpace(NewAccountAmount) || !float.TryParse(NewAccountAmount, out placeholder))      // Checks if the Amount TextBox is empty or non numeric
            {
                md.Content = "Some Amount TextBox is empty or contains non numeric characters";
                await md.ShowAsync();
                return false;
            }

            if (String.IsNullOrWhiteSpace(NewAccountDescription))       // The description is optional so if empty we add a '-' to the database.
            {
                NewAccountDescription = "-";
            }

            return true;
        }

        public async Task DeleteAccount()
        {
            if(SelectedAccount == null)
            {
                md.Content = "No account selected";
                await md.ShowAsync();
                return;
            }
            SocketMethods.client = new Socket(SocketMethods.ipAddress.AddressFamily,
            SocketType.Stream, ProtocolType.Tcp);
            SocketMethods.ConnectToServer();
            SocketMethods.SendMessage($"6;{SelectedAccount.AccID}");
            SocketMethods.ReceiveMessage();
            SocketMethods.client.Shutdown(System.Net.Sockets.SocketShutdown.Both);
            SocketMethods.Disconnect();
            accounts.Remove(selectedAccount);
        }

        public void CreateBasicAccounts(int compid) 
        {
            string[] accountNames = new string[4] {"Bank", "Accounts Receivable", "Accounts Payable", "Loans"};
            string[] accountDescriptions = new string[4] { "Your Bank Account", "What your Customers still have to pay you", "What you still owe", "The Loans you still have to pay back" };

            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            for (int i = 0; i < 4; i++)
            {

                MySqlCommand addAccounts = new MySqlCommand("INSERT INTO accounts(compid, name, description, amount) VALUES(@compid, @name, @description, 0)", conn);  // inserts a new entry to the accounts table in the database
                addAccounts.Parameters.AddWithValue("@compid", compid);
                addAccounts.Parameters.AddWithValue("@name", accountNames[i]);
                addAccounts.Parameters.AddWithValue("@description", accountDescriptions[i]);
                addAccounts.ExecuteNonQuery();
            }
            if (conn.State != ConnectionState.Open)
            {
                conn.Close();
            }
        }

        /// <summary>
        /// Function needed for the Booking page. Gets the name of all of the accounts with the current company ID
        /// </summary>
        public static List<string> GetNames(MySqlConnection conn)
        {
            string command = "SELECT name from plutos.accounts WHERE compid=" + App.compID;
            string errorMessage = "Could not load accounts";
            return MySQLCommands.SELECTSINGLELIST(command, errorMessage, conn);
        }

        /// <summary>
        /// Implementing the INotifyPropertyChanged Interface.
        /// Notifies the View of any changes to the data and updates it. 
        /// </summary>
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
