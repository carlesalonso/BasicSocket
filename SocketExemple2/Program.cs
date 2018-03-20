using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
namespace SocketExemple2
{
   
    class Program
    {
        
            static void Main(string[] args)
            {
            Task.Run(()=>SynchronousSocketListener.StartListening());

             SynchronousSocketClient.StartClient();
           
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();     
                

            }


    }
   
    public class SynchronousSocketClient
    {
        
        public  static void StartClient()
        {
            // Data buffer for incoming data.  
            byte[] bytes = new byte[1024];

            // Connect to a remote device.  
            try
            {
                // Establish the remote endpoint for the socket.  
                // This example uses port 11000 on the local computer.  
                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11000);

                // Create a TCP/IP  socket.  
                Socket sender = new Socket(ipAddress.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);

                // Connect the socket to the remote endpoint. Catch any errors.  
                try
                {
                    sender.Connect(remoteEP);

                    Console.WriteLine("Socket connected to {0}",
                        sender.RemoteEndPoint.ToString());
                    string message = null;
                    do
                    {
                        Console.Write("<client>Enter message: ");
                        message = Console.ReadLine();
                        byte[] msg = Encoding.ASCII.GetBytes(message);

                        // Send the data through the socket.  
                        int bytesSent = sender.Send(msg);

                        // Receive the response from the remote device.  
                        int bytesRec = sender.Receive(bytes);
                        Console.WriteLine("Echoed test = {0}",
                            Encoding.ASCII.GetString(bytes, 0, bytesRec));

                    } while (!message.Equals(":Q",StringComparison.InvariantCultureIgnoreCase));
                    // Release the socket.  
                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();

                }
                catch (ArgumentNullException ane)
                {
                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                }
                catch (SocketException se)
                {
                    Console.WriteLine("SocketException : {0}", se.ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unexpected exception : {0}", e.ToString());
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }


    }







public class SynchronousSocketListener
    {

        // Incoming data from the client.  
        public static string data = null;

        public static void StartListening()
        {
            // Data buffer for incoming data.  
            byte[] bytes = new Byte[1024];

            // Establish the local endpoint for the socket.  
            // Dns.GetHostName returns the name of the   
            // host running the application.  
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);

            // Create a TCP/IP socket.  
            Socket listener = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and   
            // listen for incoming connections.  
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(10);

                // Start listening for connections.  
                while (true)
                {
                    Console.WriteLine("Waiting for a connection...");
                    // Program is suspended while waiting for an incoming connection.  
                    Socket handler = listener.Accept();
                    data = null;
                    string message = null;

                    // An incoming connection needs to be processed.  
                    do
                    {
                        bytes = new byte[1024];
                        int bytesRec = handler.Receive(bytes);
                        message = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                        Console.WriteLine("<server>Text received : {0}", message);
                        byte[] msg = Encoding.ASCII.GetBytes(message);
                        handler.Send(msg);
                    } while (!message.Equals(":Q",StringComparison.InvariantCultureIgnoreCase));

                    // Show the data on the console.  


                    // Echo the data back to the client.  

                    Console.WriteLine("Client closes connection");

                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();

                }

               

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }


            finally
            {
                listener.Close();
            }

               

        }


    }
}
