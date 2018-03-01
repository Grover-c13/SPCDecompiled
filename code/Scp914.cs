using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Networking;

public class Scp914 : NetworkBehaviour
{
	public Scp914()
	{
	}

	private void Awake()
	{
		Scp914.singleton = this;
	}

	private void SetStatus(int i)
	{
		this.NetworksyncStatus = i;
	}

	public void ChangeKnobStatus()
	{
		if (!this.working && this.cooldown < 0f)
		{
			this.cooldown = 0.2f;
			this.NetworksyncStatus = this.syncStatus + 1;
			if (this.syncStatus >= 5)
			{
				this.NetworksyncStatus = 0;
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
		if (this.syncStatus != (int)this.status)
		{
			this.knob.GetComponent<AudioSource>().Play();
			this.status = (Scp914.Scp914_Status)this.syncStatus;
		}
		if (this.cooldown >= 0f)
		{
			this.cooldown -= Time.deltaTime;
		}
		this.knob.transform.localRotation = Quaternion.Lerp(this.knob.transform.localRotation, Quaternion.Euler(Vector3.forward * Mathf.Lerp(-89f, 89f, (float)this.status / 4f)), Time.deltaTime * 4f);
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
					int[] array2 = this.recipes[component.id].outputs[this.syncStatus].outputs.ToArray();
					component.SetID(array2[UnityEngine.Random.Range(0, array2.Length)]);
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

	public int NetworksyncStatus
	{
		get
		{
			return this.syncStatus;
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
			base.SetSyncVar<int>(value, ref this.syncStatus, dirtyBit);
		}
	}

	public override bool OnSerialize(NetworkWriter writer, bool forceAll)
	{
		if (forceAll)
		{
			writer.WritePackedUInt32((uint)this.syncStatus);
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
			writer.WritePackedUInt32((uint)this.syncStatus);
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
			this.syncStatus = (int)reader.ReadPackedUInt32();
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
	public int syncStatus;

	public Scp914.Scp914_Status status;

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

	public enum Scp914_Status
	{
		Rough,
		Coarse,
		OneToOne,
		Fine,
		VeryFine
	}

	[CompilerGenerated]
	private sealed class <Animation>c__Iterator0 : IEnumerator, IDisposable, IEnumerator<object>
	{
		[DebuggerHidden]
		public <Animation>c__Iterator0()
		{
		}

		public bool MoveNext()
		{
			uint num = (uint)this.$PC;
			this.$PC = -1;
			switch (num)
			{
			case 0u:
				this.$this.soundSource.Play();
				this.$current = new WaitForSeconds(1f);
				if (!this.$disposing)
				{
					this.$PC = 1;
				}
				return true;
			case 1u:
				this.<t>__0 = 0f;
				break;
			case 2u:
				break;
			case 3u:
				this.$this.UpgradeItems();
				this.$current = new WaitForSeconds(5.5f);
				if (!this.$disposing)
				{
					this.$PC = 4;
				}
				return true;
			case 4u:
				goto IL_194;
			case 5u:
				goto IL_194;
			case 6u:
				this.$this.working = false;
				this.$PC = -1;
				return false;
			default:
				return false;
			}
			if (this.<t>__0 >= 1f)
			{
				this.$current = new WaitForSeconds(6.28f);
				if (!this.$disposing)
				{
					this.$PC = 3;
				}
				return true;
			}
			this.<t>__0 += Time.fixedDeltaTime * 0.85f;
			this.$this.doors.transform.localPosition = Vector3.right * Mathf.Lerp(1.74f, 0f, this.<t>__0);
			this.$current = new WaitForFixedUpdate();
			if (!this.$disposing)
			{
				this.$PC = 2;
			}
			return true;
			IL_194:
			if (this.<t>__0 <= 0f)
			{
				this.$current = new WaitForSeconds(1f);
				if (!this.$disposing)
				{
					this.$PC = 6;
				}
				return true;
			}
			this.<t>__0 -= Time.fixedDeltaTime * 0.85f;
			this.$this.SetDoorPos(this.<t>__0);
			this.$current = new WaitForFixedUpdate();
			if (!this.$disposing)
			{
				this.$PC = 5;
			}
			return true;
		}

		object IEnumerator<object>.Current
		{
			[DebuggerHidden]
			get
			{
				return this.$current;
			}
		}

		object IEnumerator.Current
		{
			[DebuggerHidden]
			get
			{
				return this.$current;
			}
		}

		[DebuggerHidden]
		public void Dispose()
		{
			this.$disposing = true;
			this.$PC = -1;
		}

		[DebuggerHidden]
		public void Reset()
		{
			throw new NotSupportedException();
		}

		internal float <t>__0;

		internal Scp914 $this;

		internal object $current;

		internal bool $disposing;

		internal int $PC;
	}
}
