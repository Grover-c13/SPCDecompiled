using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Scp914 : NetworkBehaviour
{
	public int NetworkknobStatus
	{
		get
		{
			return this.knobStatus;
		}
		set
		{
			uint dirtyBit = 1u;
			if (NetworkServer.localClientActive && !base.syncVarHookGuard)
			{
				base.syncVarHookGuard = true;
				this.SetStatus(value);
				base.syncVarHookGuard = false;
			}
			base.SetSyncVar<int>(value, ref this.knobStatus, dirtyBit);
		}
	}

	public Scp914()
	{
	}

	private void Awake()
	{
		Scp914.singleton = this;
	}

	private void SetStatus(int i)
	{
		this.NetworkknobStatus = i;
	}

	public void ChangeKnobStatus()
	{
		if (!this.working && this.cooldown < 0f)
		{
			this.cooldown = 0.2f;
			this.NetworkknobStatus = this.knobStatus + 1;
			if (this.knobStatus >= 5)
			{
				this.NetworkknobStatus = 0;
			}
		}
	}

	public void StartRefining()
	{
		if (!this.working)
		{
			this.working = true;
			base.StartCoroutine(this.Animation());
		}
	}

	private void Update()
	{
		if (this.knobStatus != this.prevStatus)
		{
			this.knob.GetComponent<AudioSource>().Play();
			this.prevStatus = this.knobStatus;
		}
		if (this.cooldown >= 0f)
		{
			this.cooldown -= Time.deltaTime;
		}
		this.knob.transform.localRotation = Quaternion.Lerp(this.knob.transform.localRotation, Quaternion.Euler(Vector3.forward * Mathf.Lerp(-89f, 89f, (float)this.knobStatus / 4f)), Time.deltaTime * 4f);
	}

	private IEnumerator Animation()
	{
		this.soundSource.Play();
		yield return new WaitForSeconds(1f);
		float t = 0f;
		while (t < 1f)
		{
			t += Time.fixedDeltaTime * 0.85f;
			this.doors.transform.localPosition = Vector3.right * Mathf.Lerp(1.74f, 0f, t);
			yield return new WaitForFixedUpdate();
		}
		yield return new WaitForSeconds(6.28f);
		this.UpgradeItems();
		yield return new WaitForSeconds(5.5f);
		while (t > 0f)
		{
			t -= Time.fixedDeltaTime * 0.85f;
			this.SetDoorPos(t);
			yield return new WaitForFixedUpdate();
		}
		yield return new WaitForSeconds(1f);
		this.working = false;
		yield break;
	}

	[ServerCallback]
	private void UpgradeItems()
	{
		if (!NetworkServer.active)
		{
			return;
		}
		foreach (Collider collider in Physics.OverlapBox(this.intake_obj.position, Vector3.one * this.colliderSize / 2f))
		{
			Pickup component = collider.GetComponent<Pickup>();
			if (component != null)
			{
				component.SetPosition(component.transform.position + (this.output_obj.position - this.intake_obj.position));
				if (component.id < this.recipes.Length)
				{
					int[] array2 = this.recipes[component.id].outputs[this.knobStatus].outputs.ToArray();
					int num = array2[UnityEngine.Random.Range(0, array2.Length)];
					if (num < 0)
					{
						component.SetPosition(Vector3.down * 4000f);
						if (TutorialManager.status)
						{
							UnityEngine.Object.FindObjectOfType<TutorialManager>().Tutorial3_KeycardBurnt();
						}
					}
					else
					{
						component.SetID(num);
					}
				}
				else
				{
					component.SetID(component.id);
				}
			}
		}
	}

	private void SetDoorPos(float t)
	{
		this.doors.transform.localPosition = Vector3.right * Mathf.Lerp(1.74f, 0f, t);
	}

	private void UNetVersion()
	{
	}

	public override bool OnSerialize(NetworkWriter writer, bool forceAll)
	{
		if (forceAll)
		{
			writer.WritePackedUInt32((uint)this.knobStatus);
			return true;
		}
		bool flag = false;
		if ((base.syncVarDirtyBits & 1u) != 0u)
		{
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
				flag = true;
			}
			writer.WritePackedUInt32((uint)this.knobStatus);
		}
		if (!flag)
		{
			writer.WritePackedUInt32(base.syncVarDirtyBits);
		}
		return flag;
	}

	public override void OnDeserialize(NetworkReader reader, bool initialState)
	{
		if (initialState)
		{
			this.knobStatus = (int)reader.ReadPackedUInt32();
			return;
		}
		int num = (int)reader.ReadPackedUInt32();
		if ((num & 1) != 0)
		{
			this.SetStatus((int)reader.ReadPackedUInt32());
		}
	}

	public static Scp914 singleton;

	public Texture burntIcon;

	public AudioSource soundSource;

	public Transform doors;

	public Transform knob;

	public Transform intake_obj;

	public Transform output_obj;

	public float colliderSize;

	public Scp914.Recipe[] recipes;

	[SyncVar(hook = "SetStatus")]
	public int knobStatus;

	private int prevStatus = -1;

	private float cooldown;

	public bool working;

	[Serializable]
	public class Recipe
	{
		public Recipe()
		{
		}

		public List<Scp914.Recipe.Output> outputs = new List<Scp914.Recipe.Output>();

		[Serializable]
		public class Output
		{
			public Output()
			{
			}

			public List<int> outputs = new List<int>();
		}
	}
}
