using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace IvanProyecto {
    /// <summary>
    /// Lógica de interacción para IP.xaml
    /// </summary>
    public partial class IP : Window {
        public IP() {
            InitializeComponent();
        }

        public String sIP {
            get;
            set;
        }

        public String sPuerto {
            get;
            set;
        }

        public String ipPropia {
            get;
            set;

        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            this.tbIP.Text = GetLocalIP();
            this.tbPuerto.Text = "11000";

        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            this.sIP = this.tbIP.Text;
            this.sPuerto = this.tbPuerto.Text;
            if (internet.IsChecked == true) {
                ipPropia = GetExternalIP();
            }
            else {
                ipPropia = GetLocalIP();
            }
            this.DialogResult = true;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e) {
            this.DialogResult = false;
        }

        public string GetLocalIP() {
            IPHostEntry host;
            string localIP = "";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList) {
                if (ip.AddressFamily == AddressFamily.InterNetwork) {
                    localIP = ip.ToString();
                    return localIP;
                }
            }
            return "127.0.0.1";
        }

        public string GetExternalIP() {
            string url = "http://checkip.dyndns.org";
            System.Net.WebRequest req = System.Net.WebRequest.Create(url);
            System.Net.WebResponse resp = req.GetResponse();
            System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
            string response = sr.ReadToEnd().Trim();
            string[] a = response.Split(':');
            string a2 = a[1].Substring(1);
            string[] a3 = a2.Split('<');
            string a4 = a3[0];
            return a4;
        }
    }
}
