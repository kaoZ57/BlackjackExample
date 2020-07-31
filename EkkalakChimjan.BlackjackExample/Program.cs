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
            Player dealer = new Player("Dealer");
            List<Player> players = new List<Player>();
            players.Add(new Player("Tuu"));
            players.Add(new Player("Pom"));

            Deck deck = new Deck();
            Blackjack gamePlay = new Blackjack(dealer,players, deck);

            gamePlay.Play();
            //gamePlay.AllPlayerShowHands();
        }
    }
}
