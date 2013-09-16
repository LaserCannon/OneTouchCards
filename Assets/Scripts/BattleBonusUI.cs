using UnityEngine;
using System.Collections;

public class BattleBonusUI : MonoBehaviour
{
	public TextMesh Att = null;
	public TextMesh Def = null;
	
	public TextMesh Dmg = null;
	
	
	public TextMesh BonusTextPrefab = null;
	public ParticleSystem StatAddParticle = null;
	
	
	public Coroutine AnimateBonus(BattleBonus bonus, int newVal, Vector3 startPos, Vector3 dir)
	{
		return StartCoroutine(_AnimateBonus(bonus,newVal,startPos,dir));
	}
	
	private IEnumerator _AnimateBonus(BattleBonus bonus, int newVal, Vector3 startPos, Vector3 dir)
	{
		TextMesh text = (TextMesh)Instantiate(BonusTextPrefab);
		text.transform.position = startPos;
		text.transform.rotation = Quaternion.LookRotation(-Vector3.up,-transform.forward);
		
		text.text = bonus.Name + '\n'
				  + bonus.Type + (bonus.Value>0?"+":"-") + bonus.Value.ToString();
		Color c = new Color(1f,0.5f,0.5f,0f);
		if(bonus.Type!=BonusType.Dmg && bonus.Value>=0)
			c = new Color(0.5f,0.5f,1f,0f);
		else if(bonus.Type==BonusType.Dmg && bonus.Value<=0)
			c = new Color(0.5f,0.5f,1f,0f);
		
		float t = 0f;
		
		Vector3 targetPos = text.transform.position + dir.normalized/2f;
		
		while (t<2f)
		{
			text.transform.position = Vector3.Lerp(text.transform.position,targetPos,Time.deltaTime*5f);
			t += Time.deltaTime;
			
			c.a = Mathf.Clamp(Mathf.Abs(t-1f)+1f, 0f,0.75f);
			text.color = c;
			
			yield return null;
		}
		
		if(bonus.Type==BonusType.Att) {
			StartCoroutine(FlyParticleFromTo(text.transform.position,Att.transform.position));
			Att.text = "Att: "+newVal.ToString();
		}
		if(bonus.Type==BonusType.Def) {
			StartCoroutine(FlyParticleFromTo(text.transform.position,Def.transform.position));
			Def.text = "Def: "+newVal.ToString();
		}
		if(bonus.Type==BonusType.Dmg) {
			StartCoroutine(FlyParticleFromTo(text.transform.position,Dmg.transform.position));
			Dmg.text = "Dmg: "+newVal.ToString();
		}
		
		Destroy (text.gameObject);
	}

	private IEnumerator FlyParticleFromTo(Vector3 frm, Vector3 to)
	{
		yield break;
	}
	
}
