using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public enum GameState
{
	None,
	Start,
	Drawing,
	PreBattle,
	Battle,
	GameOver,
}


public class GameController : MonoBehaviour
{
	public PlayerController P1 = null;
	public PlayerController P2 = null;
	
	public BattleManager BattleMgr = null;
	
	public SongManager SongMgr = null;
	
	
	private GameState state = GameState.None;
	
	private int beatIndex = -1;
	
	private bool p1Out = false;
	private bool p2Out = false;
	
	private PlayerController attacker = null;
	private PlayerController defender = null;
	
	private float lastBeat = 0f;
	
	
	public GameState State
	{
		get { return state; }
		set { OnStateChange(value);}
	}
	
	public float TimeSinceLastBeat
	{
		get { return Time.time - lastBeat; }
	}
	
	public float PercentToNextBeat
	{
		get { return TimeSinceLastBeat / SongMgr.CurrentSong.BeatLength; }
	}
	
	
	void Awake()
	{
#if !UNITY_EDITOR
		Application.targetFrameRate = 60;
#endif
	}
	
	
	IEnumerator Start()
	{
		ColorCard.FadeToBlack(0f);
		yield return null;
		ColorCard.FadeToPicture(1f);
		yield return new WaitForSeconds(2f);
		
		State = GameState.Start;
		
		StartCoroutine(DoIntroAndBegin());
		
		P1.onCardSelected += OnCardSelected;
		P2.onCardSelected += OnCardSelected;
		
		BattleMgr.onBattleOver += OnBattleEnded;
	}
	
	
	
	IEnumerator SyncLoop()
	{
		while(enabled)
		{
			OnBeat();
			yield return new WaitForSeconds(SongMgr.CurrentSong.BeatLength);
		}
	}
	
	
	void OnBeat()
	{
		lastBeat = Time.time;
		
		if(SongMgr.CurrentSong.Beats.Count>0)
		{
			beatIndex = (beatIndex+1)%SongMgr.CurrentSong.Beats.Count;
			audio.clip = SongMgr.CurrentSong.Beats[beatIndex];
			audio.Play();
		}
		
		switch(State)
		{
		case GameState.Drawing:
			TryDrawCards();
			drawsSinceLastBattle++;
			break;
		case GameState.PreBattle:
			State = GameState.Battle;
			break;
		case GameState.Battle:
			break;
		}
	}
	
	
	void OnCardSelected(PlayerController player, Card card)
	{
		
		if(player.BattlingCard.Ability.Cause==CardEffectCauseType.Used)
			BattleMgr.ActivateAbility(player, player.BattlingCard);
		
		if(PercentToNextBeat > 0.475f && PercentToNextBeat < 0.525f)
		{
			BattleMgr.AwardBonus(player,new BattleBonus("PERFECT!",BonusType.Att,2),card.transform.position + card.transform.up*2f + card.transform.right);
			BattleMgr.AwardBonus(player,new BattleBonus("PERFECT!",BonusType.Def,2),card.transform.position + card.transform.up*2f + card.transform.right);
		}
		else if(PercentToNextBeat > 0.4f && PercentToNextBeat < 0.6f)
		{
			BattleMgr.AwardBonus(player,new BattleBonus("Good!",BonusType.Att,1),card.transform.position + card.transform.up*2f + card.transform.right);
			BattleMgr.AwardBonus(player,new BattleBonus("Good!",BonusType.Def,1),card.transform.position + card.transform.up*2f + card.transform.right);
		}
		
		if(attacker==null)
		{
			attacker = player;
			if(attacker.BattlingCard.Ability.Cause==CardEffectCauseType.FirstStrike)
				BattleMgr.ActivateAbility(attacker, attacker.BattlingCard);
			
			if(drawsSinceLastBattle<=1 && attacker.BattlingCard.Ability.Cause==CardEffectCauseType.EarlyStrike)
				BattleMgr.ActivateAbility(attacker, attacker.BattlingCard);
		}
		else
		{
			defender = player;
			
			if(drawsSinceLastBattle<=1 && attacker.BattlingCard.Ability.Cause==CardEffectCauseType.EarlyStrike)
				BattleMgr.ActivateAbility(defender, defender.BattlingCard);
		}
	}
	
	
	void OnBattleEnded()
	{
		if(State!=GameState.GameOver)
			State = GameState.Drawing;
	}
	
	
	
	void OnStateChange(GameState newState)
	{
		if(newState==state)
			return;
		
		GameState oldState = state;
		
		state = newState;
		
		switch(newState)
		{
		case GameState.Start:
			StartCoroutine(P1.ShowUtilText("READY???",Color.blue));
			StartCoroutine(P2.ShowUtilText("READY???",Color.blue));
			break;
		case GameState.Drawing:
			StartCoroutine(P1.DisplayUtilText("GO!",Color.blue));
			StartCoroutine(P2.DisplayUtilText("GO!",Color.blue));
			break;
		case GameState.PreBattle:
			StartCoroutine(P1.DisplayUtilText("DOUBLE DOWN??",Color.green*0.75f));
			StartCoroutine(P2.DisplayUtilText("DOUBLE DOWN??",Color.green*0.75f));
			break;
		case GameState.Battle:
			StartCoroutine(P1.DisplayUtilText("BATTLE!",Color.red*0.75f));
			StartCoroutine(P2.DisplayUtilText("BATTLE!",Color.red*0.75f));
			BattleMgr.DoBattle(attacker,defender);
			attacker = null;
			defender = null;
			drawsSinceLastBattle = 0;
			break;
		}
		
		if(oldState==GameState.Start)	//if old state is start, start the loop
			StartCoroutine(SyncLoop());
	}
	
	
	private int drawsSinceLastBattle = 0;
	
	void TryDrawCards()
	{
		if(P1.Selected && P2.Selected)
		{
			State = GameState.PreBattle;
			return;
		}
		
		if(!P1.Selected)
		{	
			P1.DiscardCurrent();
			
			if(!P1.Draw())
			{
				p1Out = true;
			}
		}
		if(!P2.Selected)
		{
			P2.DiscardCurrent();
			
			if(!P2.Draw())
			{
				p2Out = true;
			}
		}
		
		if(p1Out || p2Out)
		{
			State = GameState.GameOver;
		}
	}
	
	
	IEnumerator DoIntroAndBegin()
	{
		audio.clip = SongMgr.CurrentSong.Intro;
		audio.Play();
		
		yield return new WaitForSeconds(1f);
		
		DeckBuilds.BuildToDeck(DeckBuilds.P1Build,P1.MyDeck);
		DeckBuilds.BuildToDeck(DeckBuilds.P2Build,P2.MyDeck);
		
		P1.MyDeck.Shuffle();
		P2.MyDeck.Shuffle();
		
		while(audio.time < audio.clip.length-0.02f)
			yield return null;
		
		State = GameState.Drawing;
	}
	
	
	
	
}
