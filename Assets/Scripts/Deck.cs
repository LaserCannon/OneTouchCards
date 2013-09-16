using UnityEngine;
using System.Collections;

public class Deck : CardStack
{
	
	
	public override void SetCardPosition(Card card, int index)
	{
		base.SetCardPosition(card, index);
		card.transform.rotation = Quaternion.LookRotation(Vector3.down,Vector3.forward);
	}
}
