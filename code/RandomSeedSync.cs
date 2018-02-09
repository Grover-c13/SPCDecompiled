using System;
using System.Collections;
using AntiFaker;
using GameConsole;
using UnityEngine;
using UnityEngine.Networking;

public class RandomSeedSync : NetworkBehaviour
{
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
					hcz.GenerateMap(this.seed);
					enz.GenerateMap(this.seed);
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
}
