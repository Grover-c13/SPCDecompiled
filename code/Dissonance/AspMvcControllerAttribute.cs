using System;

namespace Dissonance
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Parameter)]
	internal sealed class AspMvcControllerAttribute : Attribute
	{
		public AspMvcControllerAttribute()
		{
		}

		public AspMvcControllerAttribute([NotNull] string anonymousProperty)
		{
			this.AnonymousProperty = anonymousProperty;
		}

		[CanBeNull]
		public string AnonymousProperty { get; private set; }
	}
}
