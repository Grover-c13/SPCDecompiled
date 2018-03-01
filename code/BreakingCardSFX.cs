using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BreakingCardSFX : MonoBehaviour
{
	public BreakingCardSFX()
	{
	}

	private void OnEnable()
	{
		base.StopAllCoroutines();
		base.StartCoroutine(this.DoAnimation());
	}

	private IEnumerator DoAnimation()
	{
		this.text = base.GetComponent<TextMeshProUGUI>();
		this.txt = base.GetComponent<Text>();
		for (;;)
		{
			foreach (string item in this.texts)
			{
				try
				{
					if (this.text != null)
					{
						this.text.text = item;
					}
					if (this.txt != null)
					{
						this.txt.text = item;
					}
				}
				catch
				{
					UnityEngine.Debug.LogError("Iteration error in BC-SFX script");
				}
				yield return new WaitForSeconds(this.waitTime);
			}
		}
		yield break;
	}

	public string[] texts;

	public float waitTime = 1.3f;

	private TextMeshProUGUI text;

	private Text txt;

	[CompilerGenerated]
	private sealed class <DoAnimation>c__Iterator0 : IEnumerator, IDisposable, IEnumerator<object>
	{
		[DebuggerHidden]
		public <DoAnimation>c__Iterator0()
		{
		}

		public bool MoveNext()
		{
			uint num = (uint)this.$PC;
			this.$PC = -1;
			switch (num)
			{
			case 0u:
				this.$this.text = this.$this.GetComponent<TextMeshProUGUI>();
				this.$this.txt = this.$this.GetComponent<Text>();
				break;
			case 1u:
				this.$locvar1++;
				goto IL_122;
			default:
				return false;
			}
			IL_4D:
			this.$locvar0 = this.$this.texts;
			this.$locvar1 = 0;
			IL_122:
			if (this.$locvar1 >= this.$locvar0.Length)
			{
				goto IL_4D;
			}
			this.<item>__1 = this.$locvar0[this.$locvar1];
			try
			{
				if (this.$this.text != null)
				{
					this.$this.text.text = this.<item>__1;
				}
				if (this.$this.txt != null)
				{
					this.$this.txt.text = this.<item>__1;
				}
			}
			catch
			{
				UnityEngine.Debug.LogError("Iteration error in BC-SFX script");
			}
			this.$current = new WaitForSeconds(this.$this.waitTime);
			if (!this.$disposing)
			{
				this.$PC = 1;
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

		internal string[] $locvar0;

		internal int $locvar1;

		internal string <item>__1;

		internal BreakingCardSFX $this;

		internal object $current;

		internal bool $disposing;

		internal int $PC;
	}
}
