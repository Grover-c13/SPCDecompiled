using System;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class RoundStart : NetworkBehaviour
{
	public void SetInfo(string i)
	{
		this.Networkinfo = i;
	}

	private void Awake()
	{
		RoundStart.singleton = this;
	}

	private void Update()
	{
		this.window.SetActive(this.info != string.Empty && this.info != "started");
		float num = 0f;
		float.TryParse(this.info, out num);
		num -= 1f;
		num /= 19f;
		this.loadingbar.fillAmount = Mathf.Lerp(this.loadingbar.fillAmount, num, Time.deltaTime);
		this.playersNumber.text = PlayerManager.singleton.players.Length.ToString();
	}

	private void Start()
	{
		base.GetComponent<RectTransform>().localPosition = Vector3.zero;
	}

	public void ShowButton()
	{
		this.forceButton.SetActive(true);
	}

	public void UseButton()
	{
		this.forceButton.SetActive(false);
		foreach (GameObject gameObject in PlayerManager.singleton.players)
		{
			CharacterClassManager component = gameObject.GetComponent<CharacterClassManager>();
			if (component.isLocalPlayer && gameObject.name == "Host")
			{
				component.ForceRoundStart();
			}
		}
	}

	private void UNetVersion()
	{
	}

	public string Networkinfo
	{
		get
		{
			return this.info;
		}
		set
		{
			uint dirtyBit = 1u;
			if (NetworkServer.localClientActive && !base.syncVarHookGuard)
			{
				base.syncVarHookGuard = true;
				this.SetInfo(value);
				base.syncVarHookGuard = false;
			}
			base.SetSyncVar<string>(value, ref this.info, dirtyBit);
		}
	}

	public override bool OnSerialize(NetworkWriter writer, bool forceAll)
	{
		if (forceAll)
		{
			writer.Write(this.info);
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
			writer.Write(this.info);
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
			this.info = reader.ReadString();
			return;
		}
		int num = (int)reader.ReadPackedUInt32();
		if ((num & 1) != 0)
		{
			this.SetInfo(reader.ReadString());
		}
	}

	[SyncVar(hook = "SetInfo")]
	public string info = string.Empty;

	public static RoundStart singleton;

	public GameObject window;

	public GameObject forceButton;

	public TextMeshProUGUI playersNumber;

	public Image loadingbar;
}
