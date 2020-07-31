using EkkalakChimjan.Standard52Card;
using System;
using System.Collections.Generic;

namespace EkkalakChimjan.BlackjackExample
{
    internal class Hand
    {
        private Player player;
        public List<Card> CardList { get; private set; }

        public int Bet { get; set; }
        public string Name { get; }
        public bool Stay { get; set; }

        public Hand(Player player, string name)
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

        static public void printStayText(string playerName, string handName)
        {
            Console.WriteLine("The {0}'s hand \"{0}\" stay !", playerName, handName);
        }

        static public void printNotStayText(string playerName, string handName)
        {
            Console.WriteLine("The {0}'s hand \"{1}\" want one more cards !", playerName, handName);
        }

        public static void AILogic(Hand hand)
        {
            if (hand.Point >= 17)
            {
                hand.Stay = true;
                Hand.printStayText(hand.player.Name, hand.Name);
            }
            else
            {
                hand.Stay = false;
                Hand.printNotStayText(hand.player.Name, hand.Name);
            }
        }

        public static void PlayerLogic(Hand hand)
        {
            Console.WriteLine(hand.ToString());
            Console.Write("  Do you want to \"stay\" for hand \"{0}\"? (y/n): ", hand.Name);
            hand.Stay = Console.ReadLine().ToLower() == "y";
            if (hand.Stay)
            {
                Hand.printStayText(hand.player.Name, hand.Name);
            }
            else
            {
                Hand.printNotStayText(hand.player.Name, hand.Name);
            }
        }

        public bool isStay(Logic logic)
        {
            logic(this);
            return Stay;
        }

        public override string ToString()
        {
            string line = "------------------------------";
            string returnText = string.Format("{0}\n Cards in {1}'s hand \"{2}\" (Bet: {3})\n", line, player.Name, Name, Bet);
            CardList.ForEach(card => returnText += string.Format("  {0},  ", card.ToString()));
            returnText += string.Format("\n  Total : {0} Points", Point);
            return returnText;
        }

        public void ShowOneCard()
        {
            string text = "------------------------------\n";
            text += string.Format("Cards in {0}'s hand \"{1}\"\n", player.Name, Name);
            for (int i = 0; i < CardList.Count; i++)
            {
                if (i == 0)
                {
                    text += string.Format("  {0}\n", CardList[i].ToString());
                }
                else
                {
                    text += "  [?],  \n";
                }
            }
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