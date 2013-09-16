using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
	public GameController Controller = null;
	
	public BattleManager Battler = null;
	public BattleBonusUI BonusUI = null;
	
	public Deck MyDeck = null;
	public DiscardDeck MyDiscard = null;
	
	public Transform BattleSpot = null;
	public Transform HandSpot = null;
	
	
	public TextMesh UtilText = null;
	public Renderer UtilTextBackground = null;
	
	
	public delegate void CardDelegate(PlayerController player, Card card);
	public event CardDelegate onCardSelected = null;
	
	
	
	
	
	private Card currentCard = null;
	
	private Card battlingCard = null;
	private Card doubledDownCard = null;
	
	
	public Card CurrentCard
	{
		get { return currentCard; }
	}
	
	public bool CardInHand
	{
		get { return currentCard!=null; }
	}
	
	public Card BattlingCard
	{
		get { return battlingCard; }
	}
	
	public bool Selected
	{
		get { return battlingCard!=null; }
	}
	
	public Card DoubledCard
	{
		get { return doubledDownCard; }
	}
	
	public bool Doubled
	{
		get { return doubledDownCard!=null; }
	}
	
	
	private PlayerController opponent = null;
	public PlayerController Opponent
	{
		get { return opponent; }
	}
	
	void Start()
	{
		if(Controller.P1==this)
			opponent = Controller.P2;
		if(Controller.P2==this)
			opponent = Controller.P1;
	}
	
	
	void Update()
	{
		foreach(Touch t in Input.touches)
		{
			if(t.phase==TouchPhase.Began)
			{
				Vector3 screenp = Camera.main.WorldToScreenPoint(BattleSpot.position);
				Vector3 screenpopp = Camera.main.WorldToScreenPoint(opponent.BattleSpot.position);
				
				if((screenp-(new Vector3(t.position.x,t.position.y,screenp.z))).magnitude < (screenpopp-(new Vector3(t.position.x,t.position.y,screenpopp.z))).magnitude)
				{
					switch(Controller.State)
					{
					case GameState.Drawing:
						PlayCurrentCard();
						break;
					case GameState.PreBattle:
						DoubleDown();
						break;
					}
				}
			}
		}
#if UNITY_EDITOR
		if(Input.GetMouseButtonDown(0))
		{
			
			Vector3 screenp = Camera.main.WorldToScreenPoint(BattleSpot.position);
			Vector3 screenpopp = Camera.main.WorldToScreenPoint(opponent.BattleSpot.position);
			
			if((screenp-Input.mousePosition).magnitude < (screenpopp-Input.mousePosition).magnitude)
			{
				switch(Controller.State)
				{
				case GameState.Drawing:
					PlayCurrentCard();
					break;
				case GameState.PreBattle:
					DoubleDown();
					break;
				}
			}
		}
#endif
	}
	
	
	public IEnumerator DisplayUtilText(string text, Color col)
	{
		UtilText.renderer.enabled = true;
		UtilTextBackground.enabled = true;
		Vector3 start = UtilText.transform.localPosition;
		start.x = 5f;
		Vector3 end = start;
		end.x = -5f;
		
		UtilTextBackground.material.color = col;
		UtilText.text = text;
		
		float t = 0f;
		
		while(t<1f)
		{
			if(t<0.4f)
				t = Mathf.MoveTowards(t,0.4f,Time.deltaTime*5f);
			else if(t>=0.6f)
				t = Mathf.MoveTowards(t,1f,Time.deltaTime*5f);
			else
				t =  Mathf.MoveTowards(t,0.6f,Time.deltaTime/5f);
			
			UtilText.transform.localPosition = Vector3.Lerp (start,end,t);
			
			yield return null;
		}
	}
	public IEnumerator ShowUtilText(string text, Color col)
	{
		UtilText.renderer.enabled = true;
		UtilTextBackground.enabled = true;
		Vector3 start = UtilText.transform.localPosition;
		start.x = 5f;
		Vector3 end = start;
		end.x = -5f;
		
		UtilTextBackground.material.color = col;
		UtilText.text = text;
		
		float t = 0f;
		
		while(t<0.5f)
		{
			t = Mathf.MoveTowards(t,0.5f,Time.deltaTime*5f);
			
			UtilText.transform.localPosition = Vector3.Lerp (start,end,t);
			
			yield return null;
		}
	}
	public void HideUtilText()
	{
		UtilText.renderer.enabled = false;
		UtilTextBackground.enabled = false;
	}
	
	
	public void DiscardAll()
	{
		DiscardCurrent();
		DiscardBattling();
		DiscardDoubled();
	}
	
	public bool DiscardCurrent()
	{
		if(currentCard==null)
			return false;
		
		Discard (currentCard);
		currentCard = null;
		
		return true;
	}
	
	public bool DiscardBattling()
	{
		if(battlingCard==null)
			return false;
		
		Discard (battlingCard);
		battlingCard = null;
		
		return true;
	}
	
	public bool DiscardDoubled()
	{
		if(doubledDownCard==null)
			return false;
		
		Discard (doubledDownCard);
		doubledDownCard = null;
		
		return true;
	}
	
	public bool Discard(Card card)
	{
		if(MyDiscard.Contains(card))
			return false;
		
		MyDiscard.AddCard(card);
		CardUtils.MoveCardTo( card,MyDiscard.TopCardPosition,Quaternion.LookRotation(Vector3.up,MyDiscard.transform.forward) );
		
		return true;
	}
	
	public bool DiscardFromDeck()
	{
		if(MyDeck.Size==0)
			return false;
		
		Card newCard = MyDeck.PullCard();
		
		Discard(newCard);
		
		return true;
	}
	
	public bool Draw()
	{
		if(MyDeck.Size==0)
			return false;
		
		currentCard = MyDeck.PullCard();
		
		CardUtils.MoveCardTo(currentCard, HandSpot.position, HandSpot.rotation);
		
		return true;
	}
	
	public bool DoubleDown()
	{
		if(MyDeck.Size==0 || doubledDownCard!=null)
			return false;
		
		Draw();
		
		doubledDownCard = currentCard;
		
		//if(doubledDownCard.Ability.Cause==CardEffectCauseType.DoubleDown)
		//{
		//	Battler.ActivateAbility(this,doubledDownCard);
		//}
		if(battlingCard.Ability.Cause==CardEffectCauseType.DoubleDown)
		{
			Battler.ActivateAbility(this,battlingCard);
		}
		
		CardUtils.MoveCardTo(currentCard, BattleSpot.position + BattleSpot.right*0.5f - Vector3.up * 0.1f, BattleSpot.rotation);
		
		Battler.AwardBonus(this,new BattleBonus("DoubleDown!",BonusType.Att,doubledDownCard.Attack),battlingCard.transform.position + battlingCard.transform.up*2f + battlingCard.transform.right);
		
		return true;
	}
	
	
	public void PlayCurrentCard()
	{
		if(!CardInHand || Selected)
			return;
		
		battlingCard = currentCard;
		
		CardUtils.MoveCardTo(currentCard, BattleSpot.position, BattleSpot.rotation);
		
		if(onCardSelected!=null)
			onCardSelected(this,battlingCard);
	}
	
	
	
	
}
