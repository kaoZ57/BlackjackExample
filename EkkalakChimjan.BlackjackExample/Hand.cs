using EkkalakChimjan.Standard52Card;
using System;
using System.Collections.Generic;

namespace EkkalakChimjan.BlackjackExample
{
    internal class Hand
    {
        private Player player;
        public string line = "------------------------------";
        public List<Card> CardList { get; private set; }

        public int Bet { get; set; }
        public string Name { get; }
        public bool Stay { get; set; }

        public Hand(Player player,string name)
        {
            this.player = player;
            Stay = false;
            Name = name;
            Bet = 0;
            CardList = new List<Card>();
        }
        public bool SetBet(int money)
        {
            if (player.Money < money || money < 1)
            {
                return false;
            }
            Bet += money;
            player.AddMoney(money * -1);
            return true;
        }
        public void AddCard(Card card)
        {
            CardList.Add(card);
        }
        public int NumberOfCards => CardList.Count;

        public delegate void Logic(Hand hand);

        public static void AILogic(Hand hand)
        {
            if (hand.Point >= 17)
            {
                hand.Stay = true;
            }
            else
            {
                hand.Stay = false;
            }
        }

        public static void PlayerLogic(Hand hand)
        {
            Console.WriteLine(hand.ToString());
            Console.Write("  Do you want to \"stay\" for hand \"{0}\"? (y/n): ", hand.Name);
            hand.Stay = Console.ReadLine().ToLower() == "y";
            Console.WriteLine(hand.line);
        }

        public bool isStay(Logic logic)
        {
            logic(this);
            return Stay;
        }

        public override string ToString()
        {
            string returnText = string.Format("{0}\n Cards in {1}'s hand \"{2}\" (Bet: {3})\n",line,player.Name, Name,Bet);
            CardList.ForEach(card => returnText += string.Format("  {0}\n", card.ToString()));
            returnText += string.Format("  Total : {0} Points", Point);
            return returnText;
        }

        public int Point
        {
            get
            {
                int point = 0;
                CardList.ForEach(card => { point += card.Value; });
                return point;
            }
        }
    }
}