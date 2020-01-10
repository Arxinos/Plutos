using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using Plutos.Models;

namespace Plutos.ClientCommunication
{
    class Data
    {
        public ObservableCollection<Company> Companies { get; set; }
        public ObservableCollection<Account> Accounts { get; set; }
    }
}
