using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;

namespace Plutos
{
    class MySQLCommands
    {
        /// <summary>
        /// Returns a list of objects from a table
        /// </summary>
        /// <param name="command"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public static List<string> SELECTCOMMALIST(string command, string errorMessage, MySqlConnection conn)
        {
            List<string> returnList = new List<string>();
            MySqlCommand selectProperties = new MySqlCommand(command, conn);
            MySqlDataReader reader = selectProperties.ExecuteReader();
            string value = String.Empty;
            try
            {
                while (reader.Read())
                {
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        value += reader[i] + ";";
                    }
                    returnList.Add(value);
                    value = String.Empty;
                }
            }
            catch (MySqlException)
            {
                Windows.UI.Popups.MessageDialog md = new Windows.UI.Popups.MessageDialog(errorMessage);
                md.ShowAsync();
                return null;
            }
            finally
            {
                reader.Close();
            }
            
            return returnList;
        }
        public static List<string> SELECTSINGLELIST(string command, string errorMessage, MySqlConnection conn)
        {
            List<string> returnList = new List<string>();
            MySqlCommand selectProperties = new MySqlCommand(command, conn);
            MySqlDataReader reader = selectProperties.ExecuteReader();
            try
            {
                while (reader.Read())
                {
                    returnList.Add(reader[0].ToString());
                }
            }
            catch (MySqlException)
            {
                Windows.UI.Popups.MessageDialog md = new Windows.UI.Popups.MessageDialog(errorMessage);
                md.ShowAsync();
                return null;
            }
            finally
            {
                reader.Close();
            }
            return returnList;
        }
    }
}
