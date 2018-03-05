using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class AlphaWarheadOutsitePanel : NetworkBehaviour
{
	public bool NetworkkeycardEntered
	{
		get
		{
			return this.keycardEntered;
		}
		set
		{
			uint dirtyBit = 1u;
			if (NetworkServer.localClientActive && !base.syncVarHookGuard)
			{
				base.syncVarHookGuard = true;
				this.SetKeycardState(value);
				base.syncVarHookGuard = false;
			}
			base.SetSyncVar<bool>(value, ref this.keycardEntered, dirtyBit);
		}
	}

	public AlphaWarheadOutsitePanel()
	{
	}

	public void SetKeycardState(bool b)
	{
		this.NetworkkeycardEntered = b;
	}

	private void Update()
	{
		if (AlphaWarheadOutsitePanel.host == null)
		{
			AlphaWarheadOutsitePanel.host = AlphaWarheadController.host;
			return;
		}
		this.buttonAnim.SetBool("isOpen", this.keycardEntered);
		base.transform.localPosition = new Vector3(0f, 0f, 9f);
		foreach (Text text in this.display)
		{
			text.text = AlphaWarheadOutsitePanel.GetTimeString();
		}
		foreach (GameObject gameObject in this.inevitable)
		{
			gameObject.SetActive(AlphaWarheadOutsitePanel.host.timeToDetonation <= 10f && AlphaWarheadOutsitePanel.host.timeToDetonation > 0f);
		}
	}

	public static string GetTimeString()
	{
		if (!AlphaWarheadOutsitePanel.nukeside.enabled && !AlphaWarheadOutsitePanel.host.detonationInProgress)
		{
			return "<size=180><color=red>DISABLED</color></size>";
		}
		if (AlphaWarheadOutsitePanel.host.detonationInProgress)
		{
			if (AlphaWarheadOutsitePanel.host.timeToDetonation == 0f)
			{
				return "<color=red><size=270>IGNITION</size></color>";
			}
			float num = ((float)AlphaWarheadController.realDetonationTime - AlphaWarheadController.alarmSource.time) * 100f;
			if (num < 0f)
			{
				num = 0f;
			}
			int i = 0;
			int num2 = 0;
			while (num >= 100f)
			{
				num -= 100f;
				i++;
			}
			while (i >= 60)
			{
				i -= 60;
				num2++;
			}
			return string.Concat(new string[]
			{
				"<color=orange><size=270>",
				num2.ToString("00").Substring(0, 2),
				":",
				i.ToString("00").Substring(0, 2),
				":",
				num.ToString("00").Substring(0, 2),
				"</size></color>"
			});
		}
		else
		{
			if (AlphaWarheadOutsitePanel.host.timeToDetonation > (float)AlphaWarheadController.realDetonationTime)
			{
				return "<color=red><size=200>RESTARTING</size></color>";
			}
			return "<color=lime><size=180>READY</size></color>";
		}
	}

	private void UNetVersion()
	{
	}

	public override bool OnSerialize(NetworkWriter writer, bool forceAll)
	{
		if (forceAll)
		{
			writer.Write(this.keycardEntered);
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
			writer.Write(this.keycardEntered);
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
			this.keycardEntered = reader.ReadBoolean();
			return;
		}
		int num = (int)reader.ReadPackedUInt32();
		if ((num & 1) != 0)
		{
			this.SetKeycardState(reader.ReadBoolean());
		}
	}

	private static AlphaWarheadController host;

	public static AlphaWarheadNukesitePanel nukeside;

	public Text[] display;

	public GameObject[] inevitable;

	public Animator buttonAnim;

	[SyncVar(hook = "SetKeycardState")]
	public bool keycardEntered;
}
