using System;

namespace EkkalakChimjan.Standard52Card
{
    public class Deck
    {
		private readonly int[] cards;
		private int nextCard;
		private readonly Random random;

		public Deck()
		{
			random = new Random();
			cards = new int[52];
			Initialize();
		}

		public void Initialize()
		{
			for (int i = 0; i < cards.Length; i++)
			{
				cards[i] = i;
			}
			nextCard = cards.Length;
			Shuffle();
		}

		public bool Shuffle(int times = 60)
		{
			if (nextCard != cards.Length)
			{
				return false;
			}
			int j, k, holder;
			for (int i = 0; i < times; i++)
			{
				j = random.Next(52);
				k = random.Next(52);
				holder = cards[j];
				cards[j] = cards[k];
				cards[k] = holder;

			}
			return true;
		}

		public Card Deal()
		{
			nextCard--;
			if (nextCard < 0)
			{
				return null;
			}
			return new Card(cards[nextCard]);
		}
	}
}
