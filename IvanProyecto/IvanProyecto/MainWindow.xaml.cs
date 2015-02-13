using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Threading;

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
        private readonly BackgroundWorker worker = new BackgroundWorker();
        IPHostEntry ipHostInfo;
        IPAddress ipAddress;
        IPEndPoint localEndPoint;

        private void Window_Loaded(object sender, RoutedEventArgs e) {

            Canvas ca;
            Color col = Color.FromRgb(41, 40, 43);
            Thickness margen = new Thickness(1);
            gridRival.Background = new SolidColorBrush(col);
            gridPropio.Background = new SolidColorBrush(Colors.LightBlue);
            for (int i = 0; i < gridRival.ColumnDefinitions.Count; i++) {
                for (int j = 0; j < gridRival.RowDefinitions.Count; j++) {
                    ca = new Canvas();
                    Grid.SetColumn(ca, i);
                    Grid.SetRow(ca, j);
                    ca.Margin = margen;
                    ca.Background = new SolidColorBrush(Colors.Azure);
                    //ca.Fill = new SolidColorBrush(col);

                    ca.MouseDown += new MouseButtonEventHandler(Rectangle_MouseDown_1);
                    gridRival.Children.Add(ca);
                }
            }
            //drag and drop
            //https://msdn.microsoft.com/en-us/library/ms742859%28v=vs.110%29.aspx

            for (int i = 0; i < gridPropio.ColumnDefinitions.Count; i++) {
                for (int j = 0; j < gridPropio.RowDefinitions.Count; j++) {
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

            //sockets

            //sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            //sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);


            //try {
            //    //sockCli.Connect(localEndPoint);
            //    //Convert.ToBase64CharArray("prueba2<EOF>");
            //    //sockCli.Connect(localEndPoint);

            //    //sockCli.Shutdown(SocketShutdown.Both);
            //    //sockCli.Close();


            //}
            //catch (Exception ex) {
            //    MessageBox.Show(ex.Message);
            //}
            ipHostInfo = Dns.Resolve(Dns.GetHostName());
            ipAddress = ipHostInfo.AddressList[0];
            localEndPoint = new IPEndPoint(ipAddress, 11000);
            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.RunWorkerAsync();
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {

        }



        Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        private void worker_DoWork(object sender, DoWorkEventArgs e) {
            String data = null;
            byte[] bytes = new Byte[1024];
            try {
                listener.Bind(localEndPoint);
                listener.Listen(1);
                while (true) {
                    Console.WriteLine("Waiting for a connection...");
                    // Program is suspended while waiting for an incoming connection.
                    Socket handler = listener.Accept();

                    // An incoming connection needs to be processed.
                    while (true) {
                        bytes = new byte[1024];
                        int bytesRec = handler.Receive(bytes);
                        data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                        if (data.IndexOf("<EOF>") > -1) {

                            break;
                        }
                    }

                    // Show the data on the console.
                    //MessageBox.Show(data);
                    Console.WriteLine("Text received : {0}", data);
                    Dispatcher dire = this.Dispatcher;
                    dire.BeginInvoke(DispatcherPriority.Normal, (Action)(() => ProcesarMensajes(data)));

                    // Echo the data back to the client.
                    byte[] msg = Encoding.ASCII.GetBytes("asdf");

                    //handler.Send(msg);
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }

            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }

        }

        //private String localIp() {
        //    IPHostEntry host;
        //    host = Dns.GetHostEntry(Dns.GetHostName());
        //    foreach (IPAddress ip in host.AddressList) {
        //        if (ip.AddressFamily == AddressFamily.InterNetwork) {
        //            return ip.ToString();
        //        }
        //    }
        //    return "127.0.0.1";
        //}

        private void cambiar(String[] coor) {
            foreach (FrameworkElement canv in this.gridPropio.Children) {
                if (Convert.ToInt32(coor[0]) == Grid.GetColumn(canv) && Convert.ToInt32(coor[1]) == Grid.GetRow(canv)) {
                    ((Canvas)canv).Background = new SolidColorBrush(Colors.Red);
                    break;
                }
            }
        }



        private void ProcesarMensajes(string mensajeRecibido) {            
            const int PJ = 0;
            this.check.IsChecked = true;
            string[] mensaje = mensajeRecibido.Split('~');
            Int32 x, y;
            aasdf(mensajeRecibido);
            switch (Convert.ToInt16(mensaje[0])) {
                case PJ:
                    MessageBox.Show("asdf");
                    break;
                default:                   
                    x = Convert.ToInt32(mensaje[1]);
                    y = Convert.ToInt32(mensaje[2]);
                    foreach (FrameworkElement canv in this.gridPropio.Children) {
                        if (x == Grid.GetColumn(canv) && y == Grid.GetRow(canv)) {
                            ((Canvas)canv).Background = new SolidColorBrush(Colors.Red);
                            break;
                        }
                    }
                    break;
            }
        }

        private void aasdf(string mensaje) {
            string[] mens = mensaje.Split('~');
            int x = Convert.ToInt32(mens[1]);
            MessageBox.Show(x.ToString());
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

                    string coord = receivedMessage.ToString().Substring(0, 3);
                    String[] coord2 = coord.Split(',');

                    //cambiar(coor);
                    Action action = delegate {
                        foreach (FrameworkElement canv in this.gridPropio.Children) {
                            if (Convert.ToInt32(coord2[0]) == Grid.GetColumn(canv) && Convert.ToInt32(coord2[1]) == Grid.GetRow(canv)) {
                                ((Canvas)canv).Background = new SolidColorBrush(Colors.Red);
                                break;
                            }
                        }
                    };
                    Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, action);
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
                    msg = enc.GetBytes(x + "," + y);
                    sock.Send(msg);
                }
                catch (Exception ex) {
                    MessageBox.Show(ex.Message);
                }
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

        private void Button_Click(object sender, RoutedEventArgs e) {
            Window2 w2 = new Window2();
            w2.ShowDialog();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e) {
            //this.grid.Visibility = Visibility.Visible;
            //this.barcos.Visibility = Visibility.Collapsed;

            //epLocal = new IPEndPoint(IPAddress.Parse("127.0.0.1"), Convert.ToInt32(this.puerto.Text));
            //sock.Bind(epLocal);

            //epRemote = new IPEndPoint(IPAddress.Parse("127.0.0.1"), Convert.ToInt32(this.puerto2.Text));
            //sock.Connect(epRemote);

            //byte[] buffer = new byte[1024];
            //sock.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epRemote, new AsyncCallback(MessageCallBack), buffer);

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

    public static class Mensajes {


    }

    public class Barco {
        private int vida;
        private int tamaño;
        List<Coordenadas> coordenadas = new List<Coordenadas> ();
        Label etiqueta;

        public Barco(int tamaño, FrameworkElement padre) {
            this.tamaño = tamaño;
            this.vida = tamaño;
            etiqueta = new Label();
            if (true) {
                
            }
        }

        public void anyadirCoordenadas(int x, int y){
            coordenadas.Add(new Coordenadas(x, y));
        }

        //comprueba si el disparo enemigo acierta en alguna coordenada
        public Boolean comprobarPosicion(int x, int y){
            for (int i = 0; i < coordenadas.Count; i++) {
                if (coordenadas[i].comprobar(x,y)) {
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

        public void setX(int x){
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


