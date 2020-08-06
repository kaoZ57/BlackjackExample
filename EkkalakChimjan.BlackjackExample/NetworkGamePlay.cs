using EkkalakChimjan.Standard52Card;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Sockets;
using System.Text;

namespace EkkalakChimjan.BlackjackExample
{
    class NetworkGamePlay : SynchronousSocketListener 
    {
        Dictionary<NetworkInfo, Player> clientList;
        List< Hand> allPlayersHand;

        Player dealer;
        int numbersOfPlayer;
        int startMoney;
        int countPlayersStay;
        Deck deck;

        public NetworkGamePlay(string ip, int port, string name) : base(ip, port, name)
        {
            deck = new Deck();
            dealer = new Player(name);
            //clientList = new List<KeyValuePair<NetworkInfo, Player>>();
            clientList = new Dictionary<NetworkInfo, Player>();
            allPlayersHand = new List<Hand>();
            settingServer();
        }

        private void allPlayerClearScreen()
        {
            byte[] byteData = CreateByteArrayOfMessage(MessageHeader.clear_screen, "");
            sendMessageToAllPlayer(byteData);
        }

        private void playBlackjack()
        {
            deck.Initialize();
            allPlayersHand.Clear();
            countPlayersStay = 0;
            if (initialAllPlayersHand())
            {
                announceToAllPlayer("All thing are ready, Game Start !!\n");
                allPlayerClearScreen();

                askAllPlayerForTheBet();

                initialCard();
                allPlayerClearScreen();

                sendHandInfomationToAllPlayer();

                askAllPlayerForGetMoreCard();

                allPlayerClearScreen();
                battle();

                announceAllPlayerToNextRound();
            }
        }

        private void announceAllPlayerToNextRound()
        {
            Console.WriteLine("Error!!\nNotImplemented :: \"announceAllPlayerToNextRound\" method\n\n");
            throw new NotImplementedException();
        }

        private void battle()
        {
            
            Hand dealerHand = dealer.GetHand(0);

            announceToAllPlayer(dealerHand.ToString());

            for (int index = 0; index < allPlayersHand.Count; index++)
            {
                Hand playerHand = allPlayersHand[index];
                NetworkInfo networkInfo = getPlayerNetworkInfo(playerHand.Player);

                if (playerHand.isValidPoint && !dealerHand.isValidPoint)
                {
                    isWin(playerHand, networkInfo);
                }
                else if (!playerHand.isValidPoint && dealerHand.isValidPoint)
                {
                    isLose(playerHand, networkInfo);
                }
                else if (playerHand.isValidPoint && dealerHand.isValidPoint)
                {
                    if (playerHand.Point > dealerHand.Point)
                    {
                        isWin(playerHand, networkInfo);
                    }
                    else if (playerHand.Point < dealerHand.Point)
                    {
                        isLose(playerHand, networkInfo);
                    }
                    else
                    {
                        isTie(playerHand, networkInfo);
                    }
                }
                else
                {
                    isTie(playerHand, networkInfo);
                }
            }
        }

        private void isTie(Hand playerHand, NetworkInfo networkInfo)
        {
            Console.WriteLine(playerHand.ToString());
            Console.WriteLine("   {0} tie!!, get {1}", playerHand.Name, playerHand.Bet);
            Console.WriteLine("{0}\n\n", playerHand.Player.Balance);

            string announce = playerHand.ToString();
            announceToPlayer(announce, networkInfo);

            announce = string.Format("\n ========= {0} vs Dealer !! =========\n  Tie!!, {0} get back {1} coins.", playerHand.Name, playerHand.Bet);
            playerHand.Player.AddMoney((int)playerHand.Bet);
            announceToAllPlayer(announce, networkInfo);

            announce += string.Format("\n {0}",playerHand.Player.ToString());
            announceToPlayer(announce, networkInfo);
        }

        private void isLose(Hand playerHand, NetworkInfo networkInfo)
        {
            Console.WriteLine(playerHand.ToString());
            Console.WriteLine("   {0} loses a bet of {1}", playerHand.Name, playerHand.Bet);
            Console.WriteLine("{0}\n\n", playerHand.Player.Balance);

            string announce = playerHand.ToString();
            announceToPlayer(announce, networkInfo);

            announce = string.Format("\n ========= {0} vs Dealer !! =========\n  Player {0} defeat!!. lose {1} coins", playerHand.Name, playerHand.Bet);
            announceToAllPlayer(announce, networkInfo);

            announce += string.Format("\n {0}", playerHand.Player.ToString());
            announceToPlayer(announce, networkInfo);
        }

