using System;
using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
	public void SetPlayer(GameObject go)
	{
		this.ply = go;
	}

	private IEnumerator LoadNextTrack(AudioClip newClip)
	{
		float speed = 0.5f;
		this.queue.Stop();
		this.queue.clip = newClip;
		this.queue.volume = 0f;
		this.queue.Play();
		this.main.volume = 1f;
		while (this.main.volume != 0f)
		{
			this.main.volume -= Time.deltaTime * speed;
			this.queue.volume += Time.deltaTime * speed;
			yield return new WaitForEndOfFrame();
		}
		this.main.Stop();
		this.queue.Stop();
		this.main.clip = newClip;
		this.main.volume = 1f;
		this.queue.volume = 0f;
		this.main.Play();
		this.main.time = 1f / speed;
		yield break;
	}

	private void Update()
	{
		if (this.ply == null || TutorialManager.status)
		{
			return;
		}
		AudioClip newClip;
		if (this.ply.transform.position.y < -500f)
		{
			newClip = this.lower;
			this.level = "LOWER";
		}
		else if (this.ply.transform.position.y < 500f)
		{
			newClip = this.upper;
			this.level = "UPPER";
		}
		else
		{
			newClip = this.outside;
			this.level = "OUTSIDE";
		}
		if (this.prevLvl != this.level)
		{
			this.prevLvl = this.level;
			base.StartCoroutine(this.LoadNextTrack(newClip));
		}
	}

	public AudioSource main;

	public AudioSource queue;

	public AudioClip lower;

	public AudioClip upper;

	public AudioClip outside;

	public string level;

	private string prevLvl;

	private GameObject ply;
}
