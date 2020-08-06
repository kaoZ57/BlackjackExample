using EkkalakChimjan.Standard52Card;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EkkalakChimjan.BlackjackExample
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
           
            Deck deck = new Deck();
            string addname;
            int addmoney;

            Blackjack gamePlay = new Blackjack("dealer", deck);

            Console.Write("Enter Name Here = ");
            addname = Console.ReadLine();

        UP:
            Console.Write("How much money is in your wallet? = ");
            string InputNum = Console.ReadLine();
            try
            {
                addmoney = Int32.Parse(InputNum);
            }
            catch (Exception)
            {
                goto UP;
            }

            Player Player1 = new Player(addname, addmoney);
            Player1.AddHand();
            gamePlay.AddPlayer(Player1);

            //gamePlay.AddPlayer(new Player("Pom"));
            gamePlay.Play();
            //gamePlay.AllPlayerShowHands();

        }
    }
}
