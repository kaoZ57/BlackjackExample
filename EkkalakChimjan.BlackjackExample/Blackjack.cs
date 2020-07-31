
using EkkalakChimjan.Standard52Card;
using System.Collections.Generic;

namespace EkkalakChimjan.BlackjackExample
{
    class Blackjack
    {
        readonly List<Player> players;
        Deck deck;
        public Blackjack()
        {
            players = new List<Player>();
        }

        public Blackjack(List<Player> players)
        {
            this.players = players;
        }
    }
}
