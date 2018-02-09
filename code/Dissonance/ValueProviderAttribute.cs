using System;

namespace Dissonance
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = true)]
	internal sealed class ValueProviderAttribute : Attribute
	{
		public ValueProviderAttribute([NotNull] string name)
		{
			this.Name = name;
		}

		[NotNull]
		public string Name { get; private set; }
	}
}
