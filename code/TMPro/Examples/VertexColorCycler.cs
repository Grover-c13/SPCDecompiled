using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace TMPro.Examples
{
	public class VertexColorCycler : MonoBehaviour
	{
		public VertexColorCycler()
		{
		}

		private void Awake()
		{
			this.m_TextComponent = base.GetComponent<TMP_Text>();
		}

		private void Start()
		{
			base.StartCoroutine(this.AnimateVertexColors());
		}

		private IEnumerator AnimateVertexColors()
		{
			TMP_TextInfo textInfo = this.m_TextComponent.textInfo;
			int currentCharacter = 0;
			Color32 c0 = this.m_TextComponent.color;
			for (;;)
			{
				int characterCount = textInfo.characterCount;
				if (characterCount == 0)
				{
					yield return new WaitForSeconds(0.25f);
				}
				else
				{
					int materialIndex = textInfo.characterInfo[currentCharacter].materialReferenceIndex;
					Color32[] newVertexColors = textInfo.meshInfo[materialIndex].colors32;
					int vertexIndex = textInfo.characterInfo[currentCharacter].vertexIndex;
					if (textInfo.characterInfo[currentCharacter].isVisible)
					{
						c0 = new Color32((byte)UnityEngine.Random.Range(0, 255), (byte)UnityEngine.Random.Range(0, 255), (byte)UnityEngine.Random.Range(0, 255), byte.MaxValue);
						newVertexColors[vertexIndex] = c0;
						newVertexColors[vertexIndex + 1] = c0;
						newVertexColors[vertexIndex + 2] = c0;
						newVertexColors[vertexIndex + 3] = c0;
						this.m_TextComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
					}
					currentCharacter = (currentCharacter + 1) % characterCount;
					yield return new WaitForSeconds(0.05f);
				}
			}
			yield break;
		}

		private TMP_Text m_TextComponent;

		[CompilerGenerated]
		private sealed class <AnimateVertexColors>c__Iterator0 : IEnumerator, IDisposable, IEnumerator<object>
		{
			[DebuggerHidden]
			public <AnimateVertexColors>c__Iterator0()
			{
			}

			public bool MoveNext()
			{
				uint num = (uint)this.$PC;
				this.$PC = -1;
				switch (num)
				{
				case 0u:
					this.<textInfo>__0 = this.$this.m_TextComponent.textInfo;
					this.<currentCharacter>__0 = 0;
					this.<c0>__0 = this.$this.m_TextComponent.color;
					break;
				case 1u:
					break;
				case 2u:
					break;
				default:
					return false;
				}
				this.<characterCount>__1 = this.<textInfo>__0.characterCount;
				if (this.<characterCount>__1 == 0)
				{
					this.$current = new WaitForSeconds(0.25f);
					if (!this.$disposing)
					{
						this.$PC = 1;
					}
				}
				else
				{
					this.<materialIndex>__1 = this.<textInfo>__0.characterInfo[this.<currentCharacter>__0].materialReferenceIndex;
					this.<newVertexColors>__1 = this.<textInfo>__0.meshInfo[this.<materialIndex>__1].colors32;
					this.<vertexIndex>__1 = this.<textInfo>__0.characterInfo[this.<currentCharacter>__0].vertexIndex;
					if (this.<textInfo>__0.characterInfo[this.<currentCharacter>__0].isVisible)
					{
						this.<c0>__0 = new Color32((byte)UnityEngine.Random.Range(0, 255), (byte)UnityEngine.Random.Range(0, 255), (byte)UnityEngine.Random.Range(0, 255), byte.MaxValue);
						this.<newVertexColors>__1[this.<vertexIndex>__1] = this.<c0>__0;
						this.<newVertexColors>__1[this.<vertexIndex>__1 + 1] = this.<c0>__0;
						this.<newVertexColors>__1[this.<vertexIndex>__1 + 2] = this.<c0>__0;
						this.<newVertexColors>__1[this.<vertexIndex>__1 + 3] = this.<c0>__0;
						this.$this.m_TextComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
					}
					this.<currentCharacter>__0 = (this.<currentCharacter>__0 + 1) % this.<characterCount>__1;
					this.$current = new WaitForSeconds(0.05f);
					if (!this.$disposing)
					{
						this.$PC = 2;
					}
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

			internal TMP_TextInfo <textInfo>__0;

			internal int <currentCharacter>__0;

			internal Color32 <c0>__0;

			internal int <characterCount>__1;

			internal int <materialIndex>__1;

			internal Color32[] <newVertexColors>__1;

			internal int <vertexIndex>__1;

			internal VertexColorCycler $this;

			internal object $current;

			internal bool $disposing;

			internal int $PC;
		}
	}
}
