using UnityEngine;
using System.Collections;



public enum CardEffectCauseType
{
	None,
	Used,
	Discarded,
	Won,
	Lost,
	FirstStrike,
	EarlyStrike,
	DoubleDown,
}

public enum CardEffectType
{
	None,
	PlusAttack,
	PlusDefense,
	PlusDeckDamage,
	SelfDamage,
	OpponentDamage,
}

[System.Serializable]
public class CardAbility
{

	public CardEffectCauseType Cause = CardEffectCauseType.None;
	public CardEffectType Effect = CardEffectType.None;
	
	public int Value = 1;
	
	public string ToString()
	{
		if(Cause == CardEffectCauseType.None || Effect == CardEffectType.None)
			return "";
		
		string output = "";
		
		switch(Cause)
		{
		case CardEffectCauseType.Used:
			output += "Use: "; break;
		case CardEffectCauseType.Discarded:
			output += "Discard: "; break;
		case CardEffectCauseType.Won:
			output += "Win: "; break;
		case CardEffectCauseType.Lost:
			output += "Lose: "; break;
		case CardEffectCauseType.FirstStrike:
			output += "First: "; break;
		case CardEffectCauseType.EarlyStrike:
			output += "Early: ";break;
		case CardEffectCauseType.DoubleDown:
			output += "Doubled: "; break;
		}
		switch(Effect)
		{
		case CardEffectType.PlusAttack:
			output += "Att " + PlusOrMinus + Value.ToString();
			break;
		case CardEffectType.PlusDefense:
			output += "Def " + PlusOrMinus + Value.ToString();
			break;
		case CardEffectType.PlusDeckDamage:
			output += "Deck Dmg " + PlusOrMinus + Value.ToString();
			break;
		case CardEffectType.SelfDamage:
			output += "Self Dmg " + PlusOrMinus + Value.ToString();
			break;
		case CardEffectType.OpponentDamage:
			output += "Enemy Dmg " + PlusOrMinus + Value.ToString();
			break;
		}
		
		return output;
	}
	
	string PlusOrMinus
	{
		get { return Value>=0 ? "+":"-"; }
	}
	
	
}
