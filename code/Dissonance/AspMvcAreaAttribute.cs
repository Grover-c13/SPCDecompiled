using System;
using System.Runtime.CompilerServices;

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
		public string AnonymousProperty
		{
			[CompilerGenerated]
			get
			{
				return this.<AnonymousProperty>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<AnonymousProperty>k__BackingField = value;
			}
		}

		[CompilerGenerated]
		private string <AnonymousProperty>k__BackingField;
	}
}
