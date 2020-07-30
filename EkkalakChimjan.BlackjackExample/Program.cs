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
            Deck d = new Deck();
            Player p = new Player();
            p.AddHand();
            Hand h = p.GetHand(0);
            h.SetBet(300);
            h.AddCard( d.Deal());
            bool a = h.isStay(Hand.PlayerLogic);
            Console.WriteLine(a);
            Console.WriteLine(p.ToString());
        }
    }
}
