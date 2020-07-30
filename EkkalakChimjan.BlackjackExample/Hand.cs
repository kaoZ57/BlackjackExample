using EkkalakChimjan.Standard52Card;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EkkalakChimjan.BlackjackExample
{
    class Hand
    {
		public int bet;
		public List<Card> cardList;
		public Hand()
		{
			bet = 0;
			cardList = new List<Card>();
		}

		public void AddCard(Card card)
		{
			cardList.Add(card);
		}
		public int Point
		{
			get
			{
				int point = 0;
				foreach (var card in cardList)
				{
					point += card.Value;// point = point + card.Value;
				}
				return point;
			}
		}
		public bool Stand()
		{
			if (Point > 21)
			{
				return false;
			}
			Console.WriteLine("Need more card?");
			if (Console.ReadLine() == "y")
			{
				return true;
			}
			return false;
		}
	}
}
