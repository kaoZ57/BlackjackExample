using System.Collections.Generic;

namespace EkkalakChimjan.BlackjackExample
{
    internal class Player
    {
        private List<Hand> handList;

        public string Name { get; private set; }
        public int Money { get; private set; }

        public int NumberOfHands => handList.Count;

        public Player(string name = "Unknown", int money = 5000)
        {
            this.Name = name;
            this.Money = money;
            handList = new List<Hand>
            {
                new Hand(this,"1")
            };
        }

        public void AddMoney(int amount)
        {
            Money += amount;
        }

        public void AddHand()
        {
            string name = (handList.Count + 1).ToString();
            handList.Add(new Hand(this, name));
        }

        public Hand GetHand(int index)
        {
            if (index < 0 && index > NumberOfHands - 1)
            {
                return null;
            }
            return handList[index];
        }

        public Hand GetHandNotSetBet()
        {
            foreach (var hand in handList)
            {
                if (hand.Bet == 0)
                {
                    return hand;
                }
            }
            return null;
        }

        public override string ToString()
        {
            string returnText = string.Format("=================\n Name:{0}\n", Name);
            returnText += string.Format(" Balance: {0}\n", Money);
            returnText += string.Format(" Number of hands: {0}\n", NumberOfHands);
            returnText += "=================";
            return returnText;
        }
    }
}