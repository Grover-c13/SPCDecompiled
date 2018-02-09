using System;

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
		public string Comment { get; private set; }
	}
}
