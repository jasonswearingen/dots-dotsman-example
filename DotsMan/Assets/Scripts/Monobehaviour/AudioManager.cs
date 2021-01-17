using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using TMPro;

public class AudioManager : MonoBehaviour
{

	public static AudioManager instance;
	public AudioSource musicSource;

	public void Awake()
	{
		instance = this;
	}

	public void PlaySfxRequest(string name)
	{
		var audio = Resources.Load<AudioClip>($"SFX/{name}");
		if(audio == null)
		{
			return;
		}
		AudioSource.PlayClipAtPoint(audio, Camera.main.transform.position);
	}
	public void PlayMusicRequest(string name)
	{
		var audio = Resources.Load<AudioClip>($"SFX/{name}");
		if (audio == null)
		{
			return;
		}
		if(musicSource.clip != audio)
		{
			musicSource.clip = audio;
			musicSource.Play();
		}

	}
}
