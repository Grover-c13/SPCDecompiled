using System;
using System.Collections;
using UnityEngine;

public class SoundtrackManager : MonoBehaviour
{
	private void FixedUpdate()
	{
		if (this.nooneSawTime > 140f && !this.overlayPlaying)
		{
			for (int i = 0; i < this.mainTracks.Length; i++)
			{
				this.mainTracks[i].playing = (i == 3);
				this.mainTracks[i].Update();
			}
			for (int j = 0; j < this.overlayTracks.Length; j++)
			{
				this.overlayTracks[j].playing = (this.overlayPlaying && j == this.overlayIndex);
				this.overlayTracks[j].Update();
			}
		}
		else
		{
			for (int k = 0; k < this.overlayTracks.Length; k++)
			{
				this.overlayTracks[k].playing = (this.overlayPlaying && k == this.overlayIndex);
				this.overlayTracks[k].Update();
			}
			for (int l = 0; l < this.mainTracks.Length; l++)
			{
				this.mainTracks[l].playing = (!this.overlayPlaying && l == this.mainIndex);
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
								MonoBehaviour.print("Found someone!");
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
}
