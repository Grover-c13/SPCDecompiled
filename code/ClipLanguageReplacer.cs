using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ClipLanguageReplacer : MonoBehaviour
{
	public ClipLanguageReplacer()
	{
	}

	private IEnumerator Start()
	{
		AudioSource asource = base.GetComponent<AudioSource>();
		asource.clip = this.englishVersion;
		string path = TranslationReader.path + "/Custom Audio/" + asource.clip.name + ".ogg";
		if (asource.playOnAwake)
		{
			asource.Stop();
		}
		if (File.Exists(path))
		{
			WWW www = new WWW("file://" + path);
			asource.clip = www.GetAudioClip(false);
			while (asource.clip.loadState != AudioDataLoadState.Loaded)
			{
				yield return www;
			}
			asource.clip.name = Path.GetFileName(path);
		}
		if (asource.playOnAwake)
		{
			asource.Play();
		}
		yield break;
	}

	[SerializeField]
	public AudioClip englishVersion;

	private FileInfo[] soundFiles;

	private List<string> validExtensions = new List<string>
	{
		".ogg",
		".wav"
	};

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
				this.<asource>__0 = this.$this.GetComponent<AudioSource>();
				this.<asource>__0.clip = this.$this.englishVersion;
				this.<path>__0 = TranslationReader.path + "/Custom Audio/" + this.<asource>__0.clip.name + ".ogg";
				if (this.<asource>__0.playOnAwake)
				{
					this.<asource>__0.Stop();
				}
				if (!File.Exists(this.<path>__0))
				{
					goto IL_125;
				}
				this.<www>__1 = new WWW("file://" + this.<path>__0);
				this.<asource>__0.clip = this.<www>__1.GetAudioClip(false);
				break;
			case 1u:
				break;
			default:
				return false;
			}
			if (this.<asource>__0.clip.loadState != AudioDataLoadState.Loaded)
			{
				this.$current = this.<www>__1;
				if (!this.$disposing)
				{
					this.$PC = 1;
				}
				return true;
			}
			this.<asource>__0.clip.name = Path.GetFileName(this.<path>__0);
			IL_125:
			if (this.<asource>__0.playOnAwake)
			{
				this.<asource>__0.Play();
			}
			this.$PC = -1;
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

		internal AudioSource <asource>__0;

		internal string <path>__0;

		internal WWW <www>__1;

		internal ClipLanguageReplacer $this;

		internal object $current;

		internal bool $disposing;

		internal int $PC;
	}
}
