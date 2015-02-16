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

        private readonly BackgroundWorker worker = new BackgroundWorker();
        IPAddress ipAddressLocal, ipAddressRival;
        IPEndPoint localEndPoint, rivalEndPoint;
        List<Barco> barcos;
        String nick, nickRival, puertoLocal;
        int ultimoX, ultimoY;
        Boolean turno = false, partidaComenzada = false;

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            nick = this.tbNick.Text;
            string ipLocal = "";
            //genero un número aleatorio para poder probar en el mismo pc
            Random ra = new Random();
            ipLocal = GetLocalIP();
            //puertoLocal = ra.Next(11000, 11100).ToString();
            puertoLocal = "11000";

            anyadirCanvas();
            barcos = new List<Barco>();
            barcos.Add(new Barco(3, canvasBarcos, 0));
            barcos.Add(new Barco(2, canvasBarcos, 30));
            barcos.Add(new Barco(2, canvasBarcos, 30));
            barcos.Add(new Barco(1, canvasBarcos, 60));
            barcos.Add(new Barco(1, canvasBarcos, 60));
            barcos.Add(new Barco(1, canvasBarcos, 60));

            ipAddressLocal = IPAddress.Parse(ipLocal);
            localEndPoint = new IPEndPoint(ipAddressLocal, Int16.Parse(puertoLocal));
            this.ventana.Title += " " + localEndPoint.ToString();
            worker.DoWork += worker_DoWork;
            worker.RunWorkerAsync();

        }

        //eliminar y sacar posición mediante divisiones
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
                    return localIP;
                }
            }
            return "127.0.0.1";
        }

        void ca_Drop(object sender, DragEventArgs e) {
            int columna, fila;
            if (e.Handled == false) {
                Canvas canvasNuevo = (Canvas)sender;
                Label label = (Label)e.Data.GetData("Etiqueta");
                Barco barco = (Barco)e.Data.GetData("Object");
                if (canvasNuevo != null && label != null) {
                    Canvas padre = (Canvas)VisualTreeHelper.GetParent(label);
                    if (padre != null) {
                        if (e.AllowedEffects.HasFlag(DragDropEffects.Move)) {
                            //rotar
                            columna = Grid.GetColumn(canvasNuevo);
                            fila = Grid.GetRow(canvasNuevo);
                            bool colocado = colocarBarco(canvasNuevo, label, barco, columna, fila);
                            if (colocado) {
                                padre.Children.Remove(label);
                                canvasNuevo.Children.Add(label);
                                Canvas.SetTop(label, 0);
                                //elimina el evento de poder arrastrar
                                barco.eliminarMouseDown();
                            }
                            if (canvasBarcos.Children.Count == 0) {
                                //MessageBox.Show("No tiene hijos");
                                this.bJugar.IsEnabled = true;
                            }
                        }
                    }
                }
            }
        }

        private bool colocarBarco(Canvas padre, Label label, Barco barco, int x, int y) {
            //si el barco es mayor que 1 abre orientación
            bool ocupado = false;
            int direccion = 0;
            //los barcos de 1 casilla no es necesario pedir dirección
            if (barco.getTamaño() > 1) {
                Orientacion or = new Orientacion(x, y, barco.getTamaño());
                
                or.ShowDialog();
                if (or.DialogResult == true) {
                    //0 derecha, 1 abajo, 2 izquierda, 3 arriba
                    direccion = or.Direccion;
                }
                else {
                    return false;
                }

                //"funciona"
                //or.ShowDialog();
                //direccion = or.Direccion;
                //dirección 4 es al cerrar la ventana de direcciones
                //if (direccion == 4) {
                //    return false;
                //}
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
                //mejorar
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
                }
                else {
                    //si alguna casilla corresponde con otro barco, no lo añade
                    return false;
                }
            }
            RotateTransform rotateTransform1 = new RotateTransform(90 * direccion, 17.5, 17.5);
            label.RenderTransform = rotateTransform1;
            return true;
        }

        //socket servidor
        Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        Socket handler;
        private void worker_DoWork(object sender, DoWorkEventArgs e) {
            String data = null;
            byte[] bytes = new Byte[1024];
            try {
                listener.Bind(localEndPoint);
                listener.Listen(1);
                while (true) {
                    //Console.WriteLine("Waiting for a connection...");
                    handler = listener.Accept();
                    data = null;
                    while (true) {
                        bytes = new byte[1024];
                        int bytesRec = handler.Receive(bytes);
                        data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                        if (data.IndexOf("FT") > -1) {
                            break;
                        }
                    }
                    Dispatcher dire = this.Dispatcher;
                    dire.BeginInvoke(DispatcherPriority.Normal, (Action)(() => ProcesarMensajes(data)));
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }

        //procesa los mensajes recibidos en el servidor, todo esto debería ir en otra clase
        private void ProcesarMensajes(string mensajeRecibido) {
            const int PP = 0;
            const int RL = 1;
            const int JU = 4;
            const int RJ = 5;
            const int FP = 6;
            const int CF = 7;

            string[] mensaje = mensajeRecibido.Split('~');
           
            switch (Convert.ToInt16(mensaje[0])) {
                case PP:
                    partidaPreparada(mensajeRecibido);
                    break;
                case RL:
                    rivalListo(mensajeRecibido);
                    break;
                case JU:
                    jugadaRecibida(mensajeRecibido);
                    break;
                case RJ:
                    resultadoJugada(mensajeRecibido);
                    break;
                case FP:
                    finPartida(mensajeRecibido);
                    break;
                case CF:
                    contestacionFin(mensajeRecibido);
                    break;
            }
        }

        private void partidaPreparada(string mensaje) {
            string[] mensajeDividido = mensaje.Split('~');
            string ipRival, puertoRival;
            ipRival = mensajeDividido[1];
            //Console.WriteLine(ipRival + " ip");
            ipAddressRival = IPAddress.Parse(ipRival);
            puertoRival = mensajeDividido[2];
            //Console.WriteLine(puertoRival + " puerto");
            rivalEndPoint = new IPEndPoint(ipAddressRival, Convert.ToInt16(puertoRival));
            nickRival = mensajeDividido[3];
            this.lbNickRival.Content = nickRival;
            this.check.IsChecked = true;
            partidaComenzada = true;
        }

        private void rivalListo(string mensaje) {
            string[] mensajeDividido = mensaje.Split('~');
            this.lbNickRival.Content = mensajeDividido[1];
            this.partidaComenzada = true;
            this.turno = true;
            this.bJugar.IsEnabled = false;
        }

        private void jugadaRecibida(string mensaje) {
            string[] mensajeDividido = mensaje.Split('~');
            int x = Convert.ToInt16(mensajeDividido[1]);
            int y = Convert.ToInt16(mensajeDividido[2]);
            Boolean acierto = false, hundido = false;
            for (int i = 0; i < barcos.Count; i++) {
                if (barcos[i].comprobarPosicion(x, y)) {
                    acierto = true;
                    hundido = barcos[i].restarVida();
                    //Console.WriteLine("acierto");
                    //Console.WriteLine(hundido.ToString());
                    break;
                }
                //Console.WriteLine("for jugadaRecibida");
            }
            cambiarCuadricula(x, y, acierto, gridPropio);
            //responderJugada
            int aciertoNum = Convert.ToInt16(acierto ? 1 : 0);
            int hundidoNum = Convert.ToInt16(hundido ? 1 : 0);
            mandarMensaje("5~" + aciertoNum + "~" + hundidoNum + "~FT");
            comprobarBarcos();
            this.turno = true;
        }

        private void comprobarBarcos() {
            Boolean hundidos = false;
            for (int i = 0; i < barcos.Count; i++) {
                hundidos = barcos[i].comprobarHundido();
                if (hundidos == false) {
                    break;
                }
            }
            if (hundidos) {
                //manda mensaje de final de partida
                String mensaje = "";
                Mensaje mens = new Mensaje("Has perdido");
                this.turno = false;
                mens.ShowDialog();
                if (mens.DialogResult == true) {
                    mensaje = mens.texto.Text;
                }
                else {
                    mensaje = mens.predeterminado;
                }
                mandarMensaje("6~"+mensaje+"~FT");
                
            }
        }

        private void resultadoJugada(string mensaje) {
            string[] mensajeDividido = mensaje.Split('~');
            Boolean acierto = Convert.ToBoolean(Convert.ToInt16(mensajeDividido[1]) == 1 ? true : false);
            cambiarCuadricula(this.ultimoX, this.ultimoY, acierto, gridRival);
            Boolean hundido = Convert.ToBoolean(Convert.ToInt16(mensajeDividido[2]) == 1 ? true : false);
            if (hundido) {
                MessageBox.Show("Barco Hundido");
            }
        }

        private void finPartida(string mensaje) {
            string[] mensajeDividido = mensaje.Split('~');
            MessageBox.Show(mensajeDividido[1]);
            String mensa = "";
            Mensaje mens = new Mensaje("Has Ganado");
            this.turno = false;
            mens.ShowDialog();
            if (mens.DialogResult == true) {
                mensa = mens.texto.Text;
            }
            else {
                mensa = mens.predeterminado;
            }
            this.partidaComenzada = false;
            mandarMensaje("7~"+mensa+"~FT");
        }

        private void contestacionFin(string mensaje) {
            string[] mensajeDividido = mensaje.Split('~');
            this.turno = false;
            MessageBox.Show(mensajeDividido[1]);
        }

        //añade un "palo" en la casilla correspodiente
        private void cambiarCuadricula(int x, int y, Boolean acierto, Grid grid) {
            ImageBrush brush = new ImageBrush();
            Canvas canvas = new Canvas();
            canvas.Height = 35;
            canvas.Width = 35;
            if (acierto) {
                brush.ImageSource = new BitmapImage(new Uri("Imagenes/paloBarco.png", UriKind.Relative));
            }
            else {
                brush.ImageSource = new BitmapImage(new Uri("Imagenes/paloAgua.png", UriKind.Relative));
            }
            foreach (FrameworkElement canv in grid.Children) {
                if (x == Grid.GetColumn(canv) && y == Grid.GetRow(canv)) {
                    canvas.Background = brush;
                    ((Canvas)canv).Children.Add(canvas);
                    break;
                }
            }
        }

        private void canvas_MouseDown_1(object sender, MouseButtonEventArgs e) {
            //sólo "funciona" cuando es tu turno
            if (turno && partidaComenzada) {
                Canvas can = sender as Canvas;
                if (can.Tag == null) {
                    this.ultimoX = Grid.GetColumn(can);
                    this.ultimoY = Grid.GetRow(can);
                    can.Tag = new object();
                    //mandar las coordenadas al otro jugador
                    mandarMensaje("4~" + this.ultimoX.ToString() + "~" + this.ultimoY.ToString() + "~FT");
                    turno = false;
                }
                else {
                    MessageBox.Show("Ya has clickeado en esta casilla");
                }
            }
        }

        //botón jugar
        private void Button_Click_1(object sender, RoutedEventArgs e) {
            //this.grid.Visibility = Visibility.Visible;
            this.canvasBarcos.Visibility = Visibility.Collapsed;
            this.gridRival.Visibility = Visibility.Visible;
            this.ventana.Width = 750;
            if (this.check.IsChecked == false) {
                IP ip = new IP();
                ip.ShowDialog();
                try {
                if (ip.DialogResult == true) {
                    Console.WriteLine(ip.ipPropia);
                    ipAddressRival = IPAddress.Parse(ip.sIP);
                    rivalEndPoint = new IPEndPoint(ipAddressRival, Convert.ToInt16(ip.sPuerto));
                    mandarMensaje("0~" + ip.ipPropia + "~" + puertoLocal + "~" + nick + "~FT");
                    this.bJugar.IsEnabled = false;
                }
                }
                catch (Exception ex) {
                    Console.WriteLine("error de conexion");
                }
            }
            else {
                mandarMensaje("1~" + nick + "~FT");
            }
        }

        //socket cliente
        private void mandarMensaje(string mensaje) {
            try {
                Socket sender = new Socket(AddressFamily.InterNetwork,
                            SocketType.Stream, ProtocolType.Tcp);
                sender.Connect(rivalEndPoint);
                byte[] msg = Encoding.ASCII.GetBytes(mensaje);
                int bytesSent = sender.Send(msg);
                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
            }
            catch (ArgumentNullException ane) {
                MessageBox.Show(ane.Message);
            }
            catch (SocketException se) {
                MessageBox.Show(se.Message);
            }
            catch (Exception e) {
                MessageBox.Show(e.Message);
            }
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

//private void cambiar(String[] coor) {
//    foreach (FrameworkElement canv in this.gridPropio.Children) {
//        if (Convert.ToInt32(coor[0]) == Grid.GetColumn(canv) && Convert.ToInt32(coor[1]) == Grid.GetRow(canv)) {
//            ((Canvas)canv).Background = new SolidColorBrush(Colors.Red);
//            break;
//        }
//    }
//}
