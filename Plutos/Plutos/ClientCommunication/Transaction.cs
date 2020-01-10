using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plutos.ClientCommunication
{
    class Transaction
    {
        public string date
        {
            get; set;
        }

        public double amount
        {
            get; set;
        }

        public string description
        {
            get; set;
        }

        public int expenseType
        {
            get; set;
        }
        public double tax
        {
            get;set;
        }
        public Transaction(string description, double amount, string date, int expenseType, double tax)
        {
            this.description = description;
            this.amount = amount;
            this.date = date;
            this.expenseType = expenseType;
            this.tax = tax;
        }

        public Transaction(string description, double amount, string date)
        {
            this.description = description;
            this.amount = amount;
            this.date = date;
        }
    }
}
