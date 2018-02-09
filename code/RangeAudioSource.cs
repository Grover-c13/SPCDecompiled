using System;
using UnityEngine;

public class RangeAudioSource : MonoBehaviour
{
	private void Update()
	{
		if (Camera.current != null)
		{
			this.isInRange = (Vector3.Distance(Camera.current.transform.position, base.transform.position) < (float)this.radius);
		}
		this.audioSource.volume += Time.deltaTime * this.lerpSpeed * (float)((!this.isInRange) ? -1 : 1);
		this.audioSource.volume = Mathf.Clamp01(this.audioSource.volume);
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(base.transform.position, (float)this.radius);
	}

	public int radius = 50;

	public float lerpSpeed = 1f;

	public AudioSource audioSource;

	private bool isInRange;
}
