namespace EkkalakChimjan.Standard52Card
{
    public enum CardSuit
    {
        Club, Diamond, Heart, Spade
    }

    public class Card
    {
        private readonly int value;
        public CardSuit Suit { get; }

        public int Value
        {
            get
            {
                if (value > 10)
                {
                    return 10;
                }
                return value;
            }
        }

        public Card(int number)
        {
            if (number >= 52 || number < 0)
            {
                number = 0;
            }
            switch (number / 13)
            {
                case 0:
                    Suit = CardSuit.Club;
                    break;

                case 1:
                    Suit = CardSuit.Diamond;
                    break;

                case 2:
                    Suit = CardSuit.Heart;
                    break;

                default:
                    Suit = CardSuit.Spade;
                    break;
            }
            value = number % 13 + 1;
        }

        public override string ToString()
        {
            string text = "\u2460";
            switch (Suit)
            {
                case CardSuit.Club:
                    text = "\u2663";
                    break;

                case CardSuit.Diamond:
                    text = "\u2666";
                    break;

                case CardSuit.Heart:
                    text = "\u2665";
                    break;

                case CardSuit.Spade:
                    text = "\u2660";
                    break;

                default:
                    break;
            }
            if (value == 1)
            {
                text = "A" + text;
            }
            else if (value == 11)
            {
                text = "J" + text;
            }
            else if (value == 12)
            {
                text = "Q" + text;
            }
            else if (value == 13)
            {
                text = "K" + text;
            }
            else
            {
                text = value.ToString() + text;
            }
            return text;
        }
    }
}