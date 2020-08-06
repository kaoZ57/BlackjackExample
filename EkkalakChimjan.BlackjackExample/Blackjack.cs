using EkkalakChimjan.Standard52Card;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Threading;

namespace EkkalakChimjan.BlackjackExample
{
    public class Blackjack
    {
        private readonly List<Player> players;
        private readonly List<Hand> allPlayerHands;
        private Player dealer;
        private readonly Deck deck;

        public Blackjack(string dealerName, Deck deck)
        {
            this.dealer = new Player(dealerName);
            this.players = new List<Player>();
            this.allPlayerHands = new List<Hand>();
            this.deck = deck;
        }

        public void AddPlayer(Player player)
        {
            players.Add(player);
        }

        public void initialCard()
        {
            deck.Initialize();
            for (int i = 0; i < 2; i++)
            {
                allPlayerHands.ForEach(hand => hand.AddCard(deck.Deal()));
                dealer.GetHand(0).AddCard(deck.Deal());
            }
        }

        private void moreCard()
        {
            Console.Clear();
            System.Console.WriteLine(dealer.GetHand(0).textShowOneCard);
            int countStay = 0;
            while (countStay < allPlayerHands.Count)
            {
                foreach (var hand in allPlayerHands)
                {
                    if (!hand.Stay && !hand.isStay(Hand.PlayerLogic))
                    {
                        hand.AddCard(deck.Deal(), true);
                    }
                    else
                    {
                        countStay++;
                    }
                }
            }
            while (!dealer.GetHand(0).Stay && !dealer.GetHand(0).isStay(Hand.AILogic))
            {
                dealer.GetHand(0).AddCard(deck.Deal(), true);
            }
        }

        private void battle()
        {
            Console.Clear();
            System.Console.WriteLine(dealer.GetHand(0).ToString());
            System.Console.WriteLine("------------------------\n");
            foreach (var playerHand in allPlayerHands)
            {
                if (playerHand.isValidPoint && !dealer.GetHand(0).isValidPoint)
                {
                    isWin(playerHand);
                }
                else if (!playerHand.isValidPoint && dealer.GetHand(0).isValidPoint)
                {
                    isLose(playerHand);
                }
                else if (playerHand.isValidPoint && dealer.GetHand(0).isValidPoint)
                {
                    if (playerHand.Point > dealer.GetHand(0).Point)
                    {
                        isWin(playerHand);
                    }
                    else if (playerHand.Point < dealer.GetHand(0).Point)
                    {
                        isLose(playerHand);
                    }
                    else
                    {
                        isTie(playerHand);
                    }
                }
                else
                {
                    isTie(playerHand);
                }
            }
        }

        private void isWin(Hand playerHand)
        {
            Console.WriteLine(playerHand.ToString());
            int money = (int)playerHand.Bet * 2;
            Console.WriteLine("   {0} win!!, get {1}",playerHand.Name,money);
            playerHand.Player.AddMoney(money);
            Console.WriteLine("{0}\n\n", playerHand.Player.Balance);
        }

        private void isLose(Hand playerHand)
        {
            Console.WriteLine(playerHand.ToString());
            Console.WriteLine("   {0} loses a bet of {1}", playerHand.Name, playerHand.Bet);
            Console.WriteLine("{0}\n\n",playerHand.Player.Balance);
        }

        private void isTie(Hand playerHand)
        {
            Console.WriteLine(playerHand.ToString());
            Console.WriteLine("   {0} tie!!, get {1}", playerHand.Name, playerHand.Bet);
            playerHand.Player.AddMoney((int)playerHand.Bet);
            Console.WriteLine("{0}\n\n", playerHand.Player.Balance);
        }

        public virtual void Play()
        {
            start:
            if (initialAllPlayersHand())
            {
                initialCard();
                
                moreCard();
                battle();
                Console.WriteLine("Play the next round? (y/n): ");
                if (Console.ReadLine().ToLower()=="y")
                {
                    goto start;
                }
                
            }
        }

        private bool initialAllPlayersHand()
        {
            allPlayerHands.Clear();
            foreach (var player in players)
            {
                if (player.Money > 0)
                {
                    player.ResetHands();
                    allPlayerHands.AddRange(player.GetAllHand());
                }
                else{
                    Console.WriteLine("\n{0} don't have money!!", player.Name);
                    Console.WriteLine("{0} kick {1} out of the game.\n", dealer.Name, player.Name);
                }
            }
            if (allPlayerHands.Count < 1)
            {
                Console.WriteLine("\nNo player in the game.\n");
                return false;
            }
            bet();
            return true;
        }

        private void bet()
        {
            Console.Clear();
            foreach (Hand hand in allPlayerHands)
            {
                bet:
                Console.WriteLine(hand.Player.Balance);
                Console.WriteLine("{0}",hand.Name);
                Console.Write("  How much do you want to bet? : ");
                try
                {
                    uint money = uint.Parse(Console.ReadLine());
                    if (!hand.SetBet(money))
                    {
                        throw new System.Exception();
                    }
                    Console.WriteLine("   {0} set bet: {1}\n\n",hand.Name,hand.Bet);
                }
                catch (Exception)
                {
                    Console.WriteLine("    invalid bet, try again");
                    goto bet;
                }
            }
        }

        public void allPlayerShowHands()
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