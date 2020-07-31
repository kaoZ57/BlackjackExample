using EkkalakChimjan.Standard52Card;
using System.Collections.Generic;

namespace EkkalakChimjan.BlackjackExample
{
    internal class Blackjack
    {
        private readonly List<Player> players;
        private Player dealer;
        private readonly Deck deck;
        private int numbersOfPlayerHandsInGame;

        public Blackjack(Player dealer, List<Player> players, Deck deck)
        {
            numbersOfPlayerHandsInGame = 0;
            this.dealer = dealer;
            this.players = players;
            this.deck = deck;
        }

        public void Play()
        {
            //แจกไพ่คนละ 2 ใบ
            for (int i = 0; i < 2; i++)
            {
                foreach (var player in players)
                {
                    for (int j = 0; j < player.NumberOfHands; j++)
                    {
                        if (i==0)
                        {
                            numbersOfPlayerHandsInGame++;
                        }
                        player.GetHand(j).AddCard(deck.Deal());
                    }
                }
                dealer.GetHand(0).AddCard(deck.Deal());
            }
            //
            System.Console.WriteLine(dealer.GetHand(0).textShowOneCard);
            //
            int countStay = 0;
            while (countStay < numbersOfPlayerHandsInGame)
            {
                foreach (var player in players)
                {
                    for (int j = 0; j < player.NumberOfHands; j++)
                    {
                        if (!player.GetHand(j).Stay)
                        {
                            bool decission = player.GetHand(j).isStay(Hand.PlayerLogic);
                            if (decission == false)
                            {
                                player.GetHand(j).AddCard(deck.Deal());
                            }
                            else
                            {
                                countStay++;
                            }
                        }
                    }
                }
            }

            while (dealer.GetHand(0).Stay == false)
            {
                if (!dealer.GetHand(0).isStay(Hand.AILogic))
                {
                    dealer.GetHand(0).AddCard(deck.Deal());
                }
            }
            //
            System.Console.WriteLine("------------------------");
            System.Console.WriteLine(dealer.GetHand(0).ToString());
            System.Console.WriteLine("------------------------");
            foreach (var player in players)
            {
                for (int i = 0; i < player.NumberOfHands; i++)
                {
                    int playerPoint = player.GetHand(i).Point;
                    int dealerPoint = dealer.GetHand(0).Point;
                    string result = ComparePoint(playerPoint, dealerPoint);
                    System.Console.WriteLine(player.GetHand(i).ToString());
                    if (result == "tie")
                    {
                        System.Console.WriteLine(player.GetHand(i).FullName + "Tie");
                    }
                    else if(result == "dealer win")
                    {
                        System.Console.WriteLine(player.GetHand(i).FullName + "Lose");
                    }
                    else
                    {
                        System.Console.WriteLine(player.GetHand(i).FullName + "Win");
                    }
                }
            }
        }
        public bool isValidPoint(int point)
        {
            if (point >=17 && point <=21)
            {
                return true;
            }
            return false;
        }
        public string ComparePoint(int playerPoint,int dealerPoint)
        {
            //tie
            if (isValidPoint(dealerPoint) && isValidPoint(playerPoint) && dealerPoint == playerPoint)
            {
                return "tie";
            }
            //dealer win
            else if (isValidPoint(dealerPoint) && isValidPoint(playerPoint) && dealerPoint > playerPoint)
            {
                return "dealer win";
            }
            //player win
            else if (isValidPoint(dealerPoint) && isValidPoint(playerPoint) && dealerPoint < playerPoint)
            {
                return "player win";
            }
            else if (isValidPoint(dealerPoint) && !isValidPoint(playerPoint))
            {
                return "dealer win";
            }
            else if (!isValidPoint(dealerPoint) && isValidPoint(playerPoint))
            {
                return "player win";
            }
            return "tie";
        }
        public void AllPlayerShowHands()
        {
            string text = dealer.GetHand(0).ToString();
            System.Console.WriteLine(text);
            foreach (var player in players)
            {
                for (int j = 0; j < player.NumberOfHands; j++)
                {
                    text = player.GetHand(j).ToString();
                    System.Console.WriteLine(text);
                }
            }
        }
    }
}