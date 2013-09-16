using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CardList : MonoBehaviour
{
	

	public List<Card> Cards = new List<Card>();
	
	
	
	public Card SpawnCard(int index)
	{
		return SpawnCard(Cards[index]);
	}
	
	
	
	public static Card SpawnCard(Card cardPrefab)
	{
		Card newCard = (Card)Instantiate(cardPrefab,Vector3.zero,Quaternion.identity);
		return newCard;
	}
	
}
