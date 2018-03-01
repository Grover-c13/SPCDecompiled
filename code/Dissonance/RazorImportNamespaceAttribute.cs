using System;
using System.Runtime.CompilerServices;

namespace Dissonance
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	internal sealed class RazorImportNamespaceAttribute : Attribute
	{
		public RazorImportNamespaceAttribute([NotNull] string name)
		{
			this.Name = name;
		}

		[NotNull]
		public string Name
		{
			[CompilerGenerated]
			get
			{
				return this.<Name>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<Name>k__BackingField = value;
			}
		}

		[CompilerGenerated]
		private string <Name>k__BackingField;
	}
}
