using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace TMPro.Examples
{
	public class TextMeshProFloatingText : MonoBehaviour
	{
		public TextMeshProFloatingText()
		{
		}

		private void Awake()
		{
			this.m_transform = base.transform;
			this.m_floatingText = new GameObject(base.name + " floating text");
			this.m_cameraTransform = Camera.main.transform;
		}

		private void Start()
		{
			if (this.SpawnType == 0)
			{
				this.m_textMeshPro = this.m_floatingText.AddComponent<TextMeshPro>();
				this.m_textMeshPro.rectTransform.sizeDelta = new Vector2(3f, 3f);
				this.m_floatingText_Transform = this.m_floatingText.transform;
				this.m_floatingText_Transform.position = this.m_transform.position + new Vector3(0f, 15f, 0f);
				this.m_textMeshPro.alignment = TextAlignmentOptions.Center;
				this.m_textMeshPro.color = new Color32((byte)UnityEngine.Random.Range(0, 255), (byte)UnityEngine.Random.Range(0, 255), (byte)UnityEngine.Random.Range(0, 255), byte.MaxValue);
				this.m_textMeshPro.fontSize = 24f;
				this.m_textMeshPro.text = string.Empty;
				base.StartCoroutine(this.DisplayTextMeshProFloatingText());
			}
			else if (this.SpawnType == 1)
			{
				this.m_floatingText_Transform = this.m_floatingText.transform;
				this.m_floatingText_Transform.position = this.m_transform.position + new Vector3(0f, 15f, 0f);
				this.m_textMesh = this.m_floatingText.AddComponent<TextMesh>();
				this.m_textMesh.font = (Resources.Load("Fonts/ARIAL", typeof(Font)) as Font);
				this.m_textMesh.GetComponent<Renderer>().sharedMaterial = this.m_textMesh.font.material;
				this.m_textMesh.color = new Color32((byte)UnityEngine.Random.Range(0, 255), (byte)UnityEngine.Random.Range(0, 255), (byte)UnityEngine.Random.Range(0, 255), byte.MaxValue);
				this.m_textMesh.anchor = TextAnchor.LowerCenter;
				this.m_textMesh.fontSize = 24;
				base.StartCoroutine(this.DisplayTextMeshFloatingText());
			}
			else if (this.SpawnType == 2)
			{
			}
		}

		public IEnumerator DisplayTextMeshProFloatingText()
		{
			float CountDuration = 2f;
			float starting_Count = UnityEngine.Random.Range(5f, 20f);
			float current_Count = starting_Count;
			Vector3 start_pos = this.m_floatingText_Transform.position;
			Color32 start_color = this.m_textMeshPro.color;
			float alpha = 255f;
			float fadeDuration = 3f / starting_Count * CountDuration;
			while (current_Count > 0f)
			{
				current_Count -= Time.deltaTime / CountDuration * starting_Count;
				if (current_Count <= 3f)
				{
					alpha = Mathf.Clamp(alpha - Time.deltaTime / fadeDuration * 255f, 0f, 255f);
				}
				this.m_textMeshPro.SetText("{0}", (float)((int)current_Count));
				this.m_textMeshPro.color = new Color32(start_color.r, start_color.g, start_color.b, (byte)alpha);
				this.m_floatingText_Transform.position += new Vector3(0f, starting_Count * Time.deltaTime, 0f);
				if (!this.lastPOS.Compare(this.m_cameraTransform.position, 1000) || !this.lastRotation.Compare(this.m_cameraTransform.rotation, 1000))
				{
					this.lastPOS = this.m_cameraTransform.position;
					this.lastRotation = this.m_cameraTransform.rotation;
					this.m_floatingText_Transform.rotation = this.lastRotation;
					Vector3 vector = this.m_transform.position - this.lastPOS;
					this.m_transform.forward = new Vector3(vector.x, 0f, vector.z);
				}
				yield return new WaitForEndOfFrame();
			}
			yield return new WaitForSeconds(UnityEngine.Random.Range(0.1f, 1f));
			this.m_floatingText_Transform.position = start_pos;
			base.StartCoroutine(this.DisplayTextMeshProFloatingText());
			yield break;
		}

		public IEnumerator DisplayTextMeshFloatingText()
		{
			float CountDuration = 2f;
			float starting_Count = UnityEngine.Random.Range(5f, 20f);
			float current_Count = starting_Count;
			Vector3 start_pos = this.m_floatingText_Transform.position;
			Color32 start_color = this.m_textMesh.color;
			float alpha = 255f;
			int int_counter = 0;
			float fadeDuration = 3f / starting_Count * CountDuration;
			while (current_Count > 0f)
			{
				current_Count -= Time.deltaTime / CountDuration * starting_Count;
				if (current_Count <= 3f)
				{
					alpha = Mathf.Clamp(alpha - Time.deltaTime / fadeDuration * 255f, 0f, 255f);
				}
				int_counter = (int)current_Count;
				this.m_textMesh.text = int_counter.ToString();
				this.m_textMesh.color = new Color32(start_color.r, start_color.g, start_color.b, (byte)alpha);
				this.m_floatingText_Transform.position += new Vector3(0f, starting_Count * Time.deltaTime, 0f);
				if (!this.lastPOS.Compare(this.m_cameraTransform.position, 1000) || !this.lastRotation.Compare(this.m_cameraTransform.rotation, 1000))
				{
					this.lastPOS = this.m_cameraTransform.position;
					this.lastRotation = this.m_cameraTransform.rotation;
					this.m_floatingText_Transform.rotation = this.lastRotation;
					Vector3 vector = this.m_transform.position - this.lastPOS;
					this.m_transform.forward = new Vector3(vector.x, 0f, vector.z);
				}
				yield return new WaitForEndOfFrame();
			}
			yield return new WaitForSeconds(UnityEngine.Random.Range(0.1f, 1f));
			this.m_floatingText_Transform.position = start_pos;
			base.StartCoroutine(this.DisplayTextMeshFloatingText());
			yield break;
		}

		public Font TheFont;

		private GameObject m_floatingText;

		private TextMeshPro m_textMeshPro;

		private TextMesh m_textMesh;

		private Transform m_transform;

		private Transform m_floatingText_Transform;

		private Transform m_cameraTransform;

		private Vector3 lastPOS = Vector3.zero;

		private Quaternion lastRotation = Quaternion.identity;

		public int SpawnType;

		[CompilerGenerated]
		private sealed class <DisplayTextMeshProFloatingText>c__Iterator0 : IEnumerator, IDisposable, IEnumerator<object>
		{
			[DebuggerHidden]
			public <DisplayTextMeshProFloatingText>c__Iterator0()
			{
			}

			public bool MoveNext()
			{
				uint num = (uint)this.$PC;
				this.$PC = -1;
				switch (num)
				{
				case 0u:
					this.<CountDuration>__0 = 2f;
					this.<starting_Count>__0 = UnityEngine.Random.Range(5f, 20f);
					this.<current_Count>__0 = this.<starting_Count>__0;
					this.<start_pos>__0 = this.$this.m_floatingText_Transform.position;
					this.<start_color>__0 = this.$this.m_textMeshPro.color;
					this.<alpha>__0 = 255f;
					this.<fadeDuration>__0 = 3f / this.<starting_Count>__0 * this.<CountDuration>__0;
					break;
				case 1u:
					break;
				case 2u:
					this.$this.m_floatingText_Transform.position = this.<start_pos>__0;
					this.$this.StartCoroutine(this.$this.DisplayTextMeshProFloatingText());
					this.$PC = -1;
					return false;
				default:
					return false;
				}
				if (this.<current_Count>__0 <= 0f)
				{
					this.$current = new WaitForSeconds(UnityEngine.Random.Range(0.1f, 1f));
					if (!this.$disposing)
					{
						this.$PC = 2;
					}
				}
				else
				{
					this.<current_Count>__0 -= Time.deltaTime / this.<CountDuration>__0 * this.<starting_Count>__0;
					if (this.<current_Count>__0 <= 3f)
					{
						this.<alpha>__0 = Mathf.Clamp(this.<alpha>__0 - Time.deltaTime / this.<fadeDuration>__0 * 255f, 0f, 255f);
					}
					this.$this.m_textMeshPro.SetText("{0}", (float)((int)this.<current_Count>__0));
					this.$this.m_textMeshPro.color = new Color32(this.<start_color>__0.r, this.<start_color>__0.g, this.<start_color>__0.b, (byte)this.<alpha>__0);
					this.$this.m_floatingText_Transform.position += new Vector3(0f, this.<starting_Count>__0 * Time.deltaTime, 0f);
					if (!this.$this.lastPOS.Compare(this.$this.m_cameraTransform.position, 1000) || !this.$this.lastRotation.Compare(this.$this.m_cameraTransform.rotation, 1000))
					{
						this.$this.lastPOS = this.$this.m_cameraTransform.position;
						this.$this.lastRotation = this.$this.m_cameraTransform.rotation;
						this.$this.m_floatingText_Transform.rotation = this.$this.lastRotation;
						Vector3 vector = this.$this.m_transform.position - this.$this.lastPOS;
						this.$this.m_transform.forward = new Vector3(vector.x, 0f, vector.z);
					}
					this.$current = new WaitForEndOfFrame();
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

			internal float <CountDuration>__0;

			internal float <starting_Count>__0;

			internal float <current_Count>__0;

			internal Vector3 <start_pos>__0;

			internal Color32 <start_color>__0;

			internal float <alpha>__0;

			internal float <fadeDuration>__0;

			internal TextMeshProFloatingText $this;

			internal object $current;

			internal bool $disposing;

			internal int $PC;
		}

		[CompilerGenerated]
		private sealed class <DisplayTextMeshFloatingText>c__Iterator1 : IEnumerator, IDisposable, IEnumerator<object>
		{
			[DebuggerHidden]
			public <DisplayTextMeshFloatingText>c__Iterator1()
			{
			}

			public bool MoveNext()
			{
				uint num = (uint)this.$PC;
				this.$PC = -1;
				switch (num)
				{
				case 0u:
					this.<CountDuration>__0 = 2f;
					this.<starting_Count>__0 = UnityEngine.Random.Range(5f, 20f);
					this.<current_Count>__0 = this.<starting_Count>__0;
					this.<start_pos>__0 = this.$this.m_floatingText_Transform.position;
					this.<start_color>__0 = this.$this.m_textMesh.color;
					this.<alpha>__0 = 255f;
					this.<int_counter>__0 = 0;
					this.<fadeDuration>__0 = 3f / this.<starting_Count>__0 * this.<CountDuration>__0;
					break;
				case 1u:
					break;
				case 2u:
					this.$this.m_floatingText_Transform.position = this.<start_pos>__0;
					this.$this.StartCoroutine(this.$this.DisplayTextMeshFloatingText());
					this.$PC = -1;
					return false;
				default:
					return false;
				}
				if (this.<current_Count>__0 <= 0f)
				{
					this.$current = new WaitForSeconds(UnityEngine.Random.Range(0.1f, 1f));
					if (!this.$disposing)
					{
						this.$PC = 2;
					}
				}
				else
				{
					this.<current_Count>__0 -= Time.deltaTime / this.<CountDuration>__0 * this.<starting_Count>__0;
					if (this.<current_Count>__0 <= 3f)
					{
						this.<alpha>__0 = Mathf.Clamp(this.<alpha>__0 - Time.deltaTime / this.<fadeDuration>__0 * 255f, 0f, 255f);
					}
					this.<int_counter>__0 = (int)this.<current_Count>__0;
					this.$this.m_textMesh.text = this.<int_counter>__0.ToString();
					this.$this.m_textMesh.color = new Color32(this.<start_color>__0.r, this.<start_color>__0.g, this.<start_color>__0.b, (byte)this.<alpha>__0);
					this.$this.m_floatingText_Transform.position += new Vector3(0f, this.<starting_Count>__0 * Time.deltaTime, 0f);
					if (!this.$this.lastPOS.Compare(this.$this.m_cameraTransform.position, 1000) || !this.$this.lastRotation.Compare(this.$this.m_cameraTransform.rotation, 1000))
					{
						this.$this.lastPOS = this.$this.m_cameraTransform.position;
						this.$this.lastRotation = this.$this.m_cameraTransform.rotation;
						this.$this.m_floatingText_Transform.rotation = this.$this.lastRotation;
						Vector3 vector = this.$this.m_transform.position - this.$this.lastPOS;
						this.$this.m_transform.forward = new Vector3(vector.x, 0f, vector.z);
					}
					this.$current = new WaitForEndOfFrame();
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

			internal float <CountDuration>__0;

			internal float <starting_Count>__0;

			internal float <current_Count>__0;

			internal Vector3 <start_pos>__0;

			internal Color32 <start_color>__0;

			internal float <alpha>__0;

			internal int <int_counter>__0;

			internal float <fadeDuration>__0;

			internal TextMeshProFloatingText $this;

			internal object $current;

			internal bool $disposing;

			internal int $PC;
		}
	}
}
