using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SoundtrackManager : MonoBehaviour
{
	public SoundtrackManager()
	{
	}

	private void FixedUpdate()
	{
		bool flag = false;
		if (AlphaWarheadController.host != null)
		{
			flag = AlphaWarheadController.host.detonationInProgress;
		}
		if (this.nooneSawTime > 140f && !this.overlayPlaying)
		{
			for (int i = 0; i < this.mainTracks.Length; i++)
			{
				this.mainTracks[i].playing = (i == 3 && !flag);
				this.mainTracks[i].Update();
			}
			for (int j = 0; j < this.overlayTracks.Length; j++)
			{
				this.overlayTracks[j].playing = (this.overlayPlaying && j == this.overlayIndex && !flag);
				this.overlayTracks[j].Update();
			}
		}
		else
		{
			for (int k = 0; k < this.overlayTracks.Length; k++)
			{
				this.overlayTracks[k].playing = (this.overlayPlaying && k == this.overlayIndex && !flag);
				this.overlayTracks[k].Update();
			}
			for (int l = 0; l < this.mainTracks.Length; l++)
			{
				this.mainTracks[l].playing = (!this.overlayPlaying && l == this.mainIndex && !flag);
				this.mainTracks[l].Update();
			}
		}
	}

	private void Update()
	{
		if (this.player == null)
		{
			return;
		}
		if (this.seeSomeone)
		{
			this.nooneSawTime = 0f;
		}
		else
		{
			this.nooneSawTime += Time.deltaTime;
		}
	}

	private IEnumerator Start()
	{
		while (this.player == null)
		{
			this.player = PlayerManager.localPlayer;
			yield return new WaitForEndOfFrame();
		}
		Transform _camera = this.player.GetComponent<Scp049PlayerScript>().plyCam.transform;
		CharacterClassManager ccm = this.player.GetComponent<CharacterClassManager>();
		for (;;)
		{
			bool foundSomeone = false;
			Team team = ccm.klasy[Mathf.Clamp(ccm.curClass, 0, ccm.klasy.Length - 1)].team;
			if (team != Team.SCP && team != Team.RIP)
			{
				foreach (GameObject item in PlayerManager.singleton.players)
				{
					try
					{
						RaycastHit raycastHit;
						if (Physics.Raycast(new Ray(this.player.transform.position, (item.transform.position - _camera.position).normalized), out raycastHit, 20f, this.mask))
						{
							Transform root = raycastHit.collider.transform.root;
							if (root.tag == "Player")
							{
								int curClass = root.GetComponent<CharacterClassManager>().curClass;
								if (ccm.klasy[Mathf.Clamp(curClass, 0, ccm.klasy.Length - 1)].team != Team.SCP)
								{
									foundSomeone = true;
								}
							}
						}
					}
					catch
					{
					}
					yield return new WaitForEndOfFrame();
				}
			}
			else
			{
				foundSomeone = true;
				this.StopOverlay(0);
			}
			this.seeSomeone = foundSomeone;
			yield return new WaitForEndOfFrame();
		}
		yield break;
	}

	public void PlayOverlay(int id)
	{
		if (id != this.overlayIndex || !this.overlayPlaying)
		{
			this.overlayPlaying = true;
			this.overlayIndex = id;
			if (this.overlayTracks[id].restartOnPlay)
			{
				this.overlayTracks[id].source.Stop();
				this.overlayTracks[id].source.Play();
			}
		}
	}

	public void StopOverlay(int id)
	{
		if (this.overlayIndex == id)
		{
			this.overlayPlaying = false;
		}
	}

	private void Awake()
	{
		SoundtrackManager.singleton = this;
	}

	public SoundtrackManager.Track[] overlayTracks;

	public SoundtrackManager.Track[] mainTracks;

	public int overlayIndex;

	public int mainIndex;

	public bool overlayPlaying;

	public GameObject player;

	public LayerMask mask;

	private float nooneSawTime;

	private bool seeSomeone;

	public static SoundtrackManager singleton;

	[Serializable]
	public class Track
	{
		public Track()
		{
		}

		public void Update()
		{
			if (this.restartOnPlay && this.source.volume == 0f && this.playing)
			{
				this.source.Stop();
				this.source.Play();
			}
			this.source.volume += 0.02f * ((!this.playing) ? (-1f / this.exitFadeDuration) : (1f / this.enterFadeDuration)) * this.maxVolume;
			this.source.volume = Mathf.Clamp(this.source.volume, 0f, this.maxVolume);
		}

		public string name;

		public AudioSource source;

		public bool playing;

		public bool restartOnPlay;

		public float enterFadeDuration;

		public float exitFadeDuration;

		public float maxVolume;
	}

	[CompilerGenerated]
	private sealed class <Start>c__Iterator0 : IEnumerator, IDisposable, IEnumerator<object>
	{
		[DebuggerHidden]
		public <Start>c__Iterator0()
		{
		}

		public bool MoveNext()
		{
			uint num = (uint)this.$PC;
			this.$PC = -1;
			switch (num)
			{
			case 0u:
				break;
			case 1u:
				break;
			case 2u:
				this.$locvar1++;
				goto IL_230;
			case 3u:
				goto IL_A9;
			default:
				return false;
			}
			if (this.$this.player == null)
			{
				this.$this.player = PlayerManager.localPlayer;
				this.$current = new WaitForEndOfFrame();
				if (!this.$disposing)
				{
					this.$PC = 1;
				}
				return true;
			}
			this.<_camera>__0 = this.$this.player.GetComponent<Scp049PlayerScript>().plyCam.transform;
			this.<ccm>__0 = this.$this.player.GetComponent<CharacterClassManager>();
			IL_A9:
			this.<foundSomeone>__1 = false;
			this.<team>__1 = this.<ccm>__0.klasy[Mathf.Clamp(this.<ccm>__0.curClass, 0, this.<ccm>__0.klasy.Length - 1)].team;
			if (this.<team>__1 == Team.SCP || this.<team>__1 == Team.RIP)
			{
				this.<foundSomeone>__1 = true;
				this.$this.StopOverlay(0);
				goto IL_25B;
			}
			this.$locvar0 = PlayerManager.singleton.players;
			this.$locvar1 = 0;
			IL_230:
			if (this.$locvar1 < this.$locvar0.Length)
			{
				this.<item>__2 = this.$locvar0[this.$locvar1];
				try
				{
					RaycastHit raycastHit;
					if (Physics.Raycast(new Ray(this.$this.player.transform.position, (this.<item>__2.transform.position - this.<_camera>__0.position).normalized), out raycastHit, 20f, this.$this.mask))
					{
						Transform root = raycastHit.collider.transform.root;
						if (root.tag == "Player")
						{
							int curClass = root.GetComponent<CharacterClassManager>().curClass;
							if (this.<ccm>__0.klasy[Mathf.Clamp(curClass, 0, this.<ccm>__0.klasy.Length - 1)].team != Team.SCP)
							{
								this.<foundSomeone>__1 = true;
							}
						}
					}
				}
				catch
				{
				}
				this.$current = new WaitForEndOfFrame();
				if (!this.$disposing)
				{
					this.$PC = 2;
				}
				return true;
			}
			IL_25B:
			this.$this.seeSomeone = this.<foundSomeone>__1;
			this.$current = new WaitForEndOfFrame();
			if (!this.$disposing)
			{
				this.$PC = 3;
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

		internal Transform <_camera>__0;

		internal CharacterClassManager <ccm>__0;

		internal bool <foundSomeone>__1;

		internal Team <team>__1;

		internal GameObject[] $locvar0;

		internal int $locvar1;

		internal GameObject <item>__2;

		internal SoundtrackManager $this;

		internal object $current;

		internal bool $disposing;

		internal int $PC;
	}
}
