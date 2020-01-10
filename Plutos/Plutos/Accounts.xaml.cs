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
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Collections.ObjectModel;
using Windows.UI.Popups;
using Plutos.Models;
using Plutos.ViewModels;
// Die Elementvorlage "Leere Seite" wird unter https://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace Plutos
{
    /// <summary>
    /// User Can Add new or Display already existing accounts
    /// </summary>
    public sealed partial class Accounts : Page
    {
        public Accounts()
        {
            this.InitializeComponent();
            this.DataContext = new AccountVM();
        }

        List<TextBox> nameTxtBx = new List<TextBox>();
        List<TextBox> descriptionTxtBx = new List<TextBox>();
        List<TextBox> amountTxtBx = new List<TextBox>();
    }
}
