using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CardStack : MonoBehaviour
{
	public float CardWidth = 0.01f;
	
	protected Stack<Card> cards = new Stack<Card>();
	
	
	public int Size
	{
		get { return cards.Count; }
	}
	
	public Vector3 TopCardPosition
	{
		get { return transform.position + Vector3.up*CardWidth*Size; }
	}
	public Vector3 NextTopCardPosition
	{
		get { return transform.position + Vector3.up*CardWidth*(Size+1f); }
	}
	
	public Vector3 GetCardPosition(int index)
	{
		return transform.position + Vector3.up*CardWidth*(index);
	}
	
	
	public void AddCard(Card card)
	{
		if(card!=null)
		{
			cards.Push(card);
			SetCardPosition(card,cards.Count-1);
		}
	}
	
	
	public Card PullCard()
	{
		if(cards.Count>0)
		{
			return cards.Pop();
		}
		return null;
	}
	
	
	public void Clear()
	{
		foreach(Card c in cards)
		{
			Destroy (c.gameObject);
		}
		
		cards.Clear();
	}
	
	
	public virtual void Shuffle()
	{
		List<Card> cardList = new List<Card>(cards);
		Stack<Card> newStack = new Stack<Card>();
		
		while(cardList.Count>0)
		{
			int pullInd = Random.Range(0,cardList.Count);
			Card addCard = cardList[pullInd];
			cardList.RemoveAt(pullInd);
			
			newStack.Push(addCard);
			SetCardPosition(addCard,newStack.Count-1);
		}
		
		cards = newStack;
	}
	
	
	
	public virtual void SetCardPosition(Card card, int index)
	{
		card.transform.position = GetCardPosition(index);
		card.transform.rotation = Quaternion.LookRotation(Vector3.down,Vector3.forward);
	}
}
