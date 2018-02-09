using System;
using UnityEngine;

public class ClipLanguageReplacer : MonoBehaviour
{
	private void Awake()
	{
		AudioSource component = base.GetComponent<AudioSource>();
		component.clip = ((!(PlayerPrefs.GetString("langver", "en") == "pl")) ? this.englishVersion : this.polishVersion);
		if (component.playOnAwake)
		{
			component.Stop();
			component.Play();
		}
	}

	[SerializeField]
	public AudioClip polishVersion;

	[SerializeField]
	public AudioClip englishVersion;
}
