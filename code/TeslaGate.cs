using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class TeslaGate : NetworkBehaviour
{
	public TeslaGate()
	{
	}

	public void Hack()
	{
		this.hack = true;
	}

	private void Update()
	{
		base.transform.localPosition = new Vector3(0f, 1.91f, 5.64f);
		base.transform.localRotation = Quaternion.Euler(new Vector3(0f, -180f, 0f));
	}

	private void Start()
	{
		base.StartCoroutine(this.IUpdate());
	}

	private IEnumerator IUpdate()
	{
		for (;;)
		{
			if (this.hack)
			{
				this.hack = false;
				this.source.PlayOneShot(this.hacksound);
				yield return new WaitForSeconds(0.2f);
				this.effect.SetActive(true);
				foreach (LightBlink lightBlink in this.blinkers)
				{
					lightBlink.disabled = false;
				}
				yield return new WaitForSeconds(4.8f);
				this.effect.SetActive(false);
				foreach (LightBlink lightBlink2 in this.blinkers)
				{
					lightBlink2.disabled = true;
				}
				yield return new WaitForSeconds(5f);
				this.trigger = false;
			}
			else if (this.trigger)
			{
				this.source.PlayOneShot(this.windup);
				yield return new WaitForSeconds(0.4f);
				this.effect.SetActive(true);
				foreach (LightBlink lightBlink3 in this.blinkers)
				{
					lightBlink3.disabled = false;
				}
				this.source.PlayOneShot(this.shock);
				yield return new WaitForSeconds(0.6f);
				this.effect.SetActive(false);
				foreach (LightBlink lightBlink4 in this.blinkers)
				{
					lightBlink4.disabled = true;
				}
				this.trigger = false;
				float f = 0f;
				while (f < 0.67f && !this.hack)
				{
					f += Time.deltaTime;
					yield return new WaitForEndOfFrame();
				}
			}
			yield return new WaitForEndOfFrame();
		}
		yield break;
	}

	public void Trigger(bool isKiller, GameObject other)
	{
		if (isKiller)
		{
			if (other.GetComponentInParent<NetworkIdentity>().isLocalPlayer)
			{
				other.GetComponent<PlayerStats>().CallCmdSelfDeduct(new PlayerStats.HitInfo((float)UnityEngine.Random.Range(700, 900), "WORLD", "TESLA"));
			}
		}
		else
		{
			this.trigger = true;
		}
	}

	private void UNetVersion()
	{
	}

	public override bool OnSerialize(NetworkWriter writer, bool forceAll)
	{
		bool result;
		return result;
	}

	public override void OnDeserialize(NetworkReader reader, bool initialState)
	{
	}

	public AudioClip windup;

	public AudioClip shock;

	public AudioClip hacksound;

	public GameObject effect;

	public AudioSource source;

	public LightBlink[] blinkers;

	private float lightIntensity;

	private bool trigger;

	private bool hack;
}
