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
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using System.Collections.ObjectModel;

using MySql.Data.MySqlClient;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
// Die Elementvorlage "Leere Seite" wird unter https://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace Plutos {
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    
    public sealed partial class CustomersClass : Page {

        private ObservableCollection<Customer> customers_;
        

        public CustomersClass() {
            this.InitializeComponent();
            string connStr = "Server=den1.mysql3.gear.host;Database=plutos;Uid=plutos;Pwd=Ez7L!3P93e_S;SslMode=None";
            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();

            customers_ = Customer.CustomerList(conn);
            this.DataContext = customers_;
            conn.Close();
        }

        public string[] countries = new string[] {
            "Afghanistan",
            "Albania",
            "Algeria",
            "American Samoa",
            "Andorra",
            "Angola",
            "Anguilla",
            "Antarctica",
            "Antigua and Barbuda",
            "Argentina",
            "Armenia",
            "Aruba",
            "Australia",
            "Austria",
            "Azerbaijan",
            "Bahamas",
            "Bahrain",
            "Bangladesh",
            "Barbados",
            "Belarus",
            "Belgium",
            "Belize",
            "Benin",
            "Bermuda",
            "Bhutan",
            "Bolivia",
            "Bosnia and Herzegovina",
            "Botswana",
            "Bouvet Island",
            "Brazil",
            "British Indian Ocean Territory",
            "Brunei Darussalam",
            "Bulgaria",
            "Burkina Faso",
            "Burundi",
            "Cambodia",
            "Cameroon",
            "Canada",
            "Cape Verde",
            "Cayman Islands",
            "Central African Republic",
            "Chad",
            "Chile",
            "China",
            "Christmas Island",
            "Cocos (Keeling) Islands",
            "Colombia",
            "Comoros",
            "Congo",
            "Congo, the Democratic Republic of the",
            "Cook Islands",
            "Costa Rica",
            "Cote D'Ivoire",
            "Croatia",
            "Cuba",
            "Cyprus",
            "Czech Republic",
            "Denmark",
            "Djibouti",
            "Dominica",
            "Dominican Republic",
            "Ecuador",
            "Egypt",
            "El Salvador",
            "Equatorial Guinea",
            "Eritrea",
            "Estonia",
            "Ethiopia",
            "Falkland Islands (Malvinas)",
            "Faroe Islands",
            "Fiji",
            "Finland",
            "France",
            "French Guiana",
            "French Polynesia",
            "French Southern Territories",
            "Gabon",
            "Gambia",
            "Georgia",
            "Germany",
            "Ghana",
            "Gibraltar",
            "Greece",
            "Greenland",
            "Grenada",
            "Guadeloupe",
            "Guam",
            "Guatemala",
            "Guinea",
            "Guinea-Bissau",
            "Guyana",
            "Haiti",
            "Heard Island and Mcdonald Islands",
            "Holy See (Vatican City State)",
            "Honduras",
            "Hong Kong",
            "Hungary",
            "Iceland",
            "India",
            "Indonesia",
            "Iran, Islamic Republic of",
            "Iraq",
            "Ireland",
            "Israel",
            "Italy",
            "Jamaica",
            "Japan",
            "Jordan",
            "Kazakhstan",
            "Kenya",
            "Kiribati",
            "Korea, Democratic People's Republic of",
            "Korea, Republic of",
            "Kuwait",
            "Kyrgyzstan",
            "Lao People's Democratic Republic",
            "Latvia",
            "Lebanon",
            "Lesotho",
            "Liberia",
            "Libyan Arab Jamahiriya",
            "Liechtenstein",
            "Lithuania",
            "Luxembourg",
            "Macao",
            "Macedonia, the Former Yugoslav Republic of",
            "Madagascar",
            "Malawi",
            "Malaysia",
            "Maldives",
            "Mali",
            "Malta",
            "Marshall Islands",
            "Martinique",
            "Mauritania",
            "Mauritius",
            "Mayotte",
            "Mexico",
            "Micronesia, Federated States of",
            "Moldova, Republic of",
            "Monaco",
            "Mongolia",
            "Montserrat",
            "Morocco",
            "Mozambique",
            "Myanmar",
            "Namibia",
            "Nauru",
            "Nepal",
            "Netherlands",
            "Netherlands Antilles",
            "New Caledonia",
            "New Zealand",
            "Nicaragua",
            "Niger",
            "Nigeria",
            "Niue",
            "Norfolk Island",
            "Northern Mariana Islands",
            "Norway",
            "Oman",
            "Pakistan",
            "Palau",
            "Palestinian Territory, Occupied",
            "Panama",
            "Papua New Guinea",
            "Paraguay",
            "Peru",
            "Philippines",
            "Pitcairn",
            "Poland",
            "Portugal",
            "Puerto Rico",
            "Qatar",
            "Reunion",
            "Romania",
            "Russian Federation",
            "Rwanda",
            "Saint Helena",
            "Saint Kitts and Nevis",
            "Saint Lucia",
            "Saint Pierre and Miquelon",
            "Saint Vincent and the Grenadines",
            "Samoa",
            "San Marino",
            "Sao Tome and Principe",
            "Saudi Arabia",
            "Senegal",
            "Serbia and Montenegro",
            "Seychelles",
            "Sierra Leone",
            "Singapore",
            "Slovakia",
            "Slovenia",
            "Solomon Islands",
            "Somalia",
            "South Africa",
            "South Georgia and the South Sandwich Islands",
            "Spain",
            "Sri Lanka",
            "Sudan",
            "Suriname",
            "Svalbard and Jan Mayen",
            "Swaziland",
            "Sweden",
            "Switzerland",
            "Syrian Arab Republic",
            "Taiwan, Province of China",
            "Tajikistan",
            "Tanzania, United Republic of",
            "Thailand",
            "Timor-Leste",
            "Togo",
            "Tokelau",
            "Tonga",
            "Trinidad and Tobago",
            "Tunisia",
            "Turkey",
            "Turkmenistan",
            "Turks and Caicos Islands",
            "Tuvalu",
            "Uganda",
            "Ukraine",
            "United Arab Emirates",
            "United Kingdom",
            "United States",
            "United States Minor Outlying Islands",
            "Uruguay",
            "Uzbekistan",
            "Vanuatu",
            "Venezuela",
            "Viet Nam",
            "Virgin Islands, British",
            "Virgin Islands, US",
            "Wallis and Futuna",
            "Western Sahara",
            "Yemen",
            "Zambia",
            "Zimbabwe",
        };
        Windows.Storage.Pickers.FileOpenPicker fileOpenPicker = null;
        Windows.Storage.StorageFile file = null;

        private async void pickCustomerImage(object sender, RoutedEventArgs e) {
            fileOpenPicker = new Windows.Storage.Pickers.FileOpenPicker();


            fileOpenPicker.ViewMode = Windows.Storage.Pickers.PickerViewMode.List;

            // Filter for file types. For example, if you want to open text files,  
            // you will add .txt to the list.  

            fileOpenPicker.FileTypeFilter.Clear();
            fileOpenPicker.FileTypeFilter.Add(".png");
            fileOpenPicker.FileTypeFilter.Add(".jpg");

            file = await fileOpenPicker.PickSingleFileAsync();
            // Process picked file
            if (file == null) {
                // The user didn't pick a file
                Windows.UI.Popups.MessageDialog MD = new Windows.UI.Popups.MessageDialog("File not found");
                await MD.ShowAsync();
                return;
            }
            IRandomAccessStream stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
            BitmapImage bitmapImage = new BitmapImage();
            await bitmapImage.SetSourceAsync(stream);
            customerPicture.Source = bitmapImage;
        }

        private async void CreateCustomer(object sender, RoutedEventArgs e)
        {
                 Windows.UI.Popups.MessageDialog md = new Windows.UI.Popups.MessageDialog("");
                if (String.IsNullOrWhiteSpace(firstNameTextBox.Text)) // Check for firstname
                {
                    md.Content = "First name is required";
                    await md.ShowAsync();
                    return;
                }
                if (String.IsNullOrWhiteSpace(lastNameTextBox.Text)) // Check for lastname
                {
                    md.Content = "Last name is required";
                    await md.ShowAsync();
                    return;
                }
                string selectedCountry = "No country";

                if (countryComboBox.SelectedItem != null)
                {
                    selectedCountry = countryComboBox.SelectedItem.ToString();
                }

                List<TextBox> propertTextBoxes = new List<TextBox>() { firstNameTextBox, lastNameTextBox, emailTextBox, phoneTextBox, streetTextBox, cityTextBox, streetTextBox, postalCodeTextBox };
                foreach (TextBox textbox in propertTextBoxes)
                {
                    if (String.IsNullOrWhiteSpace(textbox.Text))
                    {
                        textbox.Text = "";
                    }
                }
                string connStr = "Server=den1.mysql3.gear.host;Database=plutos;Uid=plutos;Pwd=Ez7L!3P93e_S;SslMode=None;Allow User Variables=true";
                MySqlConnection conn = new MySqlConnection(connStr);
                conn.Open();
                MySqlCommand addCustomer = new MySqlCommand("INSERT INTO customers(compid,firstname,lastname,day,month,year,email," +
                    "phonenumber,country,city,street,postalcode) " +
                    "VALUES(@compid,@firstname,@lastname,@day,@month,@year,@email,@phonenumber,@country,@city,@street,@postalcode)", conn);

                addCustomer.Parameters.AddWithValue("@compid", App.compID);
                addCustomer.Parameters.AddWithValue("@firstname", firstNameTextBox.Text);
                addCustomer.Parameters.AddWithValue("@lastname", lastNameTextBox.Text);
                addCustomer.Parameters.AddWithValue("@day", birthdate.Date.Day.ToString());
                addCustomer.Parameters.AddWithValue("@month", birthdate.Date.Month.ToString());
                addCustomer.Parameters.AddWithValue("@year", birthdate.Date.Year.ToString());
                addCustomer.Parameters.AddWithValue("@email", emailTextBox.Text);
                addCustomer.Parameters.AddWithValue("@phonenumber", phoneTextBox.Text);
                addCustomer.Parameters.AddWithValue("@country", selectedCountry);
                addCustomer.Parameters.AddWithValue("@city", cityTextBox.Text);
                addCustomer.Parameters.AddWithValue("@street", streetTextBox.Text);
                addCustomer.Parameters.AddWithValue("@postalcode", postalCodeTextBox.Text);


                int checkCommand = addCustomer.ExecuteNonQuery();                               //ExecuteNonQuery returns the amount of rows affected by the command.
                if (checkCommand == 0)
                {
                    md = new Windows.UI.Popups.MessageDialog("Could not create customer");
                    await md.ShowAsync();
                }
                else
                {
                    md = new Windows.UI.Popups.MessageDialog("Customer has been created");
                    await md.ShowAsync();
                }
                conn.Close();
        }

        private async void DeleteCustomer(object sender, RoutedEventArgs e)
        {
            foreach (Customer customer in MainGridView.Items)
            { 
                    Windows.UI.Popups.MessageDialog md = new Windows.UI.Popups.MessageDialog(customer.firstName + customer.lastName + customer.ToString());
                    await md.ShowAsync();
            }
        }

        private async void MarkObject(object sender, TappedRoutedEventArgs e)
        {
            Windows.UI.Popups.MessageDialog md = new Windows.UI.Popups.MessageDialog(sender.ToString());
            await md.ShowAsync();
        }
    }
}
