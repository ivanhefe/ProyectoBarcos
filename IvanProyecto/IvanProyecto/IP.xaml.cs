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

        public string getLocalIp() {
            IPHostEntry host;
            string localIP = "?";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList) {
                if (ip.AddressFamily == AddressFamily.InterNetwork) {
                    localIP = ip.ToString();
                }
            }
            return localIP;
        }

        public int puertoAleatorio() { 
        Random random = new Random();
        return random.Next(11000, 11100);
        }

        public String sIP {
            get;
            set;
        }

        public String sPuerto {
            get;
            set;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            this.tbIP.Text = getLocalIp();
            this.tbPuerto.Text = puertoAleatorio().ToString();
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            this.sIP = this.tbIP.Text;
            this.sPuerto = this.tbPuerto.Text;
            DialogResult = true;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e) {
            DialogResult = false;
        }
    }
}
