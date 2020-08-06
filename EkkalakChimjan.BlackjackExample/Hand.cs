using EkkalakChimjan.Standard52Card;
using System;
using System.Collections.Generic;

namespace EkkalakChimjan.BlackjackExample
{
    public class Hand
    {
        private Player player;
        public Player Player => player;
        public List<Card> CardList { get; private set; }
        public int Point { get; private set; }
        public uint Bet { get; set; }
        public string Name { get; set; }
        //public string FullName => string.Format("{0}'s hand \"{1}\"", player.Name, Name);
        public bool Stay { get; set; }
        public int NumberOfCards => CardList.Count;

        internal void init()
        {
            CardList.Clear();
            Point = 0;
            Bet = 0;
            Stay = false;
        }

        public delegate void Logic(Hand hand);

        public Hand(Player player)
        {
            this.player = player;
            Name = string.Format("{0}'s hand", player.Name);
            CardList = new List<Card>();
            init();
        }

        public bool isValidPoint
        {
            get => (Point >= 17 && Point <= 21);
        }

        public void AddCard(Card card, bool ShowCard = false)
        {
            Point += card.Value;
            CardList.Add(card);
            if (ShowCard)
            {
                Console.WriteLine("{0} get {1}",Name,card.ToString());
            }
        }

        

        public bool SetBet(uint money)
        {
            if (player.Money < money)
            {
                return false;
            }
            Bet += money;
            player.AddMoney((int)money * -1);
            return true;
        }

        

        static public void printStayText(string handName)
        {
            Console.WriteLine("{0} stay !",  handName);
        }

        static public void printNotStayText(string handName)
        {
            Console.WriteLine("{0} want one more cards !",  handName);
        }

        public static void AILogic(Hand hand)
        {
            if (hand.Point >= 17)
            {
                hand.Stay = true;
                Hand.printStayText(hand.Name);
            }
            else
            {
                hand.Stay = false;
                Hand.printNotStayText( hand.Name);
            }
        }

        public static void PlayerLogic(Hand hand)
        {
            Console.WriteLine(hand.ToString());
            Console.Write("  Do you want to \"stay\" for hand \"{0}\"? (y/n): ", hand.Name);
            hand.Stay = Console.ReadLine().ToLower() == "y";
            if (hand.Stay)
            {
                Hand.printStayText( hand.Name);
            }
            else
            {
                Hand.printNotStayText(hand.Name);
            }
        }

        public bool isStay(Logic logic)
        {
            logic(this);
            return Stay;
        }

        public override string ToString()
        {
            string returnText = string.Format("\n------------------ Cards on {0} -----------------\n",  Name);
            returnText += string.Format("  -Bet : {0}\n  -Cards :", Bet);
            CardList.ForEach(card => returnText += string.Format("  {0},", card.ToString()));
            returnText = returnText.TrimEnd(',');
            returnText += string.Format("\n  -Total :  {0} Points.\n", Point);
            return returnText;
        }

        public string textShowOneCard
        {
            get
            {
                string text = string.Format("\n------------------ Cards on {0} -----------------\n", Name);
                text += string.Format("  -Cards :  ");
                for (int i = 0; i < CardList.Count; i++)
                {
                    if (i == 0)
                    {
                        text += string.Format("{0}  ", CardList[i].ToString());
                    }
                    else
                    {
                        text += ",[?]  ";
                    }
                }
                return text+"\n";
            }
        }
        
        
    }
}