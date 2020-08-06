using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace EkkalakChimjan.BlackjackExample
{
    public class NetworkPlayer : SynchronousSocketListener
    {
        private NetworkInfo server;
        private bool isListening;

        public NetworkPlayer(string ip, int port, string name) : base(ip, port, name)
        {
            server = new NetworkInfo();
            isListening = true;
        }

        public void Start()
        {
            string server_ip = "";
            string server_port = "5000";
            Console.Write("Do you want to connect to the server[{0}:5000] ? (y/n)", IP);
            if (Console.ReadLine().ToLower() == "y")
            {
                server_ip = IP;
                goto connect_server;
            }
        set_server_address:
            Console.WriteLine("");
            Console.Write(" Enter server ip-address: ");
            server_ip = Console.ReadLine();
            Console.Write(" Enter server port: ");
            server_port = Console.ReadLine();
        connect_server:
            try
            {
                SetServer(server_ip, int.Parse(server_port));
                if (!ConnectToServer())
                {
                    throw new SocketException((int)SocketError.TimedOut);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(" Can't connect to the server[ {0}:{1} ]",server_ip, server_port);
                Console.WriteLine("\n{0}\n",e.ToString());
                Console.WriteLine(" Setting server address !");
                goto set_server_address;
            }
        }

        private bool ConnectToServer()
        {
            NetworkInfo info = this;
            string body = JsonConvert.SerializeObject(info);
            byte[] byteData = CreateByteArrayOfMessage(MessageHeader.register,body );
            return SendTo(server, byteData);
        }

        public void SetServer(string server_ip, int server_port)
        {
            _ = new IPEndPoint(IPAddress.Parse(server_ip), server_port);
            server.IP = server_ip;
            server.Port = server_port;
        }

        protected override void do_something_after_receive_message_from_listener(Message msg, Socket socket)
        {
            KeyValuePair<int, string> bundle;
            string input;
            byte[] byteData;
            switch (msg.header)
            {
                case MessageHeader.more_card:
                    bundle = JsonConvert.DeserializeObject<KeyValuePair<int, string>>(msg.body);
                    Console.Write(bundle.Value);
                    input = Console.ReadLine();

                    bundle = new KeyValuePair<int, string>(bundle.Key, input);
                    byteData = CreateByteArrayOfMessage(MessageHeader.more_card, JsonConvert.SerializeObject(bundle));
                    socket.Send(byteData);
                    break;

                case MessageHeader.info:
                    Console.WriteLine("\n info: {0}",msg.body);
                    Console.WriteLine(" waiting for you turn..");
                    byteData = CreateByteArrayOfMessage(MessageHeader.copy_that, "copy that");
                    socket.Send(byteData);
                    break;

                case MessageHeader.bet:
                    bundle = JsonConvert.DeserializeObject<KeyValuePair<int, string>>(msg.body);
                    Console.Write(bundle.Value);
                    input = Console.ReadLine();

                    bundle = new KeyValuePair<int, string>(bundle.Key, input);
                    byteData = CreateByteArrayOfMessage(MessageHeader.bet, JsonConvert.SerializeObject(bundle));
                    socket.Send(byteData);
                    break;

                case MessageHeader.clear_screen:
                    byteData = CreateByteArrayOfMessage(MessageHeader.copy_that, "copy that");
                    socket.Send(byteData);
                    Thread.Sleep(3000);
                    Console.Clear();
                    break;
                default:
                    Console.WriteLine("do_something_after_receive_message_from_listener\n - \"{0}\" NotImplemented",
                        msg.header.ToString());
                    throw new NotImplementedException();
            }
        }

        protected override void do_something_when_get_receive_from_send_message(NetworkInfo client,Message receiveMsg)
        {
            switch (receiveMsg.header)
            {
                case MessageHeader.connected:
                    NetworkInfo[] clientList = JsonConvert.DeserializeObject<NetworkInfo[]>(receiveMsg.body);
                    foreach (var node in clientList)
                    {
                        Console.WriteLine(" player \"{0}\" joined the game.", node.Name);
                    }
                    Console.WriteLine(" player \"{0}\"(you) joined the game.", Name);
                    Console.WriteLine(" Waiting for other player...");
                    StartListening();
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        public override bool ListeningCondition()
        {
            return isListening;
        }

        protected override void do_something_after_closed_listener()
        {
            throw new NotImplementedException();
        }
    }
}