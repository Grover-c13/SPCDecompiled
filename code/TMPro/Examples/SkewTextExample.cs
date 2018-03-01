using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace TMPro.Examples
{
	public class SkewTextExample : MonoBehaviour
	{
		public SkewTextExample()
		{
		}

		private void Awake()
		{
			this.m_TextComponent = base.gameObject.GetComponent<TMP_Text>();
		}

		private void Start()
		{
			base.StartCoroutine(this.WarpText());
		}

		private AnimationCurve CopyAnimationCurve(AnimationCurve curve)
		{
			return new AnimationCurve
			{
				keys = curve.keys
			};
		}

		private IEnumerator WarpText()
		{
			this.VertexCurve.preWrapMode = WrapMode.Once;
			this.VertexCurve.postWrapMode = WrapMode.Once;
			this.m_TextComponent.havePropertiesChanged = true;
			this.CurveScale *= 10f;
			float old_CurveScale = this.CurveScale;
			float old_ShearValue = this.ShearAmount;
			AnimationCurve old_curve = this.CopyAnimationCurve(this.VertexCurve);
			for (;;)
			{
				if (!this.m_TextComponent.havePropertiesChanged && old_CurveScale == this.CurveScale && old_curve.keys[1].value == this.VertexCurve.keys[1].value && old_ShearValue == this.ShearAmount)
				{
					yield return null;
				}
				else
				{
					old_CurveScale = this.CurveScale;
					old_curve = this.CopyAnimationCurve(this.VertexCurve);
					old_ShearValue = this.ShearAmount;
					this.m_TextComponent.ForceMeshUpdate();
					TMP_TextInfo textInfo = this.m_TextComponent.textInfo;
					int characterCount = textInfo.characterCount;
					if (characterCount != 0)
					{
						float boundsMinX = this.m_TextComponent.bounds.min.x;
						float boundsMaxX = this.m_TextComponent.bounds.max.x;
						for (int i = 0; i < characterCount; i++)
						{
							if (textInfo.characterInfo[i].isVisible)
							{
								int vertexIndex = textInfo.characterInfo[i].vertexIndex;
								int materialReferenceIndex = textInfo.characterInfo[i].materialReferenceIndex;
								Vector3[] vertices = textInfo.meshInfo[materialReferenceIndex].vertices;
								Vector3 vector = new Vector2((vertices[vertexIndex].x + vertices[vertexIndex + 2].x) / 2f, textInfo.characterInfo[i].baseLine);
								vertices[vertexIndex] += -vector;
								vertices[vertexIndex + 1] += -vector;
								vertices[vertexIndex + 2] += -vector;
								vertices[vertexIndex + 3] += -vector;
								float num = this.ShearAmount * 0.01f;
								Vector3 b = new Vector3(num * (textInfo.characterInfo[i].topRight.y - textInfo.characterInfo[i].baseLine), 0f, 0f);
								Vector3 a = new Vector3(num * (textInfo.characterInfo[i].baseLine - textInfo.characterInfo[i].bottomRight.y), 0f, 0f);
								vertices[vertexIndex] += -a;
								vertices[vertexIndex + 1] += b;
								vertices[vertexIndex + 2] += b;
								vertices[vertexIndex + 3] += -a;
								float num2 = (vector.x - boundsMinX) / (boundsMaxX - boundsMinX);
								float num3 = num2 + 0.0001f;
								float y = this.VertexCurve.Evaluate(num2) * this.CurveScale;
								float y2 = this.VertexCurve.Evaluate(num3) * this.CurveScale;
								Vector3 lhs = new Vector3(1f, 0f, 0f);
								Vector3 rhs = new Vector3(num3 * (boundsMaxX - boundsMinX) + boundsMinX, y2) - new Vector3(vector.x, y);
								float num4 = Mathf.Acos(Vector3.Dot(lhs, rhs.normalized)) * 57.29578f;
								float z = (Vector3.Cross(lhs, rhs).z <= 0f) ? (360f - num4) : num4;
								Matrix4x4 matrix = Matrix4x4.TRS(new Vector3(0f, y, 0f), Quaternion.Euler(0f, 0f, z), Vector3.one);
								vertices[vertexIndex] = matrix.MultiplyPoint3x4(vertices[vertexIndex]);
								vertices[vertexIndex + 1] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 1]);
								vertices[vertexIndex + 2] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 2]);
								vertices[vertexIndex + 3] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 3]);
								vertices[vertexIndex] += vector;
								vertices[vertexIndex + 1] += vector;
								vertices[vertexIndex + 2] += vector;
								vertices[vertexIndex + 3] += vector;
							}
						}
						this.m_TextComponent.UpdateVertexData();
						yield return null;
					}
				}
			}
			yield break;
		}

		private TMP_Text m_TextComponent;

		public AnimationCurve VertexCurve = new AnimationCurve(new Keyframe[]
		{
			new Keyframe(0f, 0f),
			new Keyframe(0.25f, 2f),
			new Keyframe(0.5f, 0f),
			new Keyframe(0.75f, 2f),
			new Keyframe(1f, 0f)
		});

		public float CurveScale = 1f;

		public float ShearAmount = 1f;

		[CompilerGenerated]
		private sealed class <WarpText>c__Iterator0 : IEnumerator, IDisposable, IEnumerator<object>
		{
			[DebuggerHidden]
			public <WarpText>c__Iterator0()
			{
			}

			public bool MoveNext()
			{
				uint num = (uint)this.$PC;
				this.$PC = -1;
				switch (num)
				{
				case 0u:
					this.$this.VertexCurve.preWrapMode = WrapMode.Once;
					this.$this.VertexCurve.postWrapMode = WrapMode.Once;
					this.$this.m_TextComponent.havePropertiesChanged = true;
					this.$this.CurveScale *= 10f;
					this.<old_CurveScale>__0 = this.$this.CurveScale;
					this.<old_ShearValue>__0 = this.$this.ShearAmount;
					this.<old_curve>__0 = this.$this.CopyAnimationCurve(this.$this.VertexCurve);
					break;
				case 1u:
					break;
				case 2u:
					break;
				default:
					return false;
				}
				while (this.$this.m_TextComponent.havePropertiesChanged || this.<old_CurveScale>__0 != this.$this.CurveScale || this.<old_curve>__0.keys[1].value != this.$this.VertexCurve.keys[1].value || this.<old_ShearValue>__0 != this.$this.ShearAmount)
				{
					this.<old_CurveScale>__0 = this.$this.CurveScale;
					this.<old_curve>__0 = this.$this.CopyAnimationCurve(this.$this.VertexCurve);
					this.<old_ShearValue>__0 = this.$this.ShearAmount;
					this.$this.m_TextComponent.ForceMeshUpdate();
					this.<textInfo>__1 = this.$this.m_TextComponent.textInfo;
					this.<characterCount>__1 = this.<textInfo>__1.characterCount;
					if (this.<characterCount>__1 != 0)
					{
						this.<boundsMinX>__1 = this.$this.m_TextComponent.bounds.min.x;
						this.<boundsMaxX>__1 = this.$this.m_TextComponent.bounds.max.x;
						for (int i = 0; i < this.<characterCount>__1; i++)
						{
							if (this.<textInfo>__1.characterInfo[i].isVisible)
							{
								int vertexIndex = this.<textInfo>__1.characterInfo[i].vertexIndex;
								int materialReferenceIndex = this.<textInfo>__1.characterInfo[i].materialReferenceIndex;
								this.<vertices>__2 = this.<textInfo>__1.meshInfo[materialReferenceIndex].vertices;
								Vector3 vector = new Vector2((this.<vertices>__2[vertexIndex].x + this.<vertices>__2[vertexIndex + 2].x) / 2f, this.<textInfo>__1.characterInfo[i].baseLine);
								this.<vertices>__2[vertexIndex] += -vector;
								this.<vertices>__2[vertexIndex + 1] += -vector;
								this.<vertices>__2[vertexIndex + 2] += -vector;
								this.<vertices>__2[vertexIndex + 3] += -vector;
								float num2 = this.$this.ShearAmount * 0.01f;
								Vector3 b = new Vector3(num2 * (this.<textInfo>__1.characterInfo[i].topRight.y - this.<textInfo>__1.characterInfo[i].baseLine), 0f, 0f);
								Vector3 a = new Vector3(num2 * (this.<textInfo>__1.characterInfo[i].baseLine - this.<textInfo>__1.characterInfo[i].bottomRight.y), 0f, 0f);
								this.<vertices>__2[vertexIndex] += -a;
								this.<vertices>__2[vertexIndex + 1] += b;
								this.<vertices>__2[vertexIndex + 2] += b;
								this.<vertices>__2[vertexIndex + 3] += -a;
								float num3 = (vector.x - this.<boundsMinX>__1) / (this.<boundsMaxX>__1 - this.<boundsMinX>__1);
								float num4 = num3 + 0.0001f;
								float y = this.$this.VertexCurve.Evaluate(num3) * this.$this.CurveScale;
								float y2 = this.$this.VertexCurve.Evaluate(num4) * this.$this.CurveScale;
								Vector3 lhs = new Vector3(1f, 0f, 0f);
								Vector3 rhs = new Vector3(num4 * (this.<boundsMaxX>__1 - this.<boundsMinX>__1) + this.<boundsMinX>__1, y2) - new Vector3(vector.x, y);
								float num5 = Mathf.Acos(Vector3.Dot(lhs, rhs.normalized)) * 57.29578f;
								float z = (Vector3.Cross(lhs, rhs).z <= 0f) ? (360f - num5) : num5;
								this.<matrix>__2 = Matrix4x4.TRS(new Vector3(0f, y, 0f), Quaternion.Euler(0f, 0f, z), Vector3.one);
								this.<vertices>__2[vertexIndex] = this.<matrix>__2.MultiplyPoint3x4(this.<vertices>__2[vertexIndex]);
								this.<vertices>__2[vertexIndex + 1] = this.<matrix>__2.MultiplyPoint3x4(this.<vertices>__2[vertexIndex + 1]);
								this.<vertices>__2[vertexIndex + 2] = this.<matrix>__2.MultiplyPoint3x4(this.<vertices>__2[vertexIndex + 2]);
								this.<vertices>__2[vertexIndex + 3] = this.<matrix>__2.MultiplyPoint3x4(this.<vertices>__2[vertexIndex + 3]);
								this.<vertices>__2[vertexIndex] += vector;
								this.<vertices>__2[vertexIndex + 1] += vector;
								this.<vertices>__2[vertexIndex + 2] += vector;
								this.<vertices>__2[vertexIndex + 3] += vector;
							}
						}
						this.$this.m_TextComponent.UpdateVertexData();
						this.$current = null;
						if (!this.$disposing)
						{
							this.$PC = 2;
						}
						return true;
					}
				}
				this.$current = null;
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

			internal float <old_CurveScale>__0;

			internal float <old_ShearValue>__0;

			internal AnimationCurve <old_curve>__0;

			internal TMP_TextInfo <textInfo>__1;

			internal int <characterCount>__1;

			internal float <boundsMinX>__1;

			internal float <boundsMaxX>__1;

			internal Vector3[] <vertices>__2;

			internal Matrix4x4 <matrix>__2;

			internal SkewTextExample $this;

			internal object $current;

			internal bool $disposing;

			internal int $PC;
		}
	}
}
