using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace TMPro.Examples
{
	public class Benchmark01 : MonoBehaviour
	{
		public Benchmark01()
		{
		}

		private IEnumerator Start()
		{
			if (this.BenchmarkType == 0)
			{
				this.m_textMeshPro = base.gameObject.AddComponent<TextMeshPro>();
				this.m_textMeshPro.autoSizeTextContainer = true;
				if (this.TMProFont != null)
				{
					this.m_textMeshPro.font = this.TMProFont;
				}
				this.m_textMeshPro.fontSize = 48f;
				this.m_textMeshPro.alignment = TextAlignmentOptions.Center;
				this.m_textMeshPro.extraPadding = true;
				this.m_textMeshPro.enableWordWrapping = false;
				this.m_material01 = this.m_textMeshPro.font.material;
				this.m_material02 = (Resources.Load("Fonts & Materials/LiberationSans SDF - Drop Shadow", typeof(Material)) as Material);
			}
			else if (this.BenchmarkType == 1)
			{
				this.m_textMesh = base.gameObject.AddComponent<TextMesh>();
				if (this.TextMeshFont != null)
				{
					this.m_textMesh.font = this.TextMeshFont;
					this.m_textMesh.GetComponent<Renderer>().sharedMaterial = this.m_textMesh.font.material;
				}
				else
				{
					this.m_textMesh.font = (Resources.Load("Fonts/ARIAL", typeof(Font)) as Font);
					this.m_textMesh.GetComponent<Renderer>().sharedMaterial = this.m_textMesh.font.material;
				}
				this.m_textMesh.fontSize = 48;
				this.m_textMesh.anchor = TextAnchor.MiddleCenter;
			}
			for (int i = 0; i <= 1000000; i++)
			{
				if (this.BenchmarkType == 0)
				{
					this.m_textMeshPro.SetText("The <#0050FF>count is: </color>{0}", (float)(i % 1000));
					if (i % 1000 == 999)
					{
						TMP_Text textMeshPro = this.m_textMeshPro;
						Material fontSharedMaterial;
						if (this.m_textMeshPro.fontSharedMaterial == this.m_material01)
						{
							Material material = this.m_material02;
							this.m_textMeshPro.fontSharedMaterial = material;
							fontSharedMaterial = material;
						}
						else
						{
							Material material = this.m_material01;
							this.m_textMeshPro.fontSharedMaterial = material;
							fontSharedMaterial = material;
						}
						textMeshPro.fontSharedMaterial = fontSharedMaterial;
					}
				}
				else if (this.BenchmarkType == 1)
				{
					this.m_textMesh.text = "The <color=#0050FF>count is: </color>" + (i % 1000).ToString();
				}
				yield return null;
			}
			yield return null;
			yield break;
		}

		public int BenchmarkType;

		public TMP_FontAsset TMProFont;

		public Font TextMeshFont;

		private TextMeshPro m_textMeshPro;

		private TextContainer m_textContainer;

		private TextMesh m_textMesh;

		private const string label01 = "The <#0050FF>count is: </color>{0}";

		private const string label02 = "The <color=#0050FF>count is: </color>";

		private Material m_material01;

		private Material m_material02;

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
					if (this.$this.BenchmarkType == 0)
					{
						this.$this.m_textMeshPro = this.$this.gameObject.AddComponent<TextMeshPro>();
						this.$this.m_textMeshPro.autoSizeTextContainer = true;
						if (this.$this.TMProFont != null)
						{
							this.$this.m_textMeshPro.font = this.$this.TMProFont;
						}
						this.$this.m_textMeshPro.fontSize = 48f;
						this.$this.m_textMeshPro.alignment = TextAlignmentOptions.Center;
						this.$this.m_textMeshPro.extraPadding = true;
						this.$this.m_textMeshPro.enableWordWrapping = false;
						this.$this.m_material01 = this.$this.m_textMeshPro.font.material;
						this.$this.m_material02 = (Resources.Load("Fonts & Materials/LiberationSans SDF - Drop Shadow", typeof(Material)) as Material);
					}
					else if (this.$this.BenchmarkType == 1)
					{
						this.$this.m_textMesh = this.$this.gameObject.AddComponent<TextMesh>();
						if (this.$this.TextMeshFont != null)
						{
							this.$this.m_textMesh.font = this.$this.TextMeshFont;
							this.$this.m_textMesh.GetComponent<Renderer>().sharedMaterial = this.$this.m_textMesh.font.material;
						}
						else
						{
							this.$this.m_textMesh.font = (Resources.Load("Fonts/ARIAL", typeof(Font)) as Font);
							this.$this.m_textMesh.GetComponent<Renderer>().sharedMaterial = this.$this.m_textMesh.font.material;
						}
						this.$this.m_textMesh.fontSize = 48;
						this.$this.m_textMesh.anchor = TextAnchor.MiddleCenter;
					}
					this.<i>__1 = 0;
					break;
				case 1u:
					this.<i>__1++;
					break;
				case 2u:
					this.$PC = -1;
					return false;
				default:
					return false;
				}
				if (this.<i>__1 > 1000000)
				{
					this.$current = null;
					if (!this.$disposing)
					{
						this.$PC = 2;
					}
				}
				else
				{
					if (this.$this.BenchmarkType == 0)
					{
						this.$this.m_textMeshPro.SetText("The <#0050FF>count is: </color>{0}", (float)(this.<i>__1 % 1000));
						if (this.<i>__1 % 1000 == 999)
						{
							TMP_Text textMeshPro = this.$this.m_textMeshPro;
							Material fontSharedMaterial;
							if (this.$this.m_textMeshPro.fontSharedMaterial == this.$this.m_material01)
							{
								Material material = this.$this.m_material02;
								this.$this.m_textMeshPro.fontSharedMaterial = material;
								fontSharedMaterial = material;
							}
							else
							{
								Material material = this.$this.m_material01;
								this.$this.m_textMeshPro.fontSharedMaterial = material;
								fontSharedMaterial = material;
							}
							textMeshPro.fontSharedMaterial = fontSharedMaterial;
						}
					}
					else if (this.$this.BenchmarkType == 1)
					{
						this.$this.m_textMesh.text = "The <color=#0050FF>count is: </color>" + (this.<i>__1 % 1000).ToString();
					}
					this.$current = null;
					if (!this.$disposing)
					{
						this.$PC = 1;
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

			internal int <i>__1;

			internal Benchmark01 $this;

			internal object $current;

			internal bool $disposing;

			internal int $PC;
		}
	}
}
