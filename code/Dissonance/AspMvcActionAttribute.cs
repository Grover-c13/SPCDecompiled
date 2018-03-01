using System;
using System.Runtime.CompilerServices;

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
