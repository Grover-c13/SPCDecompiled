using System;

namespace Dissonance
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
	internal sealed class HtmlAttributeValueAttribute : Attribute
	{
		public HtmlAttributeValueAttribute([NotNull] string name)
		{
			this.Name = name;
		}

		[NotNull]
		public string Name { get; private set; }
	}
}
