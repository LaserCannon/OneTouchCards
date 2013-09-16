using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TestDeckBuilder : MonoBehaviour
{
	
	public List<Card> cards = new List<Card>();

	void Start()
	{
		if(DeckBuilds.P1Build.Count==0 || DeckBuilds.P2Build.Count==0)
		{
			for (int i=0;i<20;i++)
			{
				DeckBuilds.P1Build.Add (cards[Random.Range(0,cards.Count)]);
				DeckBuilds.P2Build.Add (cards[Random.Range(0,cards.Count)]);
			}
		}
	}
}
