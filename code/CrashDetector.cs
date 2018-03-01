using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class CrashDetector : MonoBehaviour
{
	public CrashDetector()
	{
	}

	private void Awake()
	{
		CrashDetector.singleton = this;
	}

	public static bool Show()
	{
		if (SystemInfo.graphicsDeviceName.ToUpper().Contains("INTEL") && PlayerPrefs.GetInt("intel_warning") != 1)
		{
			PlayerPrefs.SetInt("intel_warning", 1);
			CrashDetector.singleton.StartCoroutine(CrashDetector.singleton.IShow());
			return true;
		}
		return false;
	}

	public IEnumerator IShow()
	{
		this.root.SetActive(true);
		Button button = this.root.GetComponentInChildren<Button>();
		Text text = button.GetComponent<Text>();
		button.interactable = false;
		for (int i = 10; i >= 1; i--)
		{
			text.text = "OKAY (" + i + ")";
			yield return new WaitForSeconds(1f);
		}
		text.text = "OKAY";
		button.interactable = true;
		yield break;
	}

	public GameObject root;

	public static CrashDetector singleton;

	[CompilerGenerated]
	private sealed class <IShow>c__Iterator0 : IEnumerator, IDisposable, IEnumerator<object>
	{
		[DebuggerHidden]
		public <IShow>c__Iterator0()
		{
		}

		public bool MoveNext()
		{
			uint num = (uint)this.$PC;
			this.$PC = -1;
			switch (num)
			{
			case 0u:
				this.$this.root.SetActive(true);
				this.<button>__0 = this.$this.root.GetComponentInChildren<Button>();
				this.<text>__0 = this.<button>__0.GetComponent<Text>();
				this.<button>__0.interactable = false;
				this.<i>__1 = 10;
				break;
			case 1u:
				this.<i>__1--;
				break;
			default:
				return false;
			}
			if (this.<i>__1 >= 1)
			{
				this.<text>__0.text = "OKAY (" + this.<i>__1 + ")";
				this.$current = new WaitForSeconds(1f);
				if (!this.$disposing)
				{
					this.$PC = 1;
				}
				return true;
			}
			this.<text>__0.text = "OKAY";
			this.<button>__0.interactable = true;
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

		internal Button <button>__0;

		internal Text <text>__0;

		internal int <i>__1;

		internal CrashDetector $this;

		internal object $current;

		internal bool $disposing;

		internal int $PC;
	}
}
