using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[System.Serializable]
public class Song
{
	
	
	public string Name = "";
	
	public AudioClip Intro = null;
	
	public List<AudioClip> Beats = new List<AudioClip>();

	public float BeatLength = 2f;
	
	public Material BackgroundMaterial = null;
}



public class SongManager : MonoBehaviour
{
	private static int songIndex = 0;
	public Song CurrentSong
	{
		get { return Songs[songIndex]; }
	}

	public List<Song> Songs = new List<Song>();
	
}
