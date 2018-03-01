using System;
using UnityEngine;

public class MenuMusicManager : MonoBehaviour
{
	public MenuMusicManager()
	{
	}

	private void Update()
	{
		this.curState = Mathf.Lerp(this.curState, (!this.creditsHolder.activeSelf) ? 0f : 1f, this.lerpSpeed * Time.deltaTime);
		this.mainSource.volume = 1f - this.curState;
		this.creditsSource.volume = this.curState;
		if (this.creditsChanged != this.creditsHolder.activeSelf)
		{
			this.creditsChanged = this.creditsHolder.activeSelf;
			if (this.creditsChanged)
			{
				this.creditsSource.Play();
			}
		}
	}

	private float curState;

	public float lerpSpeed = 1f;

	private bool creditsChanged;

	[Space(15f)]
	public AudioSource mainSource;

	public AudioSource creditsSource;

	[Space(8f)]
	public GameObject creditsHolder;
}
