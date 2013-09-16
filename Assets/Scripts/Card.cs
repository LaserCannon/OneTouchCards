using UnityEngine;
using System.Collections;

public class Card : MonoBehaviour
{
	public string Name = "";
	
	public int Attack = 1;
	public int Defense = 1;
	
	public int DeckDamage = 1;
	
	public CardAbility Ability;
	
	
	public int Cost = 1;
	
	
	public TextMesh AttText = null;
	public TextMesh DefText = null;
	public TextMesh DmgText = null;
	public TextMesh SpecialText = null;
	
	
	void Start()
	{
		ResetValues();
	}
	
	public void ResetValues()
	{
		AttText.text = Attack.ToString();
		DefText.text = Defense.ToString();
		DmgText.text = "-" + DeckDamage.ToString();
		
		SpecialText.text = Ability.ToString();
	}
	
	
}
