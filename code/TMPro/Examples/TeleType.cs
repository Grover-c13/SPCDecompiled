using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace TMPro.Examples
{
	public class TeleType : MonoBehaviour
	{
		public TeleType()
		{
		}

		private void Awake()
		{
			this.m_textMeshPro = base.GetComponent<TMP_Text>();
			this.m_textMeshPro.text = this.label01;
			this.m_textMeshPro.enableWordWrapping = true;
			this.m_textMeshPro.alignment = TextAlignmentOptions.Top;
		}

		private IEnumerator Start()
		{
			this.m_textMeshPro.ForceMeshUpdate();
			int totalVisibleCharacters = this.m_textMeshPro.textInfo.characterCount;
			int counter = 0;
			int visibleCount = 0;
			for (;;)
			{
				visibleCount = counter % (totalVisibleCharacters + 1);
				this.m_textMeshPro.maxVisibleCharacters = visibleCount;
				if (visibleCount >= totalVisibleCharacters)
				{
					yield return new WaitForSeconds(1f);
					this.m_textMeshPro.text = this.label02;
					yield return new WaitForSeconds(1f);
					this.m_textMeshPro.text = this.label01;
					yield return new WaitForSeconds(1f);
				}
				counter++;
				yield return new WaitForSeconds(0.05f);
			}
			yield break;
		}

		private string label01 = "Example <sprite=2> of using <sprite=7> <#ffa000>Graphics Inline</color> <sprite=5> with Text in <font=\"Bangers SDF\" material=\"Bangers SDF - Drop Shadow\">TextMesh<#40a0ff>Pro</color></font><sprite=0> and Unity<sprite=1>";

		private string label02 = "Example <sprite=2> of using <sprite=7> <#ffa000>Graphics Inline</color> <sprite=5> with Text in <font=\"Bangers SDF\" material=\"Bangers SDF - Drop Shadow\">TextMesh<#40a0ff>Pro</color></font><sprite=0> and Unity<sprite=2>";

		private TMP_Text m_textMeshPro;

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
					this.$this.m_textMeshPro.ForceMeshUpdate();
					this.<totalVisibleCharacters>__0 = this.$this.m_textMeshPro.textInfo.characterCount;
					this.<counter>__0 = 0;
					this.<visibleCount>__0 = 0;
					break;
				case 1u:
					this.$this.m_textMeshPro.text = this.$this.label02;
					this.$current = new WaitForSeconds(1f);
					if (!this.$disposing)
					{
						this.$PC = 2;
					}
					return true;
				case 2u:
					this.$this.m_textMeshPro.text = this.$this.label01;
					this.$current = new WaitForSeconds(1f);
					if (!this.$disposing)
					{
						this.$PC = 3;
					}
					return true;
				case 3u:
					IL_144:
					this.<counter>__0++;
					this.$current = new WaitForSeconds(0.05f);
					if (!this.$disposing)
					{
						this.$PC = 4;
					}
					return true;
				case 4u:
					break;
				default:
					return false;
				}
				this.<visibleCount>__0 = this.<counter>__0 % (this.<totalVisibleCharacters>__0 + 1);
				this.$this.m_textMeshPro.maxVisibleCharacters = this.<visibleCount>__0;
				if (this.<visibleCount>__0 < this.<totalVisibleCharacters>__0)
				{
					goto IL_144;
				}
				this.$current = new WaitForSeconds(1f);
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

			internal int <totalVisibleCharacters>__0;

			internal int <counter>__0;

			internal int <visibleCount>__0;

			internal TeleType $this;

			internal object $current;

			internal bool $disposing;

			internal int $PC;
		}
	}
}
