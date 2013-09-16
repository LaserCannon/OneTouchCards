using UnityEngine;
using System.Collections;

public class DiscardDeck : CardStack
{

	
	public override void SetCardPosition(Card card, int index)
	{
		//base.SetCardPosition(card, index);
	}
	
	
	public bool Contains(Card card)
	{
		return cards.Contains(card);
	}
}
