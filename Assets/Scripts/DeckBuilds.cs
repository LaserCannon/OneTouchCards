using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DeckBuilds
{

	public static List<Card> P1Build = new List<Card>();
	public static List<Card> P2Build = new List<Card>();
	
	
	
	public static void BuildToDeck(List<Card> build, Deck deck)
	{
		deck.Clear();
		
		foreach(Card c in build)
		{
			deck.AddCard( CardList.SpawnCard(c) );
		}
	}
	
}
