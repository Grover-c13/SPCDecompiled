using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class HintManager : MonoBehaviour
{
	public HintManager()
	{
	}

	private void Awake()
	{
		this.box.canvasRenderer.SetAlpha(0f);
		HintManager.singleton = this;
		for (int i = 0; i < this.hints.Length; i++)
		{
			this.hints[i].content_en = TranslationReader.Get("Hints", i);
		}
	}

	private void Start()
	{
		this.box.canvasRenderer.SetAlpha(0f);
		base.StartCoroutine(this.ShowHints());
	}

	private IEnumerator ShowHints()
	{
		CanvasRenderer cr = this.box.canvasRenderer;
		for (;;)
		{
			while (cr.GetAlpha() > 0f)
			{
				cr.SetAlpha(cr.GetAlpha() - Time.deltaTime * 5f);
				cr.GetComponentInChildren<Text>().canvasRenderer.SetAlpha(cr.GetAlpha());
				yield return new WaitForEndOfFrame();
			}
			yield return new WaitForEndOfFrame();
		}
		yield break;
	}

	public void AddHint(int hintID)
	{
		if (TutorialManager.status)
		{
			return;
		}
		if (PlayerPrefs.GetInt(this.hints[hintID].keyName, 0) == 0)
		{
			this.hintQueue.Add(this.hints[hintID]);
			PlayerPrefs.SetInt(this.hints[hintID].keyName, 1);
		}
	}

	public static HintManager singleton;

	[SerializeField]
	private Image box;

	public HintManager.Hint[] hints;

	public List<HintManager.Hint> hintQueue = new List<HintManager.Hint>();

	[Serializable]
	public class Hint
	{
		public Hint()
		{
		}

		[Multiline]
		public string content_en;

		public string keyName;

		public float duration;
	}

	[CompilerGenerated]
	private sealed class <ShowHints>c__Iterator0 : IEnumerator, IDisposable, IEnumerator<object>
	{
		[DebuggerHidden]
		public <ShowHints>c__Iterator0()
		{
		}

		public bool MoveNext()
		{
			uint num = (uint)this.$PC;
			this.$PC = -1;
			switch (num)
			{
			case 0u:
				this.<cr>__0 = this.$this.box.canvasRenderer;
				break;
			case 1u:
				IL_A1:
				if (this.<cr>__0.GetAlpha() <= 0f)
				{
					this.$current = new WaitForEndOfFrame();
					if (!this.$disposing)
					{
						this.$PC = 2;
					}
				}
				else
				{
					this.<cr>__0.SetAlpha(this.<cr>__0.GetAlpha() - Time.deltaTime * 5f);
					this.<cr>__0.GetComponentInChildren<Text>().canvasRenderer.SetAlpha(this.<cr>__0.GetAlpha());
					this.$current = new WaitForEndOfFrame();
					if (!this.$disposing)
					{
						this.$PC = 1;
					}
				}
				return true;
			case 2u:
				break;
			default:
				return false;
			}
			goto IL_A1;
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

		internal CanvasRenderer <cr>__0;

		internal float <dur>__1;

		internal float <v3>__2;

		internal HintManager $this;

		internal object $current;

		internal bool $disposing;

		internal int $PC;
	}
}