        private void isWin(Hand playerHand, NetworkInfo networkInfo)
        {
            Console.WriteLine(playerHand.ToString());
            int money = (int)playerHand.Bet * 2;
            Console.WriteLine("   {0} win!!, get {1}", playerHand.Name, money);
            playerHand.Player.AddMoney(money);
            Console.WriteLine("{0}\n\n", playerHand.Player.Balance);

            string announce = playerHand.ToString();
            announceToPlayer(announce, networkInfo);

            announce = string.Format("\n ========= {0} vs Dealer !! =========\n  Player {0} Win!!. get {1} coins", playerHand.Name, money);
            announceToAllPlayer(announce, networkInfo);

            announce += string.Format("\n {0}", playerHand.Player.ToString());
            announceToPlayer(announce, networkInfo);
        }

        private void settingServer()
        {
            setting:
            try
            {
                Console.Write("Numbers of player: ");
                numbersOfPlayer = int.Parse(Console.ReadLine());
                Console.Write("Money of players : ");
                startMoney = int.Parse(Console.ReadLine());
            }
            catch (Exception)
            {
                Console.WriteLine("invalid value!!");
                goto setting;
            }
        }

        protected override void do_something_after_receive_message_from_listener(Message msg, Socket handler)
        {
            switch (msg.header)
            {
                case MessageHeader.register:
                    NetworkInfo client = JsonConvert.DeserializeObject<NetworkInfo>(msg.body);
                    Console.WriteLine(" player \"{0}\" connected to server", client.Name);                    
                    string body = JsonConvert.SerializeObject(clientList.Keys.ToArray());
                    byte[] byteData = CreateByteArrayOfMessage(MessageHeader.connected,body);
                    handler.Send(byteData);

                    string anounce = string.Format(" player \"{0}\" joined the game.", client.Name);
                    announceToAllPlayer(anounce);

                    clientList.Add(client, new Player(client.Name, startMoney));
                    Console.WriteLine(" Numbers of player in the game: {0}",clientList.Count);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }


        protected override void do_something_when_get_receive_from_send_message(NetworkInfo client,Message msg)
        {
            KeyValuePair<int, string> bundle;
            string playerInput = string.Empty;
            string announce = string.Empty;
            Hand hand;
            switch (msg.header)
            {
                case MessageHeader.copy_that:
                    break;

                case MessageHeader.more_card:
                    bundle = JsonConvert.DeserializeObject<KeyValuePair<int, string>>(msg.body);
                    playerInput = bundle.Value;
                    hand = allPlayersHand[bundle.Key];
                    hand.Stay = playerInput.ToLower() == "y";
                    if (hand.Stay)
                    {
                        countPlayersStay++;
                        announce = string.Format(" {0} stay !", hand.Name);
                        announceToAllPlayer(announce, client);
                    }
                    else
                    {
                        announce = string.Format(" {0} want one more cards !", hand.Name);
                        announceToAllPlayer(announce, client);
                        addCardToHand(hand);
                    }
                    break;

                case MessageHeader.bet:
                    bundle = JsonConvert.DeserializeObject<KeyValuePair<int, string>>(msg.body);
                    playerInput = bundle.Value;
                    hand = allPlayersHand[bundle.Key];
                    
                    try
                    {
                        
                        uint bet = uint.Parse(playerInput);
                        if (!hand.SetBet(bet))
                        {
                            
                            throw new Exception();
                        }
                        announce = string.Format("   {0} set bet: {1}", hand.Name, hand.Bet);
                        announceToAllPlayer(announce);
                    }
                    catch (Exception)
                    {
                        announce = string.Format("   {0} set bet: {1} [it is invalid value..dumbass!!]", hand.Name, playerInput);
                        announceToAllPlayer(announce, client);
                        string error = " Invalid bet!!";
                        askPlayerForTheBet(bundle.Key,error);
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void announceToAllPlayer(string announce,NetworkInfo except = null)
        {
            byte[] byteData = CreateByteArrayOfMessage(MessageHeader.info, announce);
            foreach (var item in clientList)
            {
                NetworkInfo networkInfo = item.Key;
                if (networkInfo.Equals(except))
                {
                    continue;
                }
                Console.WriteLine(" sending info to: {0}", networkInfo.ID);
                SendTo(networkInfo, byteData);
            }
        }
        private void announceToPlayer(string announce, NetworkInfo player)
        {
            byte[] byteData = CreateByteArrayOfMessage(MessageHeader.info, announce);

                Console.WriteLine(" sending info to: {0}", player.ID);
                SendTo(player, byteData);
        }

        private void sendMessageToAllPlayer(byte[] byteData)
        {
            foreach (var item in clientList)
            {
                SendTo(item.Key, byteData);
            }
        }

        public override bool ListeningCondition()
        {
            bool condition = numbersOfPlayer > clientList.Count;
            Console.WriteLine("ListeningCondition numbersOfPlayer > clientList.Count [{0} > {1}] = {2}",
                numbersOfPlayer, clientList.Count, condition);
            return condition;
        }

        protected override void do_something_after_closed_listener()
        {
            Console.WriteLine(" Game Start..");
            playBlackjack();
        }

        

        private void sendHandInfomationToAllPlayer()
        {
            foreach (var hand in allPlayersHand)
            {
                NetworkInfo networkInfo = getPlayerNetworkInfo(hand.Player);
                string infomation = dealer.GetHand(0).textShowOneCard;
                infomation += "\n" + hand.ToString();
                byte[] byteData = CreateByteArrayOfMessage(MessageHeader.info, infomation);
                SendTo(networkInfo, byteData);
            }
        }

        private void askAllPlayerForGetMoreCard()
        {
            Console.WriteLine(" askAllPlayerForGetMoreCard [countPlayersStay < allPlayersHand.Count] => {0} < {1}",countPlayersStay,allPlayersHand.Count);
            while (countPlayersStay < allPlayersHand.Count)
            {
                for (int index = 0; index < allPlayersHand.Count; index++)
                {
                    Hand hand = allPlayersHand[index];
                    NetworkInfo networkInfo = getPlayerNetworkInfo(hand.Player);
                    string question = string.Format("  Do you want to \"stay\" for hand \"{0}\"? (y/n): ", hand.Name);

                    KeyValuePair<int, string> bundle = new KeyValuePair<int, string>(index, question);
                    byte[] byteData = CreateByteArrayOfMessage(MessageHeader.more_card, JsonConvert.SerializeObject(bundle));
                    SendTo(networkInfo, byteData);
                }
            }
            while (!dealer.GetHand(0).Stay && !dealer.GetHand(0).isStay(Hand.AILogic))
            {
                addCardToDealerHand(dealer.GetHand(0), false);
            }

        }

        private void askAllPlayerForTheBet()
        {
            for (int index = 0; index < allPlayersHand.Count; index++)
            {
                askPlayerForTheBet(index);
                
            }
        }

        private void askPlayerForTheBet(int index,string infomationString="")
        {
            Hand hand = allPlayersHand[index];
            NetworkInfo networkInfo = getPlayerNetworkInfo(hand.Player);

            string body = string.Format("\n{0}\n  You have {1} coins.\n  How much do you want to bet for {2}? : ",
                infomationString,hand.Player.Money, hand.Name);
            KeyValuePair<int, string> bundle = new KeyValuePair<int, string>(index, body);

            byte[] byteData = CreateByteArrayOfMessage(MessageHeader.bet, JsonConvert.SerializeObject(bundle));
            SendTo(networkInfo, byteData);
        }

        private NetworkInfo getPlayerNetworkInfo(Player player)
        {
            NetworkInfo networkInfo = clientList.FirstOrDefault(item => item.Value.Equals(player)).Key;
            return networkInfo;
        }

        private bool initialAllPlayersHand()
        {
            allPlayersHand.Clear();
            foreach (KeyValuePair<NetworkInfo, Player> client in clientList)
            {
                NetworkInfo networkInfo = client.Key;
                Player player = client.Value;
                if (player.Money > 0)
                {
                    player.ResetHands();
                    foreach (var hand in player.GetAllHand())
                    {
                        allPlayersHand.Add(hand);
                    }
                }
                else
                {
                    Console.WriteLine("\n{0} don't have money!!", player.Name);
                    Console.WriteLine("{0} kick {1} out of the game.\n", dealer.Name, player.Name);
                }
            }
            if (allPlayersHand.Count < 1)
            {
                Console.WriteLine("\nNo player in the game.\n");
                return false;
            }
            return true;
        }

        private void addCardToHand(Hand hand)
        {
            Card card = deck.Deal();
            hand.AddCard(card);
            NetworkInfo networkInfo = getPlayerNetworkInfo(hand.Player);

            string announce = string.Format(" you get [{0}] card.", card.ToString());
            announceToPlayer(announce, networkInfo);

            announce = string.Format(" {0} get [?] card.", hand.Name);
            announceToAllPlayer(announce, networkInfo);
            Console.WriteLine(" {0} get {1}", networkInfo.ID, card.ToString());

            announce = hand.ToString();
            announceToPlayer(announce, networkInfo);
        }
        private void addCardToDealerHand( Hand hand,bool showCard=false)
        {
            Card card = deck.Deal();
            string cardInfo = showCard?card.ToString():"[?]";
            string announce = string.Format(" {0} get [{1}] card.", hand.Name, cardInfo);
            announceToAllPlayer(announce);
            hand.AddCard(card);
            Console.WriteLine(" {0} get {1}", "Dealer", card.ToString());
        }

        private void initialCard()
        {
            deck.Initialize();
            for (int i = 0; i < 2; i++)
            {
                for (int index = 0; index < allPlayersHand.Count; index++)
                {
                    Hand hand = allPlayersHand[index];
                    addCardToHand(hand);
                }
                if (i == 0)
                {
                    addCardToDealerHand(dealer.GetHand(0),true);
                }
                else
                {
                    addCardToDealerHand(dealer.GetHand(0),false);
                }
            }
        }
    }
}
