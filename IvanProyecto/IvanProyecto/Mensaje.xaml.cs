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
using System.Windows.Shapes;

namespace IvanProyecto {
    /// <summary>
    /// Lógica de interacción para Mensaje.xaml
    /// </summary>
    public partial class Mensaje : Window {
        public Mensaje() {
            InitializeComponent();
        }

        public Mensaje(String mensaje) {
            InitializeComponent();
            this.Title = mensaje;
        }
        public String predeterminado = "Buena Partida";
        
        private void Button_Click(object sender, RoutedEventArgs e) {

            DialogResult = true;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e) {
            DialogResult = false;
        }
    }
}
