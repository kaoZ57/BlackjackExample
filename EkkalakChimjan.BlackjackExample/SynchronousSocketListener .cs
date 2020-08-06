using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace EkkalakChimjan.BlackjackExample
{
    public enum MessageHeader
    {
        error, register, connected, info, copy_that, bet ,clear_screen, more_card
    }

    public class Message
    {
        public MessageHeader header;
        public string body;
    }

    public class NetworkInfo
    {
        public string IP;
        public int Port;
        public string ID => string.Format("{0}:{1}", IP, Port);
        public string Name;
    }

    // https://docs.microsoft.com/en-us/dotnet/framework/network-programming/asynchronous-server-socket-example
    public abstract class SynchronousSocketListener : NetworkInfo
    {
        protected IPAddress ipAddress { get; private set; }
        protected IPEndPoint ipEndPoint { get; private set; }

        public SynchronousSocketListener(string ip, int port, string name)
        {
            Name = name;
            try
            {
                ipAddress = IPAddress.Parse(ip);
                ipEndPoint = new IPEndPoint(ipAddress, port);
                IP = ip;
                Port = port;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public abstract bool ListeningCondition();
        public virtual void StartListening()
        {
            Socket socket = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);
            byte[] bytes = new Byte[1024];
            try
            {
                socket.Bind(ipEndPoint);
                socket.Listen(100);
                while (ListeningCondition())
                {
                    Listening(socket, bytes);
                }
                //socket.Shutdown(SocketShutdown.Both);
                //socket.Close();
                do_something_after_closed_listener();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void Listening(Socket socket, byte[] bytes)
        {
            //https://docs.microsoft.com/en-us/dotnet/framework/network-programming/synchronous-server-socket-example

            //Console.WriteLine("Start listening...");
            // Program is suspended while waiting for an incoming connection.
            Socket handler = socket.Accept();
            string data = null;

            // An incoming connection needs to be processed.
            while (true)
            {
                int bytesRec = handler.Receive(bytes);
                //data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                data += Encoding.UTF8.GetString(bytes, 0, bytesRec);
                if (data.IndexOf("<EOF>") > -1)
                {
                    break;
                }
            }
            data = data.TrimEnd("<EOF>".ToCharArray());
            Message msg = JsonConvert.DeserializeObject<Message>(data);
            do_something_after_receive_message_from_listener(msg, handler);

            handler.Shutdown(SocketShutdown.Both);
            handler.Close();
        }

        protected abstract void do_something_after_closed_listener();
        protected abstract void do_something_after_receive_message_from_listener(Message msg, Socket handler);

        public bool SendTo(NetworkInfo node, byte[] byteData)
        {
            // Data buffer for incoming data.
            byte[] bytes = new byte[1024];

            // Connect to a remote device.
            try
            {
                IPAddress ipAddress = IPAddress.Parse(node.IP);
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, node.Port);
                Socket socket = new Socket(ipAddress.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);
                

                    socket.Connect(remoteEP);
                    if (socket.Connected)
                    {
                        //string jsonString = JsonConvert.SerializeObject(message);
                        //byte[] msg = Encoding.ASCII.GetBytes(jsonString + "<EOF>");
                        int bytesSent = socket.Send(byteData);
                        int bytesRec = socket.Receive(bytes);
                        //string data = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                        string data = Encoding.UTF8.GetString(bytes, 0, bytesRec);
                        // Release the socket.
                        socket.Shutdown(SocketShutdown.Both);
                        socket.Close();

                        data = data.TrimEnd("<EOF>".ToCharArray());
                        Message receiveMsg = JsonConvert.DeserializeObject<Message>(data);
                        do_something_when_get_receive_from_send_message(node,receiveMsg);
                        return true;
                    }

            }
            catch (Exception e)
            {
                Console.WriteLine("\n{0}\n",e.ToString());
            }
            return false;
        }

        protected abstract void do_something_when_get_receive_from_send_message(NetworkInfo node, Message receiveMsg);
    
        protected byte[] CreateByteArrayOfMessage(MessageHeader header,string body)
        {
            Message msg = new Message();
            msg.header = header;
            msg.body = body;
            string json = JsonConvert.SerializeObject(msg);
            //byte[] byteData = Encoding.ASCII.GetBytes(json + "<EOF>");
            byte[] byteData = Encoding.UTF8.GetBytes(json + "<EOF>");
            return byteData;
        }
    }
}