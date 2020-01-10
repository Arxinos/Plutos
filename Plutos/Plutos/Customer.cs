using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using MySql.Data.MySqlClient;

namespace Plutos
{
    public class Customer
    {
        public Image custImage;
        public string firstName;
        public string lastName;
        
        public Customer(string firstName, string lastName)
        {
            this.firstName = firstName;
            this.lastName = lastName;
        }

        public static ObservableCollection<Customer> CustomerList(MySqlConnection conn)
        {
            string command = "SELECT firstName,lastName from plutos.customers WHERE compid=" + App.compID;
            string errorMessage = "Could not load customers";
            List<string> customerNames = MySQLCommands.SELECTCOMMALIST(command, errorMessage, conn);
            Customer customer = null;
            ObservableCollection<Customer> customers = new ObservableCollection<Customer>(); // We need a ObservableCollection of Customers because we you can not bind a list

            if (customerNames != null) // Check if we have customers
            {
                foreach (string value in customerNames)
                {
                    customer = new Customer(value.Split(';')[0], value.Split(';')[1]); // Set firstName and lastName
                    customers.Add(customer);
                }
            }
            return customers;
        }
        
        public static List<string> GetNames(MySqlConnection conn)
        {
            string command = "SELECT firstName,lastName from plutos.customers WHERE compid=" + App.compID;
            string errorMessage = "Could not load customers";
            List<string> customerNames =  MySQLCommands.SELECTCOMMALIST(command, errorMessage,conn);
            List<string> fullNames = new List<string>();

            if(customerNames != null)
            {
                foreach(string value in customerNames)
                {
                    fullNames.Add(value.Split(';')[0] + " " + value.Split(';')[1]);
                }
            }
            return fullNames;
        } 
    }
}
