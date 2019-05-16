using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AspNetSignalR.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Alle Logik sollte in ein ViewModel
        // da nur SignalR demonstriert werden soll
        // ist alles in dieser Code-Behind
        private const string URL = "http://localhost:2970/signalr";
        private HubConnection hubConnection;
        private IHubProxy chatHubProxy;
        private bool connected = false;

        public MainWindow()
        {
            InitializeComponent();

        }

        private async void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await InitHubConnection();
                btnSend.IsEnabled = true;
                connected = true;
            }
            catch
            {
                MessageBox.Show("Verbindung mit " + URL + " konnte nicht hergestellt werden.");
            }
        }

        private async Task InitHubConnection()
        {
            hubConnection = new HubConnection(URL);
            chatHubProxy = hubConnection.CreateHubProxy("ChatHub");

            chatHubProxy.On<string, string>("newMessage", (name, message) => { AddMessage(name, message); });
            chatHubProxy.On<string>("invalidMessage", message => { AddMessage("Admin", message); });

            await hubConnection.Start();
        }

        private void AddMessage(string name, string message)
        {
            Dispatcher.Invoke(() =>
            {
                var msg = GetMessageString(name, message);
                if (rtbMessages.Document.Blocks.Count == 0)
                {
                    rtbMessages.AppendText(msg);
                }
                else
                {
                    rtbMessages.Document.Blocks.InsertBefore(rtbMessages.Document.Blocks.First(), new Paragraph(new Run(msg)));
                }
            });
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            if (connected)
            {
                chatHubProxy.Invoke("Send", txtUsername.Text, txtMessage.Text);
            }
        }

        private string GetMessageString(string name, string message)
        {
            return string.Format("{0} [{1}]: {2}", name, DateTime.Now.ToShortTimeString(), message);
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (hubConnection != null)
            {
                hubConnection.Stop();
                hubConnection.Dispose();
            }
        }
    }
}
