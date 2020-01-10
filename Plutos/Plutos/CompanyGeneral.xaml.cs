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

// Die Elementvorlage "Leere Seite" wird unter https://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace Plutos
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class CompanyGeneral : Page
    {
        public CompanyGeneral()
        {
            this.InitializeComponent();
            string[] currencys = new string[4] { "Euro", "Dollar", "Pounds", "Yen" };
            CurrencyBox.ItemsSource = currencys;
        }
        Windows.Storage.Pickers.FileOpenPicker fileOpenPicker = null;
        Windows.Storage.StorageFile file = null;
        private async void PickImage(object sender, RoutedEventArgs e)
        {
             fileOpenPicker = new Windows.Storage.Pickers.FileOpenPicker();


            fileOpenPicker.ViewMode = Windows.Storage.Pickers.PickerViewMode.List;

            // Filter for file types. For example, if you want to open text files,  
            // you will add .txt to the list.  

            fileOpenPicker.FileTypeFilter.Clear();
            fileOpenPicker.FileTypeFilter.Add(".png");
            fileOpenPicker.FileTypeFilter.Add(".jpg");

            file = await fileOpenPicker.PickSingleFileAsync();

            // Process picked file
            if (file == null)
            {
                // The user didn't pick a file
                Windows.UI.Popups.MessageDialog MD = new Windows.UI.Popups.MessageDialog("File not found");
                await MD.ShowAsync();
                return;
            }
        }

        private async void CreateCompany(object sender, RoutedEventArgs e)
        {
            Company company = new Company();
            company.GetData(CurrencyBox.Name, CurrencyBox.SelectedItem.ToString());
            //read the file into stream and set bitmapImage
            IRandomAccessStream stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
            BitmapImage bitmapImage = new BitmapImage();
            await bitmapImage.SetSourceAsync(stream);
            company.image = bitmapImage;
            foreach (var child in MainGrid.Children)
            {
                if (child is TextBox)    // is == Operator for classes
                {
                    TextBox ControlElement = child as TextBox; // Convert child to textbox 
                    if (!company.GetData(ControlElement.Name, ControlElement.Text))
                    {
                        Windows.UI.Popups.MessageDialog messageDialog = new Windows.UI.Popups.MessageDialog("Function failed");
                        await messageDialog.ShowAsync();
                        return;
                    }
                }
            }
            Windows.UI.Popups.MessageDialog messageDialog1 = new Windows.UI.Popups.MessageDialog("Added company sucessfully");
            await messageDialog1.ShowAsync();
            Company.companies.Add(company);
            company = null;
        }
    }
}
