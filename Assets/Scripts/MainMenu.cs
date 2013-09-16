using UnityEngine;
using System.Collections;





public class MainMenu : MonoBehaviour
{
	
	public AudioClip Music = null;
	public AudioClip Voice = null;
	
	public Renderer PressStart = null;
	
	public CardSelectMenu SelectionMenu1 = null;
	public CardSelectMenu SelectionMenu2 = null;
	
	public Transform Title = null;
	public ParticleSystem TitlePrt = null;
	
	
	private bool poolCreated = false;
	
	
	IEnumerator Start ()
	{
		TitlePrt.Stop();
		PressStart.enabled = false;
		
		Title.position = Title.position + Vector3.up*5f;
		
		yield return new WaitForSeconds(0.1f);
		
		ColorCard.FadeToPicture(0.5f);
		
		audio.clip = Music;
		audio.Play();
		
		StartCoroutine(MoveTitle());
		
		yield return new WaitForSeconds(1f);
		
		audio.PlayOneShot(Voice,1f);
		
		yield return new WaitForSeconds(1.75f);
		
		TitlePrt.Play();
		
		yield return ColorCard.FadeToColor(Color.white,0f);
		
		StartCoroutine(BeginFlashing());
		
		yield return ColorCard.FadeToPicture(0.5f);
	}
	
	IEnumerator MoveTitle()
	{
		Vector3 start = Title.position;
		Vector3 target = Title.position - Vector3.up*5f;
		
		float t = 0f;
		
		while(t<1f)
		{
			t += Time.deltaTime/3f;
			Title.position = Vector3.Lerp(start,target,t);
			
			yield return null;
		}
	}
	
	IEnumerator BeginFlashing()
	{
		float t = 0f;
		
		while(!Input.GetMouseButtonDown(0))
		{
			if(Mathf.FloorToInt(t)%2==0)
				PressStart.enabled = true;
			else
				PressStart.enabled = false;
			
			t+=Time.deltaTime*4f;
			
			yield return null;
		}
		
		yield return ColorCard.FadeToBlack(0.5f);
		
		Vector3 pos = Camera.main.transform.position;
		pos.x = 0f;
		Camera.main.transform.position = pos;
		yield return null;
		
		yield return ColorCard.FadeToPicture(0.5f);
		
		SelectionMenu1.CreatePool();
		
		yield return new WaitForSeconds(0.25f);
		
		SelectionMenu2.CreatePool();
		
		poolCreated = true;
	}
	
	void OnGUI()
	{
		if(DeckBuilds.P1Build.Count==0 || DeckBuilds.P2Build.Count==0)
			GUI.enabled = false;
		else
			GUI.enabled = true;
		
		if(poolCreated && !isMovedForward && GUI.Button(new Rect(Screen.width/2f-80f,Screen.height/2f-25f,160f,50f),"PLAY!!"))
		{
			isMovedForward = true;
			StartCoroutine(MoveToSong());
		}
		
		else if(isSelectingSong)
		{
			if(GUI.Button(new Rect(Screen.width/2f-80f,Screen.height/2f-25f,160f,50f),"Green Hill Zone"))
			{
				Application.LoadLevel("Game");
			}
			GUI.enabled = false;
			if(GUI.Button(new Rect(Screen.width/2f-80f,Screen.height/2f+75f,160f,50f),"Coming Soon!!"))
			{
			}
			if(GUI.Button(new Rect(Screen.width/2f-80f,Screen.height/2f+175f,160f,50f),"Coming Soon!!"))
			{
			}
		}
	}
	
	bool isMovedForward = false;
	bool isSelectingSong = false;
	
	IEnumerator MoveToSong()
	{
		yield return ColorCard.FadeToBlack(0.5f);
		
		Vector3 pos = Camera.main.transform.position;
		pos.x = 10;
		Camera.main.transform.position = pos;
		
		yield return ColorCard.FadeToPicture(0.5f);
		
		isSelectingSong = true;
	}
	
}
