using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using MySql.Data.MySqlClient;

namespace Plutos
{
    class HistoryEntry
    {
        
        
        public string bookdate
        {
            get;set;
        }

        public string transactiondate 
        {
            get;set;
        }

        public string customer {
            get;set;
        }

        public double amount
        {
            get;set;
        }

        public string account
        {
            get; set;
        }

        public static ObservableCollection<HistoryEntry> HistoryEntryList(MySqlConnection conn)
        {
            string command = "SELECT sales.amount, sales.bookdate, sales.transactiondate, customers.firstName,customers.lastName, accounts.name from plutos.sales " +
                "Inner Join plutos.accounts ON sales.accid = accounts.accid " +
                "Inner Join plutos.customers ON sales.custid = customers.custid " +
                "WHERE accounts.compid=" + App.compID;
            string errorMessage = "Could not load HistoryEntrys";
            List<string> HistoryEntryNames = MySQLCommands.SELECTCOMMALIST(command, errorMessage,conn);
            ObservableCollection<HistoryEntry> HistoryEntrys = new ObservableCollection<HistoryEntry>(); // We need a ObservableCollection of HistoryEntrys because we you can not bind a list

            if (HistoryEntryNames != null) // Check if we have HistoryEntrys
            {
                foreach (string value in HistoryEntryNames)
                {
                    HistoryEntrys.Add(new HistoryEntry { amount = Convert.ToDouble(value.Split(';')[0]), bookdate = value.Split(';')[1], transactiondate = value.Split(';')[2], customer = value.Split(';')[3] + " " + value.Split(';')[4], account = value.Split(';')[5]});// Set firstName and lastName
                }
            }
            return HistoryEntrys;
        }
    }
}
