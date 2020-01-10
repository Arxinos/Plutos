using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

// Die Elementvorlage "Leere Seite" wird unter https://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace Plutos
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class Register : Page
    {
        List<string> securityQuestions = new List<string>();
        public Register()
        {
            this.InitializeComponent();
            securityQuestions.Add("What is the first name of the person you first kissed?");
            securityQuestions.Add("What is the name of the place your wedding reception was held?");
            securityQuestions.Add("What is the last name of the teacher who gave you your first failing grade?");
            securityQuestions.Add("Where were you when you had your first alcoholic drink (or cigarette)?");
            securityQuestions.Add("Who was your childhood hero?");
            securityQuestions.Add("What is the first name of the person who has the middle name of Herbert?");
        }
        /// <summary>
        /// Adds user to the users table on button click
        /// </summary>
        private async void RegisterUser(object sender, RoutedEventArgs e)
        {
            Windows.UI.Popups.MessageDialog md = new Windows.UI.Popups.MessageDialog("");
            if(String.IsNullOrEmpty(EmailTB.Text))
            {
                md.Content = "No Email entered";
                await md.ShowAsync();
                return;
            }
            if (String.IsNullOrEmpty(PWDB.Password))
            {
                md.Content = "No password entered";
                await md.ShowAsync();
                return;
            }
            if (String.IsNullOrEmpty(ConfirmPWDB.Password))
            {
                md.Content = "Passwords needs to be confirmed";
                await md.ShowAsync();
                return;
            }

            if (PWDB.Password != ConfirmPWDB.Password) // if the passwords match the entered password is passed to the sql parameter. If they don't, an error pops up and function "RegisterUser" is closed.
            {
                md.Content = "Passwords don't match!";
                await md.ShowAsync();
                return;
            }

            string connStr = "Server=den1.mysql3.gear.host;Database=plutos;Uid=plutos;Pwd=Ez7L!3P93e_S;SslMode=None"; 
            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();
            if(await CheckExistingEmails(conn,md))
            {
                return;
            }
            MySqlCommand addUser = new MySqlCommand("INSERT INTO users(email, password,sq,sa) VALUES(@email, AES_ENCRYPT(@password, @email), @SQ, AES_ENCRYPT(@answer, @SQ))", conn); // adds email and password from the boxes to the table
            addUser.Parameters.AddWithValue("@email", EmailTB.Text);          
            addUser.Parameters.AddWithValue("@password", PWDB.Password);
            addUser.Parameters.AddWithValue("@SQ", securityQuestionCB.SelectedIndex);
            addUser.Parameters.AddWithValue("@answer", SecQuesTB.Text);

            if (addUser.ExecuteNonQuery() == 0) // Check if command was successful
            {
                md.Content = "Something went wrong";
            }
            else {
                md.Content = "You have been registered";
            }
            conn.Close();
            await md.ShowAsync();
        }
        /// <summary>
        /// Checks if entered email has already been registered in database
        /// </summary>
        private async System.Threading.Tasks.Task<bool> CheckExistingEmails(MySqlConnection conn, Windows.UI.Popups.MessageDialog md)
        {
            string command = "SELECT email from plutos.users WHERE email LIKE '" + EmailTB.Text +"'";
            string errorMessage = "Could not check if email has already been registered";
            List<string> eMails = MySQLCommands.SELECTSINGLELIST(command,errorMessage,conn);
            foreach(string eMail in eMails)
            {
                if(eMail == EmailTB.Text)
                {
                    md.Content = "Email already exists";
                    await md.ShowAsync();
                    return true;
                }
            }
            return false;
        }

    }
}
