using System;
using UnityEngine;

namespace TMPro.Examples
{
	public class SimpleScript : MonoBehaviour
	{
		private void Start()
		{
			this.m_textMeshPro = base.gameObject.AddComponent<TextMeshPro>();
			this.m_textMeshPro.autoSizeTextContainer = true;
			this.m_textMeshPro.fontSize = 48f;
			this.m_textMeshPro.alignment = TextAlignmentOptions.Center;
			this.m_textMeshPro.enableWordWrapping = false;
		}

		private void Update()
		{
			this.m_textMeshPro.SetText("The <#0050FF>count is: </color>{0:2}", this.m_frame % 1000f);
			this.m_frame += 1f * Time.deltaTime;
		}

		private TextMeshPro m_textMeshPro;

		private const string label = "The <#0050FF>count is: </color>{0:2}";

		private float m_frame;
	}
}
