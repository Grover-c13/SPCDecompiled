using System;

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
		public string FormatParameterName { get; private set; }
	}
}
