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
        IPAddress ipAddressLocal, ipAddressRival;
        IPEndPoint localEndPoint;
        List<Barco> barcos;
        

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            string ipLocal ="";
            string puertoLocal = "";
            IP ventanaip = new IP();
            if (ventanaip.ShowDialog() == ventanaip.DialogResult) {
                ipLocal = ventanaip.sIP;
                puertoLocal = ventanaip.sPuerto;
            }

            anyadirCanvas();
            barcos = new List<Barco>();
            //metodo reiniciar partida

            //ELIMINAR
            //for (int i = 0; i < 10; i++) {
            //    barcos.Add(new Barco(3, canvasBarcos, 0));
            //}
            barcos.Add(new Barco(3, canvasBarcos, 0));
            barcos.Add(new Barco(2, canvasBarcos, 30));
            //barcos.Add(new Barco(2, canvasBarcos, 60));
            //barcos.Add(new Barco(1, canvasBarcos, 90));
            //barcos.Add(new Barco(1, canvasBarcos, 120));
            barcos.Add(new Barco(1, canvasBarcos, 120));


            ipAddressLocal = IPAddress.Parse(ipLocal);
            Console.WriteLine(ipAddressLocal.ToString());
            localEndPoint = new IPEndPoint(ipAddressLocal, Int16.Parse(puertoLocal));
            this.ventana.Title += " "+localEndPoint.ToString();
            worker.DoWork += worker_DoWork;
            //worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.RunWorkerAsync();
            //Console.WriteLine(GetLocalIP());
        }

        private void anyadirCanvas() {
            Canvas canvas = null;
            Thickness margen = new Thickness(1);
            //gridRival.Background = new SolidColorBrush(col);
            ImageBrush brush = new ImageBrush();
            brush.ImageSource = new BitmapImage(new Uri("Imagenes/fondo.png", UriKind.Relative));
            //gridPropio.Background = new SolidColorBrush(Colors.LightBlue);
            for (int i = 0; i < gridRival.ColumnDefinitions.Count; i++) {
                for (int j = 0; j < gridRival.RowDefinitions.Count; j++) {
                    canvas = new Canvas();
                    Grid.SetColumn(canvas, i);
                    Grid.SetRow(canvas, j);
                    canvas.Margin = margen;
                    canvas.Background = brush;
                    //canvas.Fill = new SolidColorBrush(col);
                    canvas.MouseDown += new MouseButtonEventHandler(canvas_MouseDown_1);
                    gridRival.Children.Add(canvas);
                }
            }
            for (int i = 0; i < gridPropio.ColumnDefinitions.Count; i++) {
                for (int j = 0; j < gridPropio.RowDefinitions.Count; j++) {
                    canvas = new Canvas();
                    Grid.SetColumn(canvas, i);
                    Grid.SetRow(canvas, j);
                    canvas.Margin = margen;
                    canvas.Background = brush;
                    canvas.AllowDrop = true;
                    canvas.Drop += ca_Drop;
                    //canvas.Fill = new SolidColorBrush(col);
                    //canvas.MouseDown += new MouseButtonEventHandler(canvas_MouseDown_1);
                    gridPropio.Children.Add(canvas);
                }
            }
        }

        //ipLocal
        public string GetLocalIP() {
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


        void ca_Drop(object sender, DragEventArgs e) {
            //abrir ventana que indique dirección
            int columna, fila;
            if (e.Handled == false) {
                Canvas canvasNuevo = (Canvas)sender;
                //MessageBox.Show(e.GetPosition(canvasNuevo).ToString());
                Label label = (Label)e.Data.GetData("Etiqueta");
                Barco barco = (Barco)e.Data.GetData("Object");
                //MessageBox.Show(barco.getTamaño().ToString());
                if (canvasNuevo != null && label != null) {
                    Canvas padre = (Canvas)VisualTreeHelper.GetParent(label);
                    if (padre != null) {
                        //MessageBox.Show("zxzx");
                        if (e.AllowedEffects.HasFlag(DragDropEffects.Move)) {
                            //rotar
                            columna = Grid.GetColumn(canvasNuevo);
                            fila = Grid.GetRow(canvasNuevo);
                            bool colocado = colocarBarco(canvasNuevo, label, barco, columna, fila);
                            //QUITAR
                            //if (Grid.GetColumn(canvasNuevo) + barco.getTamaño() > 20) {
                            //    MessageBox.Show("No se puede colocar aquí");
                            //}
                            //else {
                            if (colocado) {
                                padre.Children.Remove(label);
                                //MessageBox.Show(Grid.GetColumn(canvasNuevo).ToString());
                                canvasNuevo.Children.Add(label);
                                Canvas.SetTop(label, 0);
                                barco.eliminarMouseDown();
                                //va fuera

                            }
                            if (canvasBarcos.Children.Count == 0) {
                                MessageBox.Show("No tiene hijos");
                                this.bJugar.IsEnabled = true;
                            }
                        }
                    }
                }
            }
        }

        private bool colocarBarco(Canvas padre, Label label, Barco barco, int x, int y) {
            //si barco es mayor que 1 abre orientación
            bool ocupado = false;
            int direccion = 0;
            if (barco.getTamaño() > 1) {
                Orientacion or = new Orientacion(x, y, barco.getTamaño());
                or.ShowDialog();
                if (or.DialogResult == true) {
                    //0 derecha, 1 abajo, 2 izquierda, 3 arriba
                    direccion = or.Direccion;
                }else {
                    return false;
                }
            }
            for (int i = 0; i < barco.getTamaño(); i++) {
                for (int j = 0; j < barcos.Count; j++) {
                    ocupado = barcos[j].comprobarPosicion(x, y);
                    if (ocupado == true) {
                        //limpia las coordenadas del barco arrastrado
                        barco.eliminarCoordenadas();
                        break;
                    }
                }
                if (!ocupado) {
                    barco.anyadirCoordenadas(x, y);
                    Console.WriteLine(x.ToString() + " " + y.ToString());
                    switch (direccion) {
                        case 0:
                            x++;
                            break;
                        case 1:
                            y++;
                            break;
                        case 2:
                            x--;
                            break;
                        case 3:
                            y--;
                            break;
                    }
                }else {
                    Console.WriteLine("ocupado");
                    return false;
                }
            }
            RotateTransform rotateTransform1 = new RotateTransform(90 * direccion, 17.5, 17.5);
            label.RenderTransform = rotateTransform1;
            return true;
        }
        //SOCKET SERVIDOR, FALTA PODER CAMBIAR IP
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
                        if (data.IndexOf("FT") > -1) {
                            break;
                        }
                    }
                    //MessageBox.Show(data);
                    //BORRAR CONSOLE
                    Console.WriteLine("Text received : {0}", data);
                    Dispatcher dire = this.Dispatcher;
                    string mensaje = "men : ";
                    dire.BeginInvoke(DispatcherPriority.Normal, (Action)(() => ProcesarMensajes(data))
                        );
                    // Echo the data back to the client.
                    //mensaje = ProcesarMensajes(data);
                    byte[] msg = Encoding.ASCII.GetBytes(mensaje);
                    handler.Send(msg);
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }

        private void cambiar(String[] coor) {
            foreach (FrameworkElement canv in this.gridPropio.Children) {
                if (Convert.ToInt32(coor[0]) == Grid.GetColumn(canv) && Convert.ToInt32(coor[1]) == Grid.GetRow(canv)) {
                    ((Canvas)canv).Background = new SolidColorBrush(Colors.Red);
                    break;
                }
            }
        }

        private void socketCliente() {

        }

        private string ProcesarMensajes(string mensajeRecibido) {
            const int PJ = 6;
            int sas = 0;
            this.check.IsChecked = true;
            string[] mensaje = mensajeRecibido.Split('~');
            Int32 x, y;
            //aasdf(mensajeRecibido);
            switch (Convert.ToInt16(mensaje[0])) {
                case PJ:
                    //MessageBox.Show("asdf");
                    x = Convert.ToInt32(mensaje[1]);
                    y = Convert.ToInt32(mensaje[2]);
                    foreach (Barco barco in barcos) {
                        if (barco.comprobarPosicion(x, y)) {
                            sas = 1;
                        }
                        else {
                            sas = 2;
                        }
                    }
                    break;
                default:
                    sas = 3;
                    break;
            }
            return sas.ToString();
        }

        private void aasdf(string mensaje) {
            string[] mens = mensaje.Split('~');
            int x = Convert.ToInt32(mens[1]);
            //MessageBox.Show(x.ToString());
        }
        //BORRAR

        private void mandarMensaje(String mensaje) {
        
        
        }
        #region messagecall
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
        #endregion

        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e) {

        }

        private void canvas_MouseDown_1(object sender, MouseButtonEventArgs e) {

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

        //botón nick
        private void Button_Click(object sender, RoutedEventArgs e) {
            Window2 w2 = new Window2();
            w2.ShowDialog();
        }

        //botón jugar
        private void Button_Click_1(object sender, RoutedEventArgs e) {
            //this.grid.Visibility = Visibility.Visible;
            this.canvasBarcos.Visibility = Visibility.Collapsed;
            this.gridRival.Visibility = Visibility.Visible;
            this.ventana.Width = 750;
        }

        public static class Mensajes {
        }

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

//async
//epLocal = new IPEndPoint(IPAddress.Parse("127.0.0.1"), Convert.ToInt32(this.puerto.Text));
//sock.Bind(epLocal);

//epRemote = new IPEndPoint(IPAddress.Parse("127.0.0.1"), Convert.ToInt32(this.puerto2.Text));
//sock.Connect(epRemote);

//byte[] buffer = new byte[1024];
//sock.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epRemote, new AsyncCallback(MessageCallBack), buffer);