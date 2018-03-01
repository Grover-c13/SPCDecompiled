using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace TMPro.Examples
{
	public class VertexZoom : MonoBehaviour
	{
		public VertexZoom()
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
			if (obj == this.m_TextComponent)
			{
				this.hasTextChanged = true;
			}
		}

		private IEnumerator AnimateVertexColors()
		{
			this.m_TextComponent.ForceMeshUpdate();
			TMP_TextInfo textInfo = this.m_TextComponent.textInfo;
			TMP_MeshInfo[] cachedMeshInfoVertexData = textInfo.CopyMeshInfoVertexData();
			List<float> modifiedCharScale = new List<float>();
			List<int> scaleSortingOrder = new List<int>();
			this.hasTextChanged = true;
			for (;;)
			{
				if (this.hasTextChanged)
				{
					cachedMeshInfoVertexData = textInfo.CopyMeshInfoVertexData();
					this.hasTextChanged = false;
				}
				int characterCount = textInfo.characterCount;
				if (characterCount == 0)
				{
					yield return new WaitForSeconds(0.25f);
				}
				else
				{
					modifiedCharScale.Clear();
					scaleSortingOrder.Clear();
					for (int i = 0; i < characterCount; i++)
					{
						TMP_CharacterInfo tmp_CharacterInfo = textInfo.characterInfo[i];
						if (tmp_CharacterInfo.isVisible)
						{
							int materialReferenceIndex = textInfo.characterInfo[i].materialReferenceIndex;
							int vertexIndex = textInfo.characterInfo[i].vertexIndex;
							Vector3[] vertices = cachedMeshInfoVertexData[materialReferenceIndex].vertices;
							Vector2 v = (vertices[vertexIndex] + vertices[vertexIndex + 2]) / 2f;
							Vector3 b2 = v;
							Vector3[] vertices2 = textInfo.meshInfo[materialReferenceIndex].vertices;
							vertices2[vertexIndex] = vertices[vertexIndex] - b2;
							vertices2[vertexIndex + 1] = vertices[vertexIndex + 1] - b2;
							vertices2[vertexIndex + 2] = vertices[vertexIndex + 2] - b2;
							vertices2[vertexIndex + 3] = vertices[vertexIndex + 3] - b2;
							float num = UnityEngine.Random.Range(1f, 1.5f);
							modifiedCharScale.Add(num);
							scaleSortingOrder.Add(modifiedCharScale.Count - 1);
							Matrix4x4 matrix = Matrix4x4.TRS(new Vector3(0f, 0f, 0f), Quaternion.identity, Vector3.one * num);
							vertices2[vertexIndex] = matrix.MultiplyPoint3x4(vertices2[vertexIndex]);
							vertices2[vertexIndex + 1] = matrix.MultiplyPoint3x4(vertices2[vertexIndex + 1]);
							vertices2[vertexIndex + 2] = matrix.MultiplyPoint3x4(vertices2[vertexIndex + 2]);
							vertices2[vertexIndex + 3] = matrix.MultiplyPoint3x4(vertices2[vertexIndex + 3]);
							vertices2[vertexIndex] += b2;
							vertices2[vertexIndex + 1] += b2;
							vertices2[vertexIndex + 2] += b2;
							vertices2[vertexIndex + 3] += b2;
							Vector2[] uvs = cachedMeshInfoVertexData[materialReferenceIndex].uvs0;
							Vector2[] uvs2 = textInfo.meshInfo[materialReferenceIndex].uvs0;
							uvs2[vertexIndex] = uvs[vertexIndex];
							uvs2[vertexIndex + 1] = uvs[vertexIndex + 1];
							uvs2[vertexIndex + 2] = uvs[vertexIndex + 2];
							uvs2[vertexIndex + 3] = uvs[vertexIndex + 3];
							Color32[] colors = cachedMeshInfoVertexData[materialReferenceIndex].colors32;
							Color32[] colors2 = textInfo.meshInfo[materialReferenceIndex].colors32;
							colors2[vertexIndex] = colors[vertexIndex];
							colors2[vertexIndex + 1] = colors[vertexIndex + 1];
							colors2[vertexIndex + 2] = colors[vertexIndex + 2];
							colors2[vertexIndex + 3] = colors[vertexIndex + 3];
						}
					}
					for (int j = 0; j < textInfo.meshInfo.Length; j++)
					{
						scaleSortingOrder.Sort((int a, int b) => modifiedCharScale[a].CompareTo(modifiedCharScale[b]));
						textInfo.meshInfo[j].SortGeometry(scaleSortingOrder);
						textInfo.meshInfo[j].mesh.vertices = textInfo.meshInfo[j].vertices;
						textInfo.meshInfo[j].mesh.uv = textInfo.meshInfo[j].uvs0;
						textInfo.meshInfo[j].mesh.colors32 = textInfo.meshInfo[j].colors32;
						this.m_TextComponent.UpdateGeometry(textInfo.meshInfo[j].mesh, j);
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
					this.$locvar0 = new VertexZoom.<AnimateVertexColors>c__Iterator0.<AnimateVertexColors>c__AnonStorey1();
					this.$locvar0.<>f__ref$0 = this;
					this.$this.m_TextComponent.ForceMeshUpdate();
					this.<textInfo>__0 = this.$this.m_TextComponent.textInfo;
					this.<cachedMeshInfoVertexData>__0 = this.<textInfo>__0.CopyMeshInfoVertexData();
					this.$locvar0.modifiedCharScale = new List<float>();
					this.<scaleSortingOrder>__0 = new List<int>();
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
					this.<cachedMeshInfoVertexData>__0 = this.<textInfo>__0.CopyMeshInfoVertexData();
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
					this.$locvar0.modifiedCharScale.Clear();
					this.<scaleSortingOrder>__0.Clear();
					for (int i = 0; i < this.<characterCount>__1; i++)
					{
						TMP_CharacterInfo tmp_CharacterInfo = this.<textInfo>__0.characterInfo[i];
						if (tmp_CharacterInfo.isVisible)
						{
							int materialReferenceIndex = this.<textInfo>__0.characterInfo[i].materialReferenceIndex;
							int vertexIndex = this.<textInfo>__0.characterInfo[i].vertexIndex;
							Vector3[] vertices = this.<cachedMeshInfoVertexData>__0[materialReferenceIndex].vertices;
							Vector2 v = (vertices[vertexIndex] + vertices[vertexIndex + 2]) / 2f;
							Vector3 b2 = v;
							Vector3[] vertices2 = this.<textInfo>__0.meshInfo[materialReferenceIndex].vertices;
							vertices2[vertexIndex] = vertices[vertexIndex] - b2;
							vertices2[vertexIndex + 1] = vertices[vertexIndex + 1] - b2;
							vertices2[vertexIndex + 2] = vertices[vertexIndex + 2] - b2;
							vertices2[vertexIndex + 3] = vertices[vertexIndex + 3] - b2;
							float num2 = UnityEngine.Random.Range(1f, 1.5f);
							this.$locvar0.modifiedCharScale.Add(num2);
							this.<scaleSortingOrder>__0.Add(this.$locvar0.modifiedCharScale.Count - 1);
							this.<matrix>__2 = Matrix4x4.TRS(new Vector3(0f, 0f, 0f), Quaternion.identity, Vector3.one * num2);
							vertices2[vertexIndex] = this.<matrix>__2.MultiplyPoint3x4(vertices2[vertexIndex]);
							vertices2[vertexIndex + 1] = this.<matrix>__2.MultiplyPoint3x4(vertices2[vertexIndex + 1]);
							vertices2[vertexIndex + 2] = this.<matrix>__2.MultiplyPoint3x4(vertices2[vertexIndex + 2]);
							vertices2[vertexIndex + 3] = this.<matrix>__2.MultiplyPoint3x4(vertices2[vertexIndex + 3]);
							vertices2[vertexIndex] += b2;
							vertices2[vertexIndex + 1] += b2;
							vertices2[vertexIndex + 2] += b2;
							vertices2[vertexIndex + 3] += b2;
							Vector2[] uvs = this.<cachedMeshInfoVertexData>__0[materialReferenceIndex].uvs0;
							Vector2[] uvs2 = this.<textInfo>__0.meshInfo[materialReferenceIndex].uvs0;
							uvs2[vertexIndex] = uvs[vertexIndex];
							uvs2[vertexIndex + 1] = uvs[vertexIndex + 1];
							uvs2[vertexIndex + 2] = uvs[vertexIndex + 2];
							uvs2[vertexIndex + 3] = uvs[vertexIndex + 3];
							Color32[] colors = this.<cachedMeshInfoVertexData>__0[materialReferenceIndex].colors32;
							Color32[] colors2 = this.<textInfo>__0.meshInfo[materialReferenceIndex].colors32;
							colors2[vertexIndex] = colors[vertexIndex];
							colors2[vertexIndex + 1] = colors[vertexIndex + 1];
							colors2[vertexIndex + 2] = colors[vertexIndex + 2];
							colors2[vertexIndex + 3] = colors[vertexIndex + 3];
						}
					}
					for (int j = 0; j < this.<textInfo>__0.meshInfo.Length; j++)
					{
						this.<scaleSortingOrder>__0.Sort((int a, int b) => this.$locvar0.modifiedCharScale[a].CompareTo(this.$locvar0.modifiedCharScale[b]));
						this.<textInfo>__0.meshInfo[j].SortGeometry(this.<scaleSortingOrder>__0);
						this.<textInfo>__0.meshInfo[j].mesh.vertices = this.<textInfo>__0.meshInfo[j].vertices;
						this.<textInfo>__0.meshInfo[j].mesh.uv = this.<textInfo>__0.meshInfo[j].uvs0;
						this.<textInfo>__0.meshInfo[j].mesh.colors32 = this.<textInfo>__0.meshInfo[j].colors32;
						this.$this.m_TextComponent.UpdateGeometry(this.<textInfo>__0.meshInfo[j].mesh, j);
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

			internal TMP_MeshInfo[] <cachedMeshInfoVertexData>__0;

			internal List<int> <scaleSortingOrder>__0;

			internal int <characterCount>__1;

			internal Matrix4x4 <matrix>__2;

			internal VertexZoom $this;

			internal object $current;

			internal bool $disposing;

			internal int $PC;

			private VertexZoom.<AnimateVertexColors>c__Iterator0.<AnimateVertexColors>c__AnonStorey1 $locvar0;

			private sealed class <AnimateVertexColors>c__AnonStorey1
			{
				public <AnimateVertexColors>c__AnonStorey1()
				{
				}

				internal int <>m__0(int a, int b)
				{
					return this.modifiedCharScale[a].CompareTo(this.modifiedCharScale[b]);
				}

				internal List<float> modifiedCharScale;

				internal VertexZoom.<AnimateVertexColors>c__Iterator0 <>f__ref$0;
			}
		}
	}
}
