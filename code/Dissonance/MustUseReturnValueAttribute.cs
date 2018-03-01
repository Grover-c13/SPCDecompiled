using System;
using System.Runtime.CompilerServices;

namespace Dissonance
{
	[AttributeUsage(AttributeTargets.Method)]
	internal sealed class MustUseReturnValueAttribute : Attribute
	{
		public MustUseReturnValueAttribute()
		{
		}

		public MustUseReturnValueAttribute([NotNull] string justification)
		{
			this.Justification = justification;
		}

		[CanBeNull]
		public string Justification
		{
			[CompilerGenerated]
			get
			{
				return this.<Justification>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<Justification>k__BackingField = value;
			}
		}

		[CompilerGenerated]
		private string <Justification>k__BackingField;
	}
}
