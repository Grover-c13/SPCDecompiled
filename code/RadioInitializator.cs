using System;
using System.Collections.Generic;
using Dissonance;
using Dissonance.Audio.Playback;
using Dissonance.Integrations.UNet_HLAPI;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class RadioInitializator : NetworkBehaviour
{
	public RadioInitializator()
	{
	}

	private void Start()
	{
		this.pm = PlayerManager.singleton;
		try
		{
			this.parent = GameObject.Find("VoicechatPopups").transform;
		}
		catch
		{
		}
	}

	private void LateUpdate()
	{
		if (base.isLocalPlayer)
		{
			foreach (GameObject gameObject in this.pm.players)
			{
				if (gameObject != base.gameObject)
				{
					RadioInitializator component = gameObject.GetComponent<RadioInitializator>();
					component.radio.SetRelationship();
					string playerId = component.hlapiPlayer.PlayerId;
					VoicePlayback component2 = component.radio.mySource.GetComponent<VoicePlayback>();
					bool flag = component.radio.mySource.spatialBlend == 0f && component2.Priority != ChannelPriority.None && (component.radio.ShouldBeVisible(base.gameObject) || Intercom.host.speaker == gameObject);
					if (RadioInitializator.names.Contains(playerId))
					{
						int index = RadioInitializator.names.IndexOf(playerId);
						if (!flag)
						{
							UnityEngine.Object.Destroy(RadioInitializator.spawns[index]);
							RadioInitializator.spawns.RemoveAt(index);
							RadioInitializator.names.RemoveAt(index);
							return;
						}
						RadioInitializator.spawns[index].GetComponent<Image>().color = component.serverRoles.GetGradient()[0].Evaluate(component2.Amplitude * 3f);
						RadioInitializator.spawns[index].GetComponent<Outline>().effectColor = component.serverRoles.GetGradient()[1].Evaluate(component2.Amplitude * 3f);
					}
					else if (flag)
					{
						GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(this.prefab, this.parent);
						gameObject2.transform.localScale = Vector3.one;
						gameObject2.GetComponentInChildren<Text>().text = component.nicknameSync.myNick;
						RadioInitializator.spawns.Add(gameObject2);
						RadioInitializator.names.Add(playerId);
						return;
					}
				}
			}
		}
	}

	private void OnDestroy()
	{
		try
		{
			int index = RadioInitializator.names.IndexOf(base.GetComponent<HlapiPlayer>().PlayerId);
			UnityEngine.Object.Destroy(RadioInitializator.spawns[index]);
			RadioInitializator.spawns.RemoveAt(index);
			RadioInitializator.names.RemoveAt(index);
		}
		catch
		{
		}
	}

	static RadioInitializator()
	{
		// Note: this type is marked as 'beforefieldinit'.
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

	private PlayerManager pm;

	public ServerRoles serverRoles;

	public Radio radio;

	public HlapiPlayer hlapiPlayer;

	public NicknameSync nicknameSync;

	private Transform parent;

	public GameObject prefab;

	private static List<GameObject> spawns = new List<GameObject>();

	private static List<string> names = new List<string>();
}
