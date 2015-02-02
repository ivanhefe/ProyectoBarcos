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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IvanProyecto {
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }

        //sockets
        Socket sock;
        EndPoint epLocal, epRemote;

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            #region poner canvas
            Canvas ca;
            Color col = Color.FromRgb(41, 40, 43);
            Thickness margen = new Thickness(1);
            grid.Background = new SolidColorBrush(col);
            gridPropio.Background = new SolidColorBrush(Colors.LightBlue);
            for (int i = 0; i < grid.ColumnDefinitions.Count; i++) {
                for (int j = 0; j < grid.RowDefinitions.Count; j++) {
                    ca = new Canvas();
                    Grid.SetColumn(ca, i);
                    Grid.SetRow(ca, j);
                    ca.Margin = margen;
                    ca.Background = new SolidColorBrush(Colors.Azure);
                    //ca.Fill = new SolidColorBrush(col);

                    ca.MouseDown += new MouseButtonEventHandler(Rectangle_MouseDown_1);
                    grid.Children.Add(ca);
                }
            }
            //drag and drop
            //https://msdn.microsoft.com/en-us/library/ms742859%28v=vs.110%29.aspx

            for (int i = 0; i < grid.ColumnDefinitions.Count; i++) {
                for (int j = 0; j < grid.RowDefinitions.Count; j++) {
                    ca = new Canvas();
                    Grid.SetColumn(ca, i);
                    Grid.SetRow(ca, j);
                    ca.Margin = margen;
                    ca.Background = new SolidColorBrush(Colors.Azure);
                    //ca.Fill = new SolidColorBrush(col);                  
                    //ca.MouseDown += new MouseButtonEventHandler(Rectangle_MouseDown_1);
                    gridPropio.Children.Add(ca);
                }
            }
            #endregion
            //sockets
            
            sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            
            try {
                //sockCli.Connect(localEndPoint);
                //Convert.ToBase64CharArray("prueba2<EOF>");
                //sockCli.Connect(localEndPoint);

                //sockCli.Shutdown(SocketShutdown.Both);
                //sockCli.Close();
                
                
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }

        private String localIp() {
            IPHostEntry host;
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList) {
                if (ip.AddressFamily == AddressFamily.InterNetwork) {
                    return ip.ToString();
                }
            }
            return "127.0.0.1";
        }

        private void cambiar(String[] coor) {
            foreach (FrameworkElement canv in this.gridPropio.Children) {
                if (Convert.ToInt32(coor[0]) == Grid.GetColumn(canv) && Convert.ToInt32(coor[1]) == Grid.GetRow(canv)) {
                    ((Canvas)canv).Background = new SolidColorBrush(Colors.Red);
                    break;
                }
            }
        
        }

        private void MessageCallBack(IAsyncResult aResult) {
            try {
                int size = sock.EndReceiveFrom(aResult, ref epRemote);
                if (size > 0) {
                    byte[] receivedData = new byte[1024];
                    receivedData = (byte[])aResult.AsyncState;
                    ASCIIEncoding eEncoding = new ASCIIEncoding();
                    string receivedMessage = eEncoding.GetString(receivedData);
                    MessageBox.Show(receivedMessage);
                    String[] coor = receivedData.ToString().Split(',');
                    cambiar(coor);
                    
                }

                byte[] buffer = new byte[1024];
                sock.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epRemote, new AsyncCallback(MessageCallBack), buffer);
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }

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

        //devuelve columna y fila
        private void Rectangle_MouseDown_1(object sender, MouseButtonEventArgs e) {

            Canvas can = sender as Canvas;
            if (can.Tag == null) {
                int x = Grid.GetColumn(can);
                int y = Grid.GetRow(can);
                can.Background = new SolidColorBrush(Colors.Red);
                can.Tag = new object();
                //mandar las coordenadas al otro jugador

                try {
                    ASCIIEncoding enc = new ASCIIEncoding();
                    byte[] msg = new byte[1024];
                    msg = enc.GetBytes(x+","+y);

                    sock.Send(msg);

                }
                catch (Exception ex) {
                    MessageBox.Show(ex.Message);   
                }
                //MessageBox.Show(x + "," + y);

                //recibir coordenadas
                //foreach (FrameworkElement canv in this.gridPropio.Children) {
                //    if (x == Grid.GetColumn(canv) && y == Grid.GetRow(canv)) {
                //        ((Canvas)canv).Background = new SolidColorBrush(Colors.Red);
                //        break;
                //    }
                //}
            }
            else {
                MessageBox.Show("Ya has clickeado en esta casilla");
            }
        }

        private void enviarComando(String comando) {
            byte[] msg = Encoding.ASCII.GetBytes("mensaje prueba<EOF>");
            try {
                //sock.Connect(localEndPoint);
                //sock.Send(msg);
                //sock.Shutdown(SocketShutdown.Both);
                //sock.Disconnect(true);
            }
            catch (Exception ex) {

                MessageBox.Show(ex.Message);
            }

            //sockCli.Shutdown(SocketShutdown.Both);


        }
        private void CheckBox_Checked(object sender, RoutedEventArgs e) {

        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            Window2 w2 = new Window2();
            w2.ShowDialog();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e) {
            //this.grid.Visibility = Visibility.Visible;
            //this.barcos.Visibility = Visibility.Collapsed;

            epLocal = new IPEndPoint(IPAddress.Parse("127.0.0.1"), Convert.ToInt32(this.puerto.Text));
            sock.Bind(epLocal);

            epRemote = new IPEndPoint(IPAddress.Parse("127.0.0.1"), Convert.ToInt32(this.puerto2.Text));
            sock.Connect(epRemote);

            byte[] buffer = new byte[1024];
            sock.BeginReceiveFrom(buffer, 0, buffer.Length ,SocketFlags.None, ref epRemote, new AsyncCallback(MessageCallBack),buffer);

        }

        #region draganddrop no va
        private void Label_MouseMove(object sender, MouseEventArgs e) {
            Label la = sender as Label;
            if (la != null && e.LeftButton == MouseButtonState.Pressed) {
                DragDrop.DoDragDrop(la, la.Content.ToString(), DragDropEffects.Move);
            }
        }

        private void Ellipse_MouseMove(object sender, MouseEventArgs e) {
            Ellipse ellipse = sender as Ellipse;
            if (ellipse != null && e.LeftButton == MouseButtonState.Pressed) {
                DragDrop.DoDragDrop(ellipse, ellipse.Fill.ToString(), DragDropEffects.Copy);
            }
        }

        private void gridPropio_Drop(object sender, DragEventArgs e) {


        }

        private Brush _previousFill = null;
        private void gridPropio_DragEnter(object sender, DragEventArgs e) {
            if (e.Data.GetDataPresent(DataFormats.Bitmap)) {
                e.Effects = DragDropEffects.Copy;
            }
            else {
                e.Effects = DragDropEffects.None;
            }
        }

        private void gridPropio_DragOver(object sender, DragEventArgs e) {
            e.Effects = DragDropEffects.None;

            // If the DataObject contains string data, extract it. 
            if (e.Data.GetDataPresent(DataFormats.StringFormat)) {
                string dataString = (string)e.Data.GetData(DataFormats.StringFormat);

                // If the string can be converted into a Brush, allow copying.
                BrushConverter converter = new BrushConverter();
                if (converter.IsValid(dataString)) {
                    e.Effects = DragDropEffects.Copy | DragDropEffects.Move;
                }
            }
        }
        #endregion
    }
}
