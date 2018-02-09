using System;
using Dissonance;
using Dissonance.Audio.Playback;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SpeakerIcon : MonoBehaviour
{
	private void Start()
	{
		this.img = base.GetComponentInChildren<RawImage>();
		this.ccm = base.GetComponentInParent<CharacterClassManager>();
		this.nid = base.GetComponentInParent<NetworkIdentity>();
		this.r = base.GetComponentInParent<Radio>();
		this.cr = base.GetComponent<CanvasRenderer>();
	}

	public void SetAlpha(float a)
	{
		this.cr.SetAlpha(Mathf.Clamp01(a));
	}

	private void Update()
	{
		if (this.nid.isLocalPlayer)
		{
			return;
		}
		if (this.vp == null)
		{
			if (this.r.mySource != null)
			{
				this.vp = this.r.mySource.GetComponent<VoicePlayback>();
			}
			return;
		}
		if (this.vp.Priority == ChannelPriority.None)
		{
			this.id = 0;
		}
		this.img.texture = this.sprites[this.id];
	}

	private void LateUpdate()
	{
		Class @class = null;
		if (this.ccm.curClass >= 0)
		{
			@class = this.ccm.klasy[this.ccm.curClass];
			if (this.nid.isLocalPlayer)
			{
				SpeakerIcon.iAmHuman = (@class.team != Team.SCP);
			}
		}
		if (this.cam == null)
		{
			this.cam = GameObject.Find("SpectatorCamera").transform;
		}
		else if (@class != null)
		{
			base.transform.localPosition = Vector3.up * 1.23f + Vector3.up * @class.iconHeightOffset;
			base.transform.LookAt(this.cam);
		}
	}

	private Transform cam;

	private CharacterClassManager ccm;

	private RawImage img;

	private NetworkIdentity nid;

	private CanvasRenderer cr;

	private Radio r;

	private VoicePlayback vp;

	public Texture[] sprites;

	public int id;

	public static bool iAmHuman;
}
