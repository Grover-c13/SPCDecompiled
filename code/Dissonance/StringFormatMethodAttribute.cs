using System;
using System.Runtime.CompilerServices;

namespace Dissonance
{
	[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Delegate)]
	internal sealed class StringFormatMethodAttribute : Attribute
	{
		public StringFormatMethodAttribute([NotNull] string formatParameterName)
		{
			this.FormatParameterName = formatParameterName;
		}

		[NotNull]
		public string FormatParameterName
		{
			[CompilerGenerated]
			get
			{
				return this.<FormatParameterName>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<FormatParameterName>k__BackingField = value;
			}
		}

		[CompilerGenerated]
		private string <FormatParameterName>k__BackingField;
	}
}
