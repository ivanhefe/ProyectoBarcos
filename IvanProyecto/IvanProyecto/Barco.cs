using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace IvanProyecto {

    public class Barco {

        private int vida;
        private int tamaño;
        List<Coordenadas> coordenadas = new List<Coordenadas>();
        Label etiqueta;

        public Barco(int tamaño, FrameworkElement padre, int posicion) {
            string tam = tamaño.ToString();
            var brush = new ImageBrush();
            brush.ImageSource = new BitmapImage(new Uri("./Imagenes/barco"+tam+".png", UriKind.Relative));
            this.tamaño = tamaño;
            this.vida = tamaño;
            etiqueta = new Label();
            if (tamaño == 1) {
                this.etiqueta.Background = brush;
                this.etiqueta.Width = 33 * tamaño;
                this.etiqueta.Height = 33;
            }
            else {
                string texto = new string('X', tamaño);
                etiqueta.Content = texto;
                etiqueta.FontSize = 34;
                etiqueta.FontWeight = FontWeights.ExtraBlack;
            }
            
            etiqueta.MouseDown += etiqueta_MouseDown;
            ((Canvas)padre).Children.Add(etiqueta);
            Canvas.SetTop(etiqueta, posicion);
        }

        public int getTamaño() {
            return tamaño;
        }
        void etiqueta_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            Label etiqueta = sender as Label;
            DataObject data = new DataObject();
            data.SetData("Etiqueta", this.etiqueta);
            data.SetData("Object", this);
            if (etiqueta != null && e.LeftButton == MouseButtonState.Pressed) {
                
                DragDrop.DoDragDrop(etiqueta, data, DragDropEffects.Move);
                
            }
        }

        public void anyadirCoordenadas(int x, int y) {
            coordenadas.Add(new Coordenadas(x, y));
        }

        public void eliminarMouseDown() {
            this.etiqueta.MouseDown -= etiqueta_MouseDown;
        }
        //comprueba si el disparo enemigo acierta en alguna coordenada
        public Boolean comprobarPosicion(int x, int y) {
            for (int i = 0; i < coordenadas.Count; i++) {
                if (coordenadas[i].comprobar(x, y)) {
                    return true;
                }
            }
            return false;
        }
    }
    public class Coordenadas {
        private int x;
        private int y;

        public Coordenadas(int x, int y) {
            this.x = x;
            this.y = y;
        }

        public void setX(int x) {
            this.x = x;
        }

        public Boolean comprobar(int x, int y) {
            if (this.x == x && this.y == y)
                return true;
            return false;
        }

        public override string ToString() {
            return x.ToString() + "" + y.ToString();
        }
    }
}
