using System;

namespace Dissonance
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
	internal sealed class HtmlElementAttributesAttribute : Attribute
	{
		public HtmlElementAttributesAttribute()
		{
		}

		public HtmlElementAttributesAttribute([NotNull] string name)
		{
			this.Name = name;
		}

		[CanBeNull]
		public string Name { get; private set; }
	}
}
