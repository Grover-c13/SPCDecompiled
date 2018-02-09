using System;

namespace Dissonance
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Parameter)]
	internal sealed class AspMvcActionAttribute : Attribute
	{
		public AspMvcActionAttribute()
		{
		}

		public AspMvcActionAttribute([NotNull] string anonymousProperty)
		{
			this.AnonymousProperty = anonymousProperty;
		}

		[CanBeNull]
		public string AnonymousProperty { get; private set; }
	}
}
