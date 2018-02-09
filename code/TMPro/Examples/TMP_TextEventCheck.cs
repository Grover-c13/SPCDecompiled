using System;
using UnityEngine;
using UnityEngine.Events;

namespace TMPro.Examples
{
	public class TMP_TextEventCheck : MonoBehaviour
	{
		private void OnEnable()
		{
			if (this.TextEventHandler != null)
			{
				this.TextEventHandler.onCharacterSelection.AddListener(new UnityAction<char, int>(this.OnCharacterSelection));
				this.TextEventHandler.onWordSelection.AddListener(new UnityAction<string, int, int>(this.OnWordSelection));
				this.TextEventHandler.onLineSelection.AddListener(new UnityAction<string, int, int>(this.OnLineSelection));
				this.TextEventHandler.onLinkSelection.AddListener(new UnityAction<string, string, int>(this.OnLinkSelection));
			}
		}

		private void OnDisable()
		{
			if (this.TextEventHandler != null)
			{
				this.TextEventHandler.onCharacterSelection.RemoveListener(new UnityAction<char, int>(this.OnCharacterSelection));
				this.TextEventHandler.onWordSelection.RemoveListener(new UnityAction<string, int, int>(this.OnWordSelection));
				this.TextEventHandler.onLineSelection.RemoveListener(new UnityAction<string, int, int>(this.OnLineSelection));
				this.TextEventHandler.onLinkSelection.RemoveListener(new UnityAction<string, string, int>(this.OnLinkSelection));
			}
		}

		private void OnCharacterSelection(char c, int index)
		{
			Debug.Log(string.Concat(new object[]
			{
				"Character [",
				c,
				"] at Index: ",
				index,
				" has been selected."
			}));
		}

		private void OnWordSelection(string word, int firstCharacterIndex, int length)
		{
			Debug.Log(string.Concat(new object[]
			{
				"Word [",
				word,
				"] with first character index of ",
				firstCharacterIndex,
				" and length of ",
				length,
				" has been selected."
			}));
		}

		private void OnLineSelection(string lineText, int firstCharacterIndex, int length)
		{
			Debug.Log(string.Concat(new object[]
			{
				"Line [",
				lineText,
				"] with first character index of ",
				firstCharacterIndex,
				" and length of ",
				length,
				" has been selected."
			}));
		}

		private void OnLinkSelection(string linkID, string linkText, int linkIndex)
		{
			Debug.Log(string.Concat(new object[]
			{
				"Link Index: ",
				linkIndex,
				" with ID [",
				linkID,
				"] and Text \"",
				linkText,
				"\" has been selected."
			}));
		}

		public TMP_TextEventHandler TextEventHandler;
	}
}
