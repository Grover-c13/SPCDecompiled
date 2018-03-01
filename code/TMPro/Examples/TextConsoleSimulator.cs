using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace TMPro.Examples
{
	public class TextConsoleSimulator : MonoBehaviour
	{
		public TextConsoleSimulator()
		{
		}

		private void Awake()
		{
			this.m_TextComponent = base.gameObject.GetComponent<TMP_Text>();
		}

		private void Start()
		{
			base.StartCoroutine(this.RevealCharacters(this.m_TextComponent));
		}

		private void OnEnable()
		{
			TMPro_EventManager.TEXT_CHANGED_EVENT.Add(new Action<UnityEngine.Object>(this.ON_TEXT_CHANGED));
		}

		private void OnDisable()
		{
			TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(new Action<UnityEngine.Object>(this.ON_TEXT_CHANGED));
		}

		private void ON_TEXT_CHANGED(UnityEngine.Object obj)
		{
			this.hasTextChanged = true;
		}

		private IEnumerator RevealCharacters(TMP_Text textComponent)
		{
			textComponent.ForceMeshUpdate();
			TMP_TextInfo textInfo = textComponent.textInfo;
			int totalVisibleCharacters = textInfo.characterCount;
			int visibleCount = 0;
			for (;;)
			{
				if (this.hasTextChanged)
				{
					totalVisibleCharacters = textInfo.characterCount;
					this.hasTextChanged = false;
				}
				if (visibleCount > totalVisibleCharacters)
				{
					yield return new WaitForSeconds(1f);
					visibleCount = 0;
				}
				textComponent.maxVisibleCharacters = visibleCount;
				visibleCount++;
				yield return new WaitForSeconds(0f);
			}
			yield break;
		}

		private IEnumerator RevealWords(TMP_Text textComponent)
		{
			textComponent.ForceMeshUpdate();
			int totalWordCount = textComponent.textInfo.wordCount;
			int totalVisibleCharacters = textComponent.textInfo.characterCount;
			int counter = 0;
			int currentWord = 0;
			int visibleCount = 0;
			for (;;)
			{
				currentWord = counter % (totalWordCount + 1);
				if (currentWord == 0)
				{
					visibleCount = 0;
				}
				else if (currentWord < totalWordCount)
				{
					visibleCount = textComponent.textInfo.wordInfo[currentWord - 1].lastCharacterIndex + 1;
				}
				else if (currentWord == totalWordCount)
				{
					visibleCount = totalVisibleCharacters;
				}
				textComponent.maxVisibleCharacters = visibleCount;
				if (visibleCount >= totalVisibleCharacters)
				{
					yield return new WaitForSeconds(1f);
				}
				counter++;
				yield return new WaitForSeconds(0.1f);
			}
			yield break;
		}

		private TMP_Text m_TextComponent;

		private bool hasTextChanged;

		[CompilerGenerated]
		private sealed class <RevealCharacters>c__Iterator0 : IEnumerator, IDisposable, IEnumerator<object>
		{
			[DebuggerHidden]
			public <RevealCharacters>c__Iterator0()
			{
			}

			public bool MoveNext()
			{
				uint num = (uint)this.$PC;
				this.$PC = -1;
				switch (num)
				{
				case 0u:
					this.textComponent.ForceMeshUpdate();
					this.<textInfo>__0 = this.textComponent.textInfo;
					this.<totalVisibleCharacters>__0 = this.<textInfo>__0.characterCount;
					this.<visibleCount>__0 = 0;
					break;
				case 1u:
					this.<visibleCount>__0 = 0;
					goto IL_C2;
				case 2u:
					break;
				default:
					return false;
				}
				if (this.$this.hasTextChanged)
				{
					this.<totalVisibleCharacters>__0 = this.<textInfo>__0.characterCount;
					this.$this.hasTextChanged = false;
				}
				if (this.<visibleCount>__0 > this.<totalVisibleCharacters>__0)
				{
					this.$current = new WaitForSeconds(1f);
					if (!this.$disposing)
					{
						this.$PC = 1;
					}
					return true;
				}
				IL_C2:
				this.textComponent.maxVisibleCharacters = this.<visibleCount>__0;
				this.<visibleCount>__0++;
				this.$current = new WaitForSeconds(0f);
				if (!this.$disposing)
				{
					this.$PC = 2;
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

			internal TMP_Text textComponent;

			internal TMP_TextInfo <textInfo>__0;

			internal int <totalVisibleCharacters>__0;

			internal int <visibleCount>__0;

			internal TextConsoleSimulator $this;

			internal object $current;

			internal bool $disposing;

			internal int $PC;
		}

		[CompilerGenerated]
		private sealed class <RevealWords>c__Iterator1 : IEnumerator, IDisposable, IEnumerator<object>
		{
			[DebuggerHidden]
			public <RevealWords>c__Iterator1()
			{
			}

			public bool MoveNext()
			{
				uint num = (uint)this.$PC;
				this.$PC = -1;
				switch (num)
				{
				case 0u:
					this.textComponent.ForceMeshUpdate();
					this.<totalWordCount>__0 = this.textComponent.textInfo.wordCount;
					this.<totalVisibleCharacters>__0 = this.textComponent.textInfo.characterCount;
					this.<counter>__0 = 0;
					this.<currentWord>__0 = 0;
					this.<visibleCount>__0 = 0;
					break;
				case 1u:
					IL_140:
					this.<counter>__0++;
					this.$current = new WaitForSeconds(0.1f);
					if (!this.$disposing)
					{
						this.$PC = 2;
					}
					return true;
				case 2u:
					break;
				default:
					return false;
				}
				this.<currentWord>__0 = this.<counter>__0 % (this.<totalWordCount>__0 + 1);
				if (this.<currentWord>__0 == 0)
				{
					this.<visibleCount>__0 = 0;
				}
				else if (this.<currentWord>__0 < this.<totalWordCount>__0)
				{
					this.<visibleCount>__0 = this.textComponent.textInfo.wordInfo[this.<currentWord>__0 - 1].lastCharacterIndex + 1;
				}
				else if (this.<currentWord>__0 == this.<totalWordCount>__0)
				{
					this.<visibleCount>__0 = this.<totalVisibleCharacters>__0;
				}
				this.textComponent.maxVisibleCharacters = this.<visibleCount>__0;
				if (this.<visibleCount>__0 < this.<totalVisibleCharacters>__0)
				{
					goto IL_140;
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

			internal TMP_Text textComponent;

			internal int <totalWordCount>__0;

			internal int <totalVisibleCharacters>__0;

			internal int <counter>__0;

			internal int <currentWord>__0;

			internal int <visibleCount>__0;

			internal object $current;

			internal bool $disposing;

			internal int $PC;
		}
	}
}
