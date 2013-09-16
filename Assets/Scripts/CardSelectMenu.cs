using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class CardSelectMenu : MonoBehaviour
{
	
	public CardList MasterCardList = null;
	
	public bool isP1 = true;
	
	public Transform ListCorner = null;
	public Transform BuildCorner = null;
	
	public TextMesh PointsLeft = null;
	
	
	public LayerMask CardMask;
	
	public TextMesh Cost = null;
	
	
	private const int rowWidth = 8;
	private const int maxCards = 50;
	private const int maxPoints = 200;
	
	private int currentPoints = 0;
	
	private List<Card> OneOfEach = new List<Card>();
	
	private List<Card> CurrentBuild = new List<Card>();
	
	
	
	void Awake()
	{
#if !UNITY_EDITOR
		Application.targetFrameRate = 60;
#endif
	}
	
	
	void Start()
	{
		//Invoke ("CreatePool",0.5f);
	}
	
	public void CreatePool()
	{
		
		if(isP1)
			DeckBuilds.P1Build.Clear();
		else
			DeckBuilds.P2Build.Clear();
		
		currentPoints = 0;
		
		Vector2 cardCoords = Vector2.zero;
		foreach(Card card in MasterCardList.Cards)
		{
			Vector3 startPos = new Vector3(15f,cardCoords.y*1.5f,0f);
			Vector3 targetPos = new Vector3(cardCoords.x*1.1f,-cardCoords.y*1.5f,-cardCoords.x*0.01f);
			Card newCard = (Card)Instantiate(card,ListCorner.TransformPoint(startPos),ListCorner.rotation);
			
			TextMesh cost = (TextMesh)Instantiate(Cost);
			cost.transform.parent = newCard.transform;
			cost.transform.localPosition = Vector3.up*0.5f+Vector3.right*0.35f+Vector3.forward;
			cost.transform.localRotation = Quaternion.LookRotation(-Vector3.forward);
			cost.text = newCard.Cost.ToString();
			
			OneOfEach.Add(newCard);
			
			CardUtils.MoveCardTo( newCard, ListCorner.TransformPoint(targetPos), Quaternion.LookRotation(-ListCorner.forward), 10f, 0.1f * (cardCoords.x + cardCoords.y*(float)rowWidth) );
			
			cardCoords.x++;
			if(cardCoords.x >= rowWidth)
			{
				cardCoords.x = 0;
				cardCoords.y++;
			}
		}
	}
	
	void Update()
	{
		if(Input.GetMouseButtonDown(0))
		{
			RaycastHit hit = new RaycastHit();
			if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),out hit,1000f,CardMask))
			{
				Card hitCard = hit.collider.transform.parent.gameObject.GetComponent<Card>();
				if(hitCard!=null)
				{
					if(OneOfEach.Contains(hitCard))
					{
						AddCardToBuild(hitCard);
					}
					else if(CurrentBuild.Contains(hitCard))
					{
						StartCoroutine(RemoveCardFromBuild(hitCard));
					}
					
				}
			}
		}
	}
							
	void AddCardToBuild(Card card)
	{
		if(CurrentBuild.Count >= maxCards)
		{
			return;
		}
		
		if(currentPoints + card.Cost > maxPoints)
		{
			return;
		}
		
		int index = OneOfEach.IndexOf(card);
		if(index>=0)
		{
			Card builtCard = CardList.SpawnCard(card);
			builtCard.transform.position = card.transform.position;
			builtCard.transform.rotation = card.transform.rotation;
			CardUtils.MoveCardTo(builtCard,BuildCorner.TransformPoint(new Vector3(CurrentBuild.Count*0.15f,0f,-CurrentBuild.Count*0.01f)),
										   Quaternion.LookRotation(-BuildCorner.forward));
			
			CurrentBuild.Add(builtCard);
			
			currentPoints += card.Cost;
			PointsLeft.text = (maxPoints-currentPoints).ToString();// + "/" + maxPoints.ToString();
			
			if(isP1)
				DeckBuilds.P1Build.Add(MasterCardList.Cards[index]);
			else
				DeckBuilds.P2Build.Add(MasterCardList.Cards[index]);
		}
	}
	
	IEnumerator RemoveCardFromBuild(Card card)
	{
		int index = CurrentBuild.IndexOf(card);
		if(index>=0)
		{
			if(isP1)
				DeckBuilds.P1Build.RemoveAt(index);
			else
				DeckBuilds.P2Build.RemoveAt(index);
			
			CurrentBuild.Remove(card);
			
			currentPoints -= card.Cost;
			PointsLeft.text = (maxPoints-currentPoints).ToString();// + "/" + maxPoints.ToString();
			
			yield return CardUtils.MoveCardTo(card,card.transform.position-BuildCorner.up*5f,Quaternion.LookRotation(-BuildCorner.forward));
			
			Destroy(card);
		}
			
	}
	
	
}
