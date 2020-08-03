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
            Blackjack gamePlay = new Blackjack("dealer", deck);
            Player tuu = new Player("Tuu");
            tuu.AddHand();
            gamePlay.AddPlayer(tuu);
            gamePlay.AddPlayer(new Player("Pom"));
            gamePlay.Play();
            //gamePlay.AllPlayerShowHands();
        }
    }
}
