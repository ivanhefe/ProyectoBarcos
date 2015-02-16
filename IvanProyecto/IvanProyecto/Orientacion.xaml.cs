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

        public Orientacion() {
            InitializeComponent();
        }

        public Orientacion(int columna, int fila, int tamaño) {
            Console.WriteLine(tamaño.ToString());
            InitializeComponent();          
            if (columna == 0 || columna - tamaño < -1 ) {
                this.bIzqu.IsEnabled = false;
            } 
            if (columna == 9 || columna + tamaño > 10) {
                this.bDer.IsEnabled = false;
            }
            if (fila == 0 || fila - tamaño < -1) {
                this.bArriba.IsEnabled = false;
            }
            if (fila == 9 || fila + tamaño >10) {
                this.bAba.IsEnabled = false;
            }
            Direccion = 4;
        }

        public int Direccion {
            get;
            set;
        }
        //botón arriba
        private void Button_Click(object sender, RoutedEventArgs e) {
            this.Direccion = 3;
            DialogResult = true;
        }
        //botón izquierda
        private void Button_Click_1(object sender, RoutedEventArgs e) {
            this.Direccion = 2;
            DialogResult = true;
        }
        //botón abajo
        private void Button_Click_2(object sender, RoutedEventArgs e) {
            this.Direccion = 1;
            DialogResult = true;
        }
        //botón derecha
        private void Button_Click_3(object sender, RoutedEventArgs e) {
            this.Direccion = 0;
            DialogResult = true;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {          
            
        }

        private void Window_Closed(object sender, EventArgs e) {           
           
        }
    }
}
