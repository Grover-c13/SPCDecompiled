using System;

namespace Dissonance
{
	[AttributeUsage(AttributeTargets.Parameter)]
	internal sealed class AspMvcAreaAttribute : Attribute
	{
		public AspMvcAreaAttribute()
		{
		}

		public AspMvcAreaAttribute([NotNull] string anonymousProperty)
		{
			this.AnonymousProperty = anonymousProperty;
		}

		[CanBeNull]
		public string AnonymousProperty { get; private set; }
	}
}
