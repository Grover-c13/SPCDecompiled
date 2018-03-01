using System;
using System.Runtime.CompilerServices;

namespace Dissonance
{
	[MeansImplicitUse(ImplicitUseTargetFlags.WithMembers)]
	internal sealed class PublicAPIAttribute : Attribute
	{
		public PublicAPIAttribute()
		{
		}

		public PublicAPIAttribute([NotNull] string comment)
		{
			this.Comment = comment;
		}

		[CanBeNull]
		public string Comment
		{
			[CompilerGenerated]
			get
			{
				return this.<Comment>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<Comment>k__BackingField = value;
			}
		}

		[CompilerGenerated]
		private string <Comment>k__BackingField;
	}
}
