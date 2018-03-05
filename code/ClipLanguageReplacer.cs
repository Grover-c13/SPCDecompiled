using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ClipLanguageReplacer : MonoBehaviour
{
	public ClipLanguageReplacer()
	{
	}

	private IEnumerator Start()
	{
		AudioSource asource = base.GetComponent<AudioSource>();
		asource.clip = this.englishVersion;
		string path = TranslationReader.path + "/Custom Audio/" + asource.clip.name + ".ogg";
		if (asource.playOnAwake)
		{
			asource.Stop();
		}
		if (File.Exists(path))
		{
			WWW www = new WWW("file://" + path);
			asource.clip = www.GetAudioClip(false);
			while (asource.clip.loadState != AudioDataLoadState.Loaded)
			{
				yield return www;
			}
			asource.clip.name = Path.GetFileName(path);
		}
		if (asource.playOnAwake)
		{
			asource.Play();
		}
		yield break;
	}

	[SerializeField]
	public AudioClip englishVersion;

	private FileInfo[] soundFiles;

	private List<string> validExtensions = new List<string>
	{
		".ogg",
		".wav"
	};
}
