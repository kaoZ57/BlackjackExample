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
            Deck d = new Deck();
            Player p = new Player("Tuu");
            p.AddHand();
            Hand h = p.GetHand(0);
            h.SetBet(300);
            h.AddCard( d.Deal());
            h.AddCard(d.Deal());

            Console.WriteLine(h.textShowOneCard);
            Console.WriteLine(h.ToString());
            bool a = h.isStay(Hand.AILogic);
            Console.WriteLine(a);
            Console.WriteLine(p.ToString());
        }
    }
}
