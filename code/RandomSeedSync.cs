using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using AntiFaker;
using GameConsole;
using UnityEngine;
using UnityEngine.Networking;

public class RandomSeedSync : NetworkBehaviour
{
	public RandomSeedSync()
	{
	}

	private void Start()
	{
		if (base.isLocalPlayer)
		{
			this.Networkseed = ConfigFile.GetInt("map_seed", -1);
			while (this.seed == -1)
			{
				this.Networkseed = UnityEngine.Random.Range(-999999999, 999999999);
			}
		}
		base.StartCoroutine(this.Generate());
	}

	private void SetSeed(int i)
	{
		this.Networkseed = i;
	}

	private IEnumerator Generate()
	{
		bool generated = false;
		while (!generated)
		{
			if (base.name == "Host")
			{
				GameConsole.Console console = UnityEngine.Object.FindObjectOfType<GameConsole.Console>();
				console.AddLog("Initializing generator...", new Color32(0, byte.MaxValue, 0, byte.MaxValue), false);
				ImageGenerator lcz = null;
				ImageGenerator hcz = null;
				ImageGenerator enz = null;
				foreach (ImageGenerator imageGenerator in UnityEngine.Object.FindObjectsOfType<ImageGenerator>())
				{
					if (imageGenerator.height == 0)
					{
						lcz = imageGenerator;
					}
					if (imageGenerator.height == -1000)
					{
						hcz = imageGenerator;
					}
					if (imageGenerator.height == -1001)
					{
						enz = imageGenerator;
					}
				}
				if (!TutorialManager.status)
				{
					lcz.GenerateMap(this.seed);
					hcz.GenerateMap(this.seed + 1);
					enz.GenerateMap(this.seed + 2);
					foreach (Door door in UnityEngine.Object.FindObjectsOfType<Door>())
					{
						door.UpdatePos();
					}
				}
				generated = true;
				foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("DoorButton"))
				{
					try
					{
						gameObject.GetComponent<ButtonWallAdjuster>().Adjust();
						foreach (ButtonWallAdjuster buttonWallAdjuster in gameObject.GetComponentsInChildren<ButtonWallAdjuster>())
						{
							buttonWallAdjuster.Invoke("Adjust", 4f);
						}
					}
					catch
					{
					}
				}
				foreach (Lift lift in UnityEngine.Object.FindObjectsOfType<Lift>())
				{
					foreach (Lift.Elevator elevator in lift.elevators)
					{
						elevator.SetPosition();
					}
				}
				yield return new WaitForSeconds(1f);
				console.AddLog("Spawning items...", new Color32(0, byte.MaxValue, 0, byte.MaxValue), false);
				if (base.isLocalPlayer)
				{
					base.GetComponent<HostItemSpawner>().Spawn(this.seed);
					base.GetComponent<AntiFakeCommands>().FindAllowedTeleportPositions();
				}
				console.AddLog("The scene is ready! Good luck!", new Color32(0, byte.MaxValue, 0, byte.MaxValue), false);
			}
			yield return new WaitForEndOfFrame();
		}
		yield break;
	}

	private void UNetVersion()
	{
	}

	public int Networkseed
	{
		get
		{
			return this.seed;
		}
		set
		{
			uint dirtyBit = 1u;
			if (NetworkServer.localClientActive && !base.syncVarHookGuard)
			{
				base.syncVarHookGuard = true;
				this.SetSeed(value);
				base.syncVarHookGuard = false;
			}
			base.SetSyncVar<int>(value, ref this.seed, dirtyBit);
		}
	}

	public override bool OnSerialize(NetworkWriter writer, bool forceAll)
	{
		if (forceAll)
		{
			writer.WritePackedUInt32((uint)this.seed);
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
			writer.WritePackedUInt32((uint)this.seed);
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
			this.seed = (int)reader.ReadPackedUInt32();
			return;
		}
		int num = (int)reader.ReadPackedUInt32();
		if ((num & 1) != 0)
		{
			this.SetSeed((int)reader.ReadPackedUInt32());
		}
	}

	[SyncVar(hook = "SetSeed")]
	public int seed = -1;

	[CompilerGenerated]
	private sealed class <Generate>c__Iterator0 : IEnumerator, IDisposable, IEnumerator<object>
	{
		[DebuggerHidden]
		public <Generate>c__Iterator0()
		{
		}

		public bool MoveNext()
		{
			uint num = (uint)this.$PC;
			this.$PC = -1;
			switch (num)
			{
			case 0u:
				this.<generated>__0 = false;
				goto IL_375;
			case 1u:
				this.<console>__1.AddLog("Spawning items...", new Color32(0, byte.MaxValue, 0, byte.MaxValue), false);
				if (this.$this.isLocalPlayer)
				{
					this.$this.GetComponent<HostItemSpawner>().Spawn(this.$this.seed);
					this.$this.GetComponent<AntiFakeCommands>().FindAllowedTeleportPositions();
				}
				this.<console>__1.AddLog("The scene is ready! Good luck!", new Color32(0, byte.MaxValue, 0, byte.MaxValue), false);
				break;
			case 2u:
				goto IL_375;
			default:
				return false;
			}
			IL_356:
			this.$current = new WaitForEndOfFrame();
			if (!this.$disposing)
			{
				this.$PC = 2;
			}
			return true;
			IL_375:
			if (this.<generated>__0)
			{
				this.$PC = -1;
			}
			else
			{
				if (this.$this.name == "Host")
				{
					this.<console>__1 = UnityEngine.Object.FindObjectOfType<GameConsole.Console>();
					this.<console>__1.AddLog("Initializing generator...", new Color32(0, byte.MaxValue, 0, byte.MaxValue), false);
					this.<lcz>__1 = null;
					this.<hcz>__1 = null;
					this.<enz>__1 = null;
					this.$locvar0 = UnityEngine.Object.FindObjectsOfType<ImageGenerator>();
					this.$locvar1 = 0;
					while (this.$locvar1 < this.$locvar0.Length)
					{
						ImageGenerator imageGenerator = this.$locvar0[this.$locvar1];
						if (imageGenerator.height == 0)
						{
							this.<lcz>__1 = imageGenerator;
						}
						if (imageGenerator.height == -1000)
						{
							this.<hcz>__1 = imageGenerator;
						}
						if (imageGenerator.height == -1001)
						{
							this.<enz>__1 = imageGenerator;
						}
						this.$locvar1++;
					}
					if (!TutorialManager.status)
					{
						this.<lcz>__1.GenerateMap(this.$this.seed);
						this.<hcz>__1.GenerateMap(this.$this.seed + 1);
						this.<enz>__1.GenerateMap(this.$this.seed + 2);
						foreach (Door door in UnityEngine.Object.FindObjectsOfType<Door>())
						{
							door.UpdatePos();
						}
					}
					this.<generated>__0 = true;
					this.$locvar4 = GameObject.FindGameObjectsWithTag("DoorButton");
					this.$locvar5 = 0;
					while (this.$locvar5 < this.$locvar4.Length)
					{
						GameObject gameObject = this.$locvar4[this.$locvar5];
						try
						{
							gameObject.GetComponent<ButtonWallAdjuster>().Adjust();
							foreach (ButtonWallAdjuster buttonWallAdjuster in gameObject.GetComponentsInChildren<ButtonWallAdjuster>())
							{
								buttonWallAdjuster.Invoke("Adjust", 4f);
							}
						}
						catch
						{
						}
						this.$locvar5++;
					}
					this.$locvar8 = UnityEngine.Object.FindObjectsOfType<Lift>();
					this.$locvar9 = 0;
					while (this.$locvar9 < this.$locvar8.Length)
					{
						Lift lift = this.$locvar8[this.$locvar9];
						foreach (Lift.Elevator elevator in lift.elevators)
						{
							elevator.SetPosition();
						}
						this.$locvar9++;
					}
					this.$current = new WaitForSeconds(1f);
					if (!this.$disposing)
					{
						this.$PC = 1;
					}
					return true;
				}
				goto IL_356;
			}
			return false;
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

		internal bool <generated>__0;

		internal GameConsole.Console <console>__1;

		internal ImageGenerator <lcz>__1;

		internal ImageGenerator <hcz>__1;

		internal ImageGenerator <enz>__1;

		internal ImageGenerator[] $locvar0;

		internal int $locvar1;

		internal GameObject[] $locvar4;

		internal int $locvar5;

		internal Lift[] $locvar8;

		internal int $locvar9;

		internal RandomSeedSync $this;

		internal object $current;

		internal bool $disposing;

		internal int $PC;
	}
}
