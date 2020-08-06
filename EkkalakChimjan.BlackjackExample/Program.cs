using EkkalakChimjan.Standard52Card;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EkkalakChimjan.BlackjackExample
{
    class Program
    {
        [Obsolete]
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            //Deck deck = new Deck();
            //Blackjack gamePlay = new Blackjack("dealer", deck);
            //Player tuu = new Player("Tuu");
            //tuu.AddHand();
            //gamePlay.AddPlayer(tuu);
            //gamePlay.AddPlayer(new Player("Pom"));
            //gamePlay.Play();
            ////gamePlay.AllPlayerShowHands();



            string hostName = Dns.GetHostName(); // Retrive the Name of HOST  
            // Get the IP  
            string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();
            Random random = new Random();
            int myPort = random.Next(5001, 7000);


            Console.Write("Run as a server? (y/n): ");
            bool isServer = Console.ReadLine().ToLower() == "y";
            Console.Clear();
            if (isServer)
            {
                runAsServer(myIP,5000);
            }
            else
            {

                runAsClient(myIP, myPort);
            }

        }

        private static void runAsClient(string myIp,int myPort)
        {
            Console.WriteLine("Run as Client");
            Console.WriteLine("Your ip:port: {0}:{1}", myIp, myPort);
            Console.Write("\n What is your name ? :");
            string name = Console.ReadLine();
            name = name == "" ? string.Format("PlayerOnAddress[{0}:{1}]", myIp, myPort) : name;

            NetworkPlayer player = new NetworkPlayer(myIp, myPort, name);
            player.Start();
        }

        private static void runAsServer(string myIp, int myPort)
        {
            Console.WriteLine("Server ip:port: {0}:{1}", myIp, myPort);
            NetworkGamePlay gameplay = new NetworkGamePlay(myIp, myPort, "Dealer");
            Console.WriteLine("waiting for connection...");
            gameplay.StartListening();

        }
    }
}
