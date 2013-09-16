using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleManager : MonoBehaviour
{

	
	public delegate void MessageDelegate();
	public event MessageDelegate onBattleOver = null;
	
	
	
	private Dictionary<PlayerController,List<BattleBonus>> bonusDB = new Dictionary<PlayerController, List<BattleBonus>>();
	
	
	public void ActivateAbility(PlayerController player, Card card)
	{
		StartCoroutine(_ActivateAbility(player, card));
	}
		
	public IEnumerator _ActivateAbility(PlayerController player, Card card)
	{
		switch(card.Ability.Effect)
		{
		case CardEffectType.OpponentDamage:
			int num = card.Ability.Value;
			while(num>0)
			{
				player.Opponent.DiscardFromDeck();
				num--;
				yield return new WaitForSeconds(0.2f);
			}
			break;
		case CardEffectType.SelfDamage:
			int num2 = card.Ability.Value;
			while(num2>0)
			{
				player.DiscardFromDeck();
				num2--;
				yield return new WaitForSeconds(0.2f);
			}
			break;
		case CardEffectType.PlusAttack:
			AwardBonus(player,new BattleBonus("Boost!",BonusType.Att,card.Ability.Value),card.transform.position + card.transform.up*2f + card.transform.right);
			break;
		case CardEffectType.PlusDefense:
			AwardBonus(player,new BattleBonus("Boost!",BonusType.Def,card.Ability.Value),card.transform.position + card.transform.up*2f + card.transform.right);
			break;
		case CardEffectType.PlusDeckDamage:
			AwardBonus(player,new BattleBonus("Boost!",BonusType.Dmg,card.Ability.Value),card.transform.position + card.transform.up*2f + card.transform.right);
			break;
			
		}
	}
	
	
	public void AwardBonus(PlayerController player, BattleBonus bonus, Vector3 startPos )
	{
		if(!bonusDB.ContainsKey(player))
			bonusDB.Add(player,new List<BattleBonus>());
		
		bonusDB[player].Add (bonus);
		
		player.BonusUI.AnimateBonus(bonus,GetStatSum(player,bonus.Type),startPos, -player.BattleSpot.up);
		
		StartCoroutine(PulseAndIncreaseBattlingStat(player,bonus));
	}
	
	IEnumerator PulseAndIncreaseBattlingStat(PlayerController player, BattleBonus bonus)
	{
		float t = 0f;
		
		int newVal = GetStatSum(player,bonus.Type);
		TextMesh mesh = null;
		
		if(bonus.Type==BonusType.Att)	mesh = player.BattlingCard.AttText;
		if(bonus.Type==BonusType.Def)	mesh = player.BattlingCard.DefText;
		if(bonus.Type==BonusType.Dmg)	mesh = player.BattlingCard.DmgText;
		
		mesh.text = mesh==player.BattlingCard.DmgText ? "-"+newVal.ToString() : newVal.ToString();
		
		if(mesh!=player.BattlingCard.DmgText)
		{
			mesh.color = new Color(0.5f,0.5f,1f);
		}
		
		while(t<1f)
		{
			
			t = Mathf.MoveTowards(t,1f,Time.deltaTime*3f);
				
			float size = 1f + Mathf.Sin(Mathf.PI*t);
			
			mesh.transform.localScale = new Vector3(size,size,size);
			
			yield return null;
		}
	}
	IEnumerator PulseBattlingStat(PlayerController player, BonusType type)
	{
		float t = 0f;
		
		TextMesh mesh = null;
		
		if(type==BonusType.Att)	mesh = player.BattlingCard.AttText;
		if(type==BonusType.Def)	mesh = player.BattlingCard.DefText;
		if(type==BonusType.Dmg)	mesh = player.BattlingCard.DmgText;
		
		while(t<1f)
		{
			
			t = Mathf.MoveTowards(t,1f,Time.deltaTime*3f);
				
			float size = 1f + Mathf.Sin(Mathf.PI*t);
			
			mesh.transform.localScale = new Vector3(size,size,size);
			
			yield return null;
		}
	}
	
	
	public Coroutine DoBattle(PlayerController attacker, PlayerController defender)
	{
		return StartCoroutine(_DoBattle(attacker,defender));
	}
	
	private IEnumerator _DoBattle(PlayerController attacker, PlayerController defender)
	{
		yield return new WaitForSeconds(2f);
		
		PlayerController winner = GetWinner(attacker,defender,false);
		PlayerController loser = winner==attacker ? defender:attacker;
		if(winner==null)	loser = null;
		
		yield return new WaitForSeconds(2f);
		
		int loss = GetStatSum(loser,BonusType.Dmg);
		
		if(winner.BattlingCard.Ability.Cause==CardEffectCauseType.Won)
			ActivateAbility(winner,winner.BattlingCard);
		if(loser.BattlingCard.Ability.Cause==CardEffectCauseType.Lost)
			ActivateAbility(loser,loser.BattlingCard);
		
		attacker.DiscardAll();
		defender.DiscardAll();
		
		yield return new WaitForSeconds(1f);
		
		for(int i=0;i<loss;i++)
		{
			loser.Draw();
			loser.DiscardCurrent();
			yield return new WaitForSeconds(0.25f);
		}
		
		bonusDB.Clear();
		
		yield return new WaitForSeconds(1f);
		
		if(onBattleOver!=null)
			onBattleOver();
	}
	
	private int GetStatSum(PlayerController player, BonusType type)
	{
		int val = 0;
		if(type==BonusType.Att)	val = player.BattlingCard.Attack;
		if(type==BonusType.Def)	val = player.BattlingCard.Defense;
		if(type==BonusType.Dmg)	val = player.BattlingCard.DeckDamage;
		
		if(bonusDB.ContainsKey(player))
		{
			foreach(BattleBonus bonus in bonusDB[player])
			{
				if(type==bonus.Type)
					val += bonus.Value;
			}
		}
		
		return val;
	}
	
	
	private PlayerController GetWinner(PlayerController attacker, PlayerController defender, bool isCounter = false)
	{
		int att = GetStatSum(attacker,BonusType.Att);
		int def = GetStatSum(defender,BonusType.Def);
		
		StartCoroutine(PulseBattlingStat(attacker,BonusType.Att));
		StartCoroutine(PulseBattlingStat(defender,BonusType.Def));
		
		if(att>=def)
		{
			return attacker;
		}
		else if(isCounter)
		{
			return null;
		}
		else
		{
			return GetWinner(defender,attacker,true);
		}
	}
	
}
