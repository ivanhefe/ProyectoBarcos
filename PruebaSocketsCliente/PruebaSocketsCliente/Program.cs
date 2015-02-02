using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PruebaSocketsCliente {
    class Program {
        static void Main(string[] args) {
            StartClient();
        }


        public static void StartClient() {
            // Data buffer for incoming data.
            byte[] bytes = new byte[1024];

            // Connect to a remote device.
            try {
                // Establish the remote endpoint for the socket.
                // This example uses port 11000 on the local computer.
                IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());

                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11000);

                // Create a TCP/IP  socket.
                Socket sender = new Socket(AddressFamily.InterNetwork,
                    SocketType.Stream, ProtocolType.Tcp);

                // Connect the socket to the remote endpoint. Catch any errors.
                try {
                    

                    
                        sender.Connect(remoteEP); 
                    
                    Console.WriteLine("Socket connected to {0}",
                        sender.RemoteEndPoint.ToString());

                    // Encode the data string into a byte array.
                    byte[] msg = Encoding.ASCII.GetBytes("This is a test<EOF>");

                    // Send the data through the socket.
                    int bytesSent = sender.Send(msg);

                    // Receive the response from the remote device.
                    int bytesRec = sender.Receive(bytes);
                    Console.WriteLine("Echoed test = {0}",
                        Encoding.ASCII.GetString(bytes, 0, bytesRec));
                    sender.Shutdown(SocketShutdown.Both);
                    
                    sender.Close();
                    
                    // Release the socket.
                    //
                    //sender.Close();
                    

                }
                catch (ArgumentNullException ane) {
                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                    Console.Read();
                }
                catch (SocketException se) {
                    Console.WriteLine("SocketException : {0}", se.ToString());
                    Console.Read();
                }
                catch (Exception e) {
                    Console.WriteLine("Unexpected exception : {0}", e.ToString());
                    Console.Read();
                }

            }
            catch (Exception e) {
                Console.WriteLine(e.ToString());
                Console.Read();
            }
            Console.Read();
        }

    }
}
