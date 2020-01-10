using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Plutos.ClientCommunication;
using Plutos.Models;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Popups;

namespace Plutos
{
    public class StateObject
    {
        // Client socket.  
        public Socket workSocket = null;
        // Size of receive buffer.  
        public const int BufferSize = 256;
        // Receive buffer.  
        public byte[] buffer = new byte[BufferSize];
        // Received data string.  
        public StringBuilder sb = new StringBuilder();
    }

    class SocketMethods
    {
        #region variables
        private const int port = 47001;                                 //The port we are going to be listening to
        public static String response = String.Empty;                  //What the server sends back          
        static RSAManager rsaManager = new RSAManager();                //An instance of our own RSAManager class
        public static AESManager aesManager = new AESManager();
        public static Data data = new Data();
        #endregion

        #region ResetEvents  
        private static ManualResetEvent connectDone =                   //A ManualResetEvents can stop and start threads.
            new ManualResetEvent(false);                                //We use them to halt the main thread until the async method has started.
        private static ManualResetEvent sendDone =
            new ManualResetEvent(false);
        private static ManualResetEvent receiveDone =
            new ManualResetEvent(false);
        private static ManualResetEvent disconnectDone =
            new ManualResetEvent(false);
        #endregion

        #region Socket
        static IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());               //Resolves the Hostname or IP of the server we want to connect to. 
        public static IPAddress ipAddress = ipHostInfo.AddressList[0];                             //Selects the first of the resolved adresses. [0] = IPv6, [1] = IPv4
        static IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);                       //creating an endpoint that points to our server. combination of ip and port. e.g 192.68.0.0:80
        public static Socket client = new Socket(ipAddress.AddressFamily,                          //Creating a new Socket and setting it's parameters. Static since we only have one connection at a time.
            SocketType.Stream, ProtocolType.Tcp);
        #endregion

        public static async Task<bool> StartClient(string eMail, string password)
        {
            
            GetRSAKey();
            MessageDialog md = new MessageDialog("Please check your input data and try again");
            if (Login(eMail, password))
            {
                GetData();
                return true;
            }
            else
            {
                await md.ShowAsync();
                return false;
            }
        }

        public static void GetRSAKey()
        {
            ConnectToServer();
            SendMessage("0");
            ReceiveMessage();
            rsaManager.SetPublicKey(response);
            client.Shutdown(SocketShutdown.Both);
            Disconnect();
        }

        public static void SendPackage(object state, object e)
        {
            client = new Socket(ipAddress.AddressFamily,                          //Creating a new Socket and setting it's parameters. Static since we only have one connection at a time.
            SocketType.Stream, ProtocolType.Tcp);
            ConnectToServer();
            SendMessage("2");
            Disconnect();
        }

        static bool Login(string eMail, string password)
        {
            client = new Socket(ipAddress.AddressFamily,
            SocketType.Stream, ProtocolType.Tcp);
            ConnectToServer();
            LoginData loginData = new LoginData()
            {
                eMail = eMail,
                password = password,
            };
            Keys keys = new Keys(HelperMethods.GetRandomString(16), HelperMethods.GetRandomString(16));
            aesManager.aesKey = keys.aesKey;
            aesManager.IV = keys.IV;
            string aesKeys = JsonConvert.SerializeObject(keys);
            string loginString = JsonConvert.SerializeObject(loginData);
            SendMessage("1;" + Convert.ToBase64String(rsaManager.Encrypt(loginString + ";" + aesKeys)));
            ReceiveMessage();
            if(response == "1")
            {
                return false;
            }
            client.Shutdown(SocketShutdown.Both);
            Disconnect();
            return true;
        }

        public static void GetData()
        {
            client = new Socket(ipAddress.AddressFamily,
            SocketType.Stream, ProtocolType.Tcp);
            ConnectToServer();
            SendMessage("3;" + App.compID.ToString());
            ReceiveMessage();
            string decryptedResponse = aesManager.Decrypt(Convert.FromBase64String(response));
            data.Companies = JsonConvert.DeserializeObject<ObservableCollection<Company>>(decryptedResponse.Split(";")[0]);
            data.Accounts = JsonConvert.DeserializeObject<ObservableCollection<Account>>(decryptedResponse.Split(";")[1]);
            App.chartData = JsonConvert.DeserializeObject<ChartData>(decryptedResponse.Split(";")[2]);
            Disconnect();
           
        }

        public static void ChangeCompany()
        {
            client = new Socket(ipAddress.AddressFamily,
            SocketType.Stream, ProtocolType.Tcp);
            ConnectToServer();
            SendMessage("4;" + App.compID.ToString());
            ReceiveMessage();
            if (response != "1")
            {
                string decryptedResponse = aesManager.Decrypt(Convert.FromBase64String(response));
                data.Accounts = JsonConvert.DeserializeObject<ObservableCollection<Account>>(decryptedResponse);
            }
            client.Shutdown(SocketShutdown.Both);
            Disconnect();
        }

        public static void CreateAccount(Account newAccount)
        {
            client = new Socket(ipAddress.AddressFamily,
            SocketType.Stream, ProtocolType.Tcp);
            ConnectToServer();
            string encryptedjson = Convert.ToBase64String(aesManager.Encrypt(JsonConvert.SerializeObject(newAccount) + ";" + App.compID.ToString()));
            SendMessage("5;" + encryptedjson);
            ReceiveMessage();
            if (response != "1")
            {
                string decryptedResponse = aesManager.Decrypt(Convert.FromBase64String(response));
                data.Accounts = JsonConvert.DeserializeObject<ObservableCollection<Account>>(decryptedResponse);
            }
            Disconnect();
        }

        #region ConnectAndDisconnect
        static public void ConnectToServer()
        {
            client = new Socket(ipAddress.AddressFamily,                          //Creating a new Socket and setting it's parameters. Static since we only have one connection at a time.
            SocketType.Stream, ProtocolType.Tcp);
            connectDone.Reset();
            try
            {
                client.BeginConnect(remoteEP,
                       new AsyncCallback(ConnectCallback), client);
                connectDone.WaitOne();
            }
            catch (SocketException ex)
            {
                Debug.WriteLine("Error: ", ex.ToString());
            }
        }

        private static void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket client = (Socket)ar.AsyncState;

                // Complete the connection.  
                client.EndConnect(ar);

                // Console.WriteLine("Socket connected to {0}",
                //  client.RemoteEndPoint.ToString());
                // Signal that the connection has been made.  
                connectDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        static public void Disconnect()
        {
            try
            {
                client.BeginDisconnect(true, DisconnectCallback, client);
                disconnectDone.WaitOne();
            }
            catch
            {
               // Console.WriteLine("Something Wrong");
            }
        }

        static private void DisconnectCallback(IAsyncResult ar)
        {
            Socket client = (Socket)ar.AsyncState;
            client.EndDisconnect(ar);

            disconnectDone.Set();
        }
        #endregion

        #region SendAndReceive
        static public void ReceiveMessage()
        {
            receiveDone.Reset();
            Receive(client);
            receiveDone.WaitOne();
        }

        static void Receive(Socket client)
        {
            try
            {
                // Create the state object.  
                StateObject state = new StateObject();
                state.workSocket = client;

                // Begin receiving the data from the remote device.  
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception e)
            {
              //  Console.WriteLine(e.ToString());
            }
        }

        private static void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the state object and the client socket   
                // from the asynchronous state object.  
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.workSocket;

                // Read data from the remote device.  
                int bytesRead = client.EndReceive(ar);

                if (bytesRead > 0)
                {
                    // There might be more data, so store the data received so far.  
                    state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                    // Get the rest of the data.  
                    client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                        new AsyncCallback(ReceiveCallback), state);
                }
                else
                {
                    // All the data has arrived; put it in response.  
                    if (state.sb.Length >= 1)
                    {
                        response = state.sb.ToString();
                    }
                    // Signal that all bytes have been received.  
                    receiveDone.Set();
                }
            }
            catch (Exception e)
            {
               // Console.WriteLine(e.ToString());
            }
        }

        static public void SendMessage(string message)
        {
            sendDone.Reset();
            Send(client, $"{message};!");
            sendDone.WaitOne();
        }

        private static void Send(Socket client, String data)
        {
            // Convert the string data to byte data using ASCII encoding.  
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.  
            client.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), client);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket client = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = client.EndSend(ar);
              //  Console.WriteLine("Sent {0} bytes to server.", bytesSent);

                // Signal that all bytes have been sent.  
                sendDone.Set();
            }
            catch (Exception e)
            {
               // Console.WriteLine(e.ToString());
            }
        }
        #endregion
    }
}
