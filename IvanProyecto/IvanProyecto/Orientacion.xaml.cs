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
    /// Lógica de interacción para Orientacion.xaml
    /// </summary>
    public partial class Orientacion : Window {

        //enum columnas{ A = 0, B,C,D,E,F,G, H, I, J};
        public Orientacion() {
            InitializeComponent();
        }

        public Orientacion(bool arriba, bool abajo, bool izq, bool dere) {
            InitializeComponent();
            if (arriba) {
                this.bArriba.IsEnabled = false;
            }
            if (abajo) {
                this.bAba.IsEnabled = false;
            }
            if (izq) {
                this.bIzqu.IsEnabled = false;
            }
            if (dere) {
                this.bDer.IsEnabled = false;
            }
        }

        public Orientacion(int columna, int fila, int tamaño) {
            InitializeComponent();
            if (columna == 0) {
                this.bIzqu.IsEnabled = false;
            } if (columna == 9) {
                this.bDer.IsEnabled = false;
            }
            if (fila == 0) {
                this.bArriba.IsEnabled = false;
            }
            if (fila == 9) {
                this.bAba.IsEnabled = false;
            }
        }
        //botón arriba
        private void Button_Click(object sender, RoutedEventArgs e) {
            DialogResult = true;
        }
        //botón izquierda
        private void Button_Click_1(object sender, RoutedEventArgs e) {

        }
        //botón abajo
        private void Button_Click_2(object sender, RoutedEventArgs e) {

        }
        //botón derecha
        private void Button_Click_3(object sender, RoutedEventArgs e) {

        }
    }
}
