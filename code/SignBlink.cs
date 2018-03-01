using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class SignBlink : MonoBehaviour
{
	public SignBlink()
	{
	}

	public void Play(int duration)
	{
		if (this.startText == string.Empty)
		{
			this.startText = base.GetComponent<TextMeshProUGUI>().text;
		}
		else
		{
			base.GetComponent<TextMeshProUGUI>().text = this.startText;
		}
	}

	private IEnumerator Blink(string text, int iterations)
	{
		for (int i = 0; i < iterations; i++)
		{
			string s = string.Empty;
			for (int j = 0; j < text.Length; j++)
			{
				s += "QWERTYUIOPASDFGHJKLZXCVBNM01234567890!@#$%^&*()-_=+[]{}/<>"[UnityEngine.Random.Range(0, "QWERTYUIOPASDFGHJKLZXCVBNM01234567890!@#$%^&*()-_=+[]{}/<>".Length)];
				if (j != text.Length - 1 && this.verticalText)
				{
					s += "\n";
				}
			}
			base.GetComponent<TextMeshProUGUI>().text = s;
			yield return new WaitForSeconds(0.02f);
		}
		base.GetComponent<TextMeshProUGUI>().text = text;
		yield break;
	}

	public bool verticalText;

	private string startText;

	private const string alphabet = "QWERTYUIOPASDFGHJKLZXCVBNM01234567890!@#$%^&*()-_=+[]{}/<>";

	[CompilerGenerated]
	private sealed class <Blink>c__Iterator0 : IEnumerator, IDisposable, IEnumerator<object>
	{
		[DebuggerHidden]
		public <Blink>c__Iterator0()
		{
		}

		public bool MoveNext()
		{
			uint num = (uint)this.$PC;
			this.$PC = -1;
			switch (num)
			{
			case 0u:
				this.<i>__1 = 0;
				break;
			case 1u:
				this.<i>__1++;
				break;
			default:
				return false;
			}
			if (this.<i>__1 < this.iterations)
			{
				this.<s>__2 = string.Empty;
				for (int i = 0; i < this.text.Length; i++)
				{
					this.<s>__2 += "QWERTYUIOPASDFGHJKLZXCVBNM01234567890!@#$%^&*()-_=+[]{}/<>"[UnityEngine.Random.Range(0, "QWERTYUIOPASDFGHJKLZXCVBNM01234567890!@#$%^&*()-_=+[]{}/<>".Length)];
					if (i != this.text.Length - 1 && this.$this.verticalText)
					{
						this.<s>__2 += "\n";
					}
				}
				this.$this.GetComponent<TextMeshProUGUI>().text = this.<s>__2;
				this.$current = new WaitForSeconds(0.02f);
				if (!this.$disposing)
				{
					this.$PC = 1;
				}
				return true;
			}
			this.$this.GetComponent<TextMeshProUGUI>().text = this.text;
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

		internal int <i>__1;

		internal int iterations;

		internal string <s>__2;

		internal string text;

		internal SignBlink $this;

		internal object $current;

		internal bool $disposing;

		internal int $PC;
	}
}
