using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace TMPro.Examples
{
	public class VertexShakeB : MonoBehaviour
	{
		public VertexShakeB()
		{
		}

		private void Awake()
		{
			this.m_TextComponent = base.GetComponent<TMP_Text>();
		}

		private void OnEnable()
		{
			TMPro_EventManager.TEXT_CHANGED_EVENT.Add(new Action<UnityEngine.Object>(this.ON_TEXT_CHANGED));
		}

		private void OnDisable()
		{
			TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(new Action<UnityEngine.Object>(this.ON_TEXT_CHANGED));
		}

		private void Start()
		{
			base.StartCoroutine(this.AnimateVertexColors());
		}

		private void ON_TEXT_CHANGED(UnityEngine.Object obj)
		{
			if (this.m_TextComponent)
			{
				this.hasTextChanged = true;
			}
		}

		private IEnumerator AnimateVertexColors()
		{
			this.m_TextComponent.ForceMeshUpdate();
			TMP_TextInfo textInfo = this.m_TextComponent.textInfo;
			Vector3[][] copyOfVertices = new Vector3[0][];
			this.hasTextChanged = true;
			for (;;)
			{
				if (this.hasTextChanged)
				{
					if (copyOfVertices.Length < textInfo.meshInfo.Length)
					{
						copyOfVertices = new Vector3[textInfo.meshInfo.Length][];
					}
					for (int i = 0; i < textInfo.meshInfo.Length; i++)
					{
						int num = textInfo.meshInfo[i].vertices.Length;
						copyOfVertices[i] = new Vector3[num];
					}
					this.hasTextChanged = false;
				}
				if (textInfo.characterCount == 0)
				{
					yield return new WaitForSeconds(0.25f);
				}
				else
				{
					int lineCount = textInfo.lineCount;
					for (int j = 0; j < lineCount; j++)
					{
						int firstCharacterIndex = textInfo.lineInfo[j].firstCharacterIndex;
						int lastCharacterIndex = textInfo.lineInfo[j].lastCharacterIndex;
						Vector3 b = (textInfo.characterInfo[firstCharacterIndex].bottomLeft + textInfo.characterInfo[lastCharacterIndex].topRight) / 2f;
						Quaternion q = Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(-0.25f, 0.25f));
						for (int k = firstCharacterIndex; k <= lastCharacterIndex; k++)
						{
							if (textInfo.characterInfo[k].isVisible)
							{
								int materialReferenceIndex = textInfo.characterInfo[k].materialReferenceIndex;
								int vertexIndex = textInfo.characterInfo[k].vertexIndex;
								Vector3[] vertices = textInfo.meshInfo[materialReferenceIndex].vertices;
								Vector3 b2 = (vertices[vertexIndex] + vertices[vertexIndex + 2]) / 2f;
								copyOfVertices[materialReferenceIndex][vertexIndex] = vertices[vertexIndex] - b2;
								copyOfVertices[materialReferenceIndex][vertexIndex + 1] = vertices[vertexIndex + 1] - b2;
								copyOfVertices[materialReferenceIndex][vertexIndex + 2] = vertices[vertexIndex + 2] - b2;
								copyOfVertices[materialReferenceIndex][vertexIndex + 3] = vertices[vertexIndex + 3] - b2;
								float d = UnityEngine.Random.Range(0.95f, 1.05f);
								Matrix4x4 matrix = Matrix4x4.TRS(Vector3.one, Quaternion.identity, Vector3.one * d);
								copyOfVertices[materialReferenceIndex][vertexIndex] = matrix.MultiplyPoint3x4(copyOfVertices[materialReferenceIndex][vertexIndex]);
								copyOfVertices[materialReferenceIndex][vertexIndex + 1] = matrix.MultiplyPoint3x4(copyOfVertices[materialReferenceIndex][vertexIndex + 1]);
								copyOfVertices[materialReferenceIndex][vertexIndex + 2] = matrix.MultiplyPoint3x4(copyOfVertices[materialReferenceIndex][vertexIndex + 2]);
								copyOfVertices[materialReferenceIndex][vertexIndex + 3] = matrix.MultiplyPoint3x4(copyOfVertices[materialReferenceIndex][vertexIndex + 3]);
								copyOfVertices[materialReferenceIndex][vertexIndex] += b2;
								copyOfVertices[materialReferenceIndex][vertexIndex + 1] += b2;
								copyOfVertices[materialReferenceIndex][vertexIndex + 2] += b2;
								copyOfVertices[materialReferenceIndex][vertexIndex + 3] += b2;
								copyOfVertices[materialReferenceIndex][vertexIndex] -= b;
								copyOfVertices[materialReferenceIndex][vertexIndex + 1] -= b;
								copyOfVertices[materialReferenceIndex][vertexIndex + 2] -= b;
								copyOfVertices[materialReferenceIndex][vertexIndex + 3] -= b;
								matrix = Matrix4x4.TRS(Vector3.one, q, Vector3.one);
								copyOfVertices[materialReferenceIndex][vertexIndex] = matrix.MultiplyPoint3x4(copyOfVertices[materialReferenceIndex][vertexIndex]);
								copyOfVertices[materialReferenceIndex][vertexIndex + 1] = matrix.MultiplyPoint3x4(copyOfVertices[materialReferenceIndex][vertexIndex + 1]);
								copyOfVertices[materialReferenceIndex][vertexIndex + 2] = matrix.MultiplyPoint3x4(copyOfVertices[materialReferenceIndex][vertexIndex + 2]);
								copyOfVertices[materialReferenceIndex][vertexIndex + 3] = matrix.MultiplyPoint3x4(copyOfVertices[materialReferenceIndex][vertexIndex + 3]);
								copyOfVertices[materialReferenceIndex][vertexIndex] += b;
								copyOfVertices[materialReferenceIndex][vertexIndex + 1] += b;
								copyOfVertices[materialReferenceIndex][vertexIndex + 2] += b;
								copyOfVertices[materialReferenceIndex][vertexIndex + 3] += b;
							}
						}
					}
					for (int l = 0; l < textInfo.meshInfo.Length; l++)
					{
						textInfo.meshInfo[l].mesh.vertices = copyOfVertices[l];
						this.m_TextComponent.UpdateGeometry(textInfo.meshInfo[l].mesh, l);
					}
					yield return new WaitForSeconds(0.1f);
				}
			}
			yield break;
		}

		public float AngleMultiplier = 1f;

		public float SpeedMultiplier = 1f;

		public float CurveScale = 1f;

		private TMP_Text m_TextComponent;

		private bool hasTextChanged;

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
					this.$this.m_TextComponent.ForceMeshUpdate();
					this.<textInfo>__0 = this.$this.m_TextComponent.textInfo;
					this.<copyOfVertices>__0 = new Vector3[0][];
					this.$this.hasTextChanged = true;
					break;
				case 1u:
					break;
				case 2u:
					break;
				default:
					return false;
				}
				if (this.$this.hasTextChanged)
				{
					if (this.<copyOfVertices>__0.Length < this.<textInfo>__0.meshInfo.Length)
					{
						this.<copyOfVertices>__0 = new Vector3[this.<textInfo>__0.meshInfo.Length][];
					}
					for (int i = 0; i < this.<textInfo>__0.meshInfo.Length; i++)
					{
						int num2 = this.<textInfo>__0.meshInfo[i].vertices.Length;
						this.<copyOfVertices>__0[i] = new Vector3[num2];
					}
					this.$this.hasTextChanged = false;
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
					this.<lineCount>__1 = this.<textInfo>__0.lineCount;
					for (int j = 0; j < this.<lineCount>__1; j++)
					{
						int firstCharacterIndex = this.<textInfo>__0.lineInfo[j].firstCharacterIndex;
						int lastCharacterIndex = this.<textInfo>__0.lineInfo[j].lastCharacterIndex;
						Vector3 b = (this.<textInfo>__0.characterInfo[firstCharacterIndex].bottomLeft + this.<textInfo>__0.characterInfo[lastCharacterIndex].topRight) / 2f;
						Quaternion q = Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(-0.25f, 0.25f));
						for (int k = firstCharacterIndex; k <= lastCharacterIndex; k++)
						{
							if (this.<textInfo>__0.characterInfo[k].isVisible)
							{
								int materialReferenceIndex = this.<textInfo>__0.characterInfo[k].materialReferenceIndex;
								int vertexIndex = this.<textInfo>__0.characterInfo[k].vertexIndex;
								Vector3[] vertices = this.<textInfo>__0.meshInfo[materialReferenceIndex].vertices;
								Vector3 b2 = (vertices[vertexIndex] + vertices[vertexIndex + 2]) / 2f;
								this.<copyOfVertices>__0[materialReferenceIndex][vertexIndex] = vertices[vertexIndex] - b2;
								this.<copyOfVertices>__0[materialReferenceIndex][vertexIndex + 1] = vertices[vertexIndex + 1] - b2;
								this.<copyOfVertices>__0[materialReferenceIndex][vertexIndex + 2] = vertices[vertexIndex + 2] - b2;
								this.<copyOfVertices>__0[materialReferenceIndex][vertexIndex + 3] = vertices[vertexIndex + 3] - b2;
								float d = UnityEngine.Random.Range(0.95f, 1.05f);
								this.<matrix>__2 = Matrix4x4.TRS(Vector3.one, Quaternion.identity, Vector3.one * d);
								this.<copyOfVertices>__0[materialReferenceIndex][vertexIndex] = this.<matrix>__2.MultiplyPoint3x4(this.<copyOfVertices>__0[materialReferenceIndex][vertexIndex]);
								this.<copyOfVertices>__0[materialReferenceIndex][vertexIndex + 1] = this.<matrix>__2.MultiplyPoint3x4(this.<copyOfVertices>__0[materialReferenceIndex][vertexIndex + 1]);
								this.<copyOfVertices>__0[materialReferenceIndex][vertexIndex + 2] = this.<matrix>__2.MultiplyPoint3x4(this.<copyOfVertices>__0[materialReferenceIndex][vertexIndex + 2]);
								this.<copyOfVertices>__0[materialReferenceIndex][vertexIndex + 3] = this.<matrix>__2.MultiplyPoint3x4(this.<copyOfVertices>__0[materialReferenceIndex][vertexIndex + 3]);
								this.<copyOfVertices>__0[materialReferenceIndex][vertexIndex] += b2;
								this.<copyOfVertices>__0[materialReferenceIndex][vertexIndex + 1] += b2;
								this.<copyOfVertices>__0[materialReferenceIndex][vertexIndex + 2] += b2;
								this.<copyOfVertices>__0[materialReferenceIndex][vertexIndex + 3] += b2;
								this.<copyOfVertices>__0[materialReferenceIndex][vertexIndex] -= b;
								this.<copyOfVertices>__0[materialReferenceIndex][vertexIndex + 1] -= b;
								this.<copyOfVertices>__0[materialReferenceIndex][vertexIndex + 2] -= b;
								this.<copyOfVertices>__0[materialReferenceIndex][vertexIndex + 3] -= b;
								this.<matrix>__2 = Matrix4x4.TRS(Vector3.one, q, Vector3.one);
								this.<copyOfVertices>__0[materialReferenceIndex][vertexIndex] = this.<matrix>__2.MultiplyPoint3x4(this.<copyOfVertices>__0[materialReferenceIndex][vertexIndex]);
								this.<copyOfVertices>__0[materialReferenceIndex][vertexIndex + 1] = this.<matrix>__2.MultiplyPoint3x4(this.<copyOfVertices>__0[materialReferenceIndex][vertexIndex + 1]);
								this.<copyOfVertices>__0[materialReferenceIndex][vertexIndex + 2] = this.<matrix>__2.MultiplyPoint3x4(this.<copyOfVertices>__0[materialReferenceIndex][vertexIndex + 2]);
								this.<copyOfVertices>__0[materialReferenceIndex][vertexIndex + 3] = this.<matrix>__2.MultiplyPoint3x4(this.<copyOfVertices>__0[materialReferenceIndex][vertexIndex + 3]);
								this.<copyOfVertices>__0[materialReferenceIndex][vertexIndex] += b;
								this.<copyOfVertices>__0[materialReferenceIndex][vertexIndex + 1] += b;
								this.<copyOfVertices>__0[materialReferenceIndex][vertexIndex + 2] += b;
								this.<copyOfVertices>__0[materialReferenceIndex][vertexIndex + 3] += b;
							}
						}
					}
					for (int l = 0; l < this.<textInfo>__0.meshInfo.Length; l++)
					{
						this.<textInfo>__0.meshInfo[l].mesh.vertices = this.<copyOfVertices>__0[l];
						this.$this.m_TextComponent.UpdateGeometry(this.<textInfo>__0.meshInfo[l].mesh, l);
					}
					this.$current = new WaitForSeconds(0.1f);
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

			internal Vector3[][] <copyOfVertices>__0;

			internal int <characterCount>__1;

			internal int <lineCount>__1;

			internal Matrix4x4 <matrix>__2;

			internal VertexShakeB $this;

			internal object $current;

			internal bool $disposing;

			internal int $PC;
		}
	}
}
