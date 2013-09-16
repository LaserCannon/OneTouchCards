using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CardUtils : MonoBehaviour
{
	static CardUtils main = null;
	
	public static Coroutine MoveCardTo(Card card, Vector3 position, Quaternion rotation, float speed = 10f, float delay = 0f)
	{
		if(main==null)
		{
			GameObject go = new GameObject();
			main = go.AddComponent<CardUtils>();
		}
		
		return main.StartCoroutine(_MoveCardTo(card, position, rotation, speed, delay));
	}
	
	
	private static Dictionary<Card,int> cardMovementTracker = new Dictionary<Card, int>();
	private static IEnumerator _MoveCardTo(Card card, Vector3 position, Quaternion rotation, float speed = 10f, float delay = 0f)
	{
		yield return new WaitForSeconds(delay);
		
		int moveid = 0;
		
		if(!cardMovementTracker.ContainsKey(card)) {
			cardMovementTracker.Add(card,0);
		} 
		moveid = ++cardMovementTracker[card];
		
		while( moveid==cardMovementTracker[card] && (card.transform.position-position).sqrMagnitude > 0.004f )
		{
			card.transform.position = Vector3.Lerp(card.transform.position, position, Time.deltaTime * 10f);
			card.transform.rotation = Quaternion.Slerp(card.transform.rotation, rotation, Time.deltaTime * 10f);
			
			yield return null;
		}
		
		if(moveid==cardMovementTracker[card])
		{
			card.transform.position = position;
			card.transform.rotation = rotation;
		}
	}
	
	
	
}
