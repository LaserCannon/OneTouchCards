using UnityEngine;
using System.Collections;

public enum BonusType { Att, Def, Dmg }

[System.Serializable]
public class BattleBonus
{
	
	public string Name = "";
	public BonusType Type = BonusType.Att;
	public int Value = 1;
	
	public BattleBonus()
	{
	}
	
	public BattleBonus(string name, BonusType type, int value)
	{
		Name = name;
		Type = type;
		Value = value;
	}
	
}
