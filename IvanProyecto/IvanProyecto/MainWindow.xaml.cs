using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IvanProyecto {
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private string p;
        //IPAddress host = 
        public MainWindow() {

        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            Rectangle re, re1;
            Color col = Color.FromRgb(41, 40, 43);
            grid.Background = new SolidColorBrush(col);
            gridPropio.Background = new SolidColorBrush(Colors.LightBlue);
            for (int i = 0; i < grid.ColumnDefinitions.Count; i++) {
                for (int j = 0; j < grid.RowDefinitions.Count; j++) {
                    re = new Rectangle();
                    Grid.SetColumn(re, i);
                    Grid.SetRow(re, j);
                   
                    re.Fill = new SolidColorBrush(col);
                    re.Stroke = new SolidColorBrush(Colors.Black);
                    re.MouseDown += new MouseButtonEventHandler(Rectangle_MouseDown_1);
                    grid.Children.Add(re);
                }
            }

            //for (int i = 0; i < gridPropio.ColumnDefinitions.Count; i++) {
            //    for (int j = 0; j < gridPropio.RowDefinitions.Count; j++) {
            //        re = new Rectangle();
            //        Grid.SetColumn(re, i);
            //        Grid.SetRow(re, j);
            //        re.Fill = new SolidColorBrush(Colors.LightBlue);
            //        re.Stroke = new SolidColorBrush(Colors.Black);
            //        re.MouseDown += new MouseButtonEventHandler(Rectangle_MouseDown_1);
            //        gridPropio.Children.Add(re);
            //    }
            //}
        }

        public MainWindow(string p) {
            InitializeComponent();
            this.p = p;
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            MessageBox.Show("asdf");
            //asdfdsf
            //Socket soc = new Socket("localhost", "5555");
            //soc.Accept();
        }

        private void Grid_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e) {
            MessageBox.Show(e.GetPosition(this).X.ToString());
            MessageBox.Show("You clicked me at " + e.GetPosition(this).ToString());
            //if (e.GetPosition(this).ToString()) {

            //}
        }

        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e) {

        }

        private void Rectangle_MouseDown_1(object sender, MouseButtonEventArgs e) {
            Rectangle re = (Rectangle)sender;
            int x = Grid.GetColumn(re);
            int y = Grid.GetRow(re);
            re.Fill = new SolidColorBrush(Colors.Red);

            //MessageBox.Show(grid.ColumnDefinitions.Count.ToString());
            MessageBox.Show(x + "," + y);
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e) {
            //ventana arriba del todo
            //CheckBox cb =(CheckBox) sender;
            //if (cb.IsChecked == true) {
            //    ventana.Topmost = true;
            //}
            //else {
            //    ventana.Topmost = false;
            //}
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            Window2 w2 = new Window2();
            w2.ShowDialog();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e) {
            this.grid.Visibility = Visibility.Visible;
            this.barcos.Visibility = Visibility.Collapsed;
        }

        private void Label_MouseMove(object sender, MouseEventArgs e) {
            Label la = sender as Label;
            if (la!= null && e.LeftButton== MouseButtonState.Pressed) {
                DragDrop.DoDragDrop(la, la.Content.ToString(), DragDropEffects.Move);
            }
        }

        private void Ellipse_MouseMove(object sender, MouseEventArgs e) {
            Ellipse ellipse = sender as Ellipse;
            if (ellipse != null && e.LeftButton == MouseButtonState.Pressed) {
                DragDrop.DoDragDrop(ellipse, ellipse.Fill.ToString(), DragDropEffects.Copy);
            }
        }
    }
}
