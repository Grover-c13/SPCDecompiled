using System;
using System.Runtime.CompilerServices;

namespace Dissonance
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	internal sealed class RazorDirectiveAttribute : Attribute
	{
		public RazorDirectiveAttribute([NotNull] string directive)
		{
			this.Directive = directive;
		}

		[NotNull]
		public string Directive
		{
			[CompilerGenerated]
			get
			{
				return this.<Directive>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<Directive>k__BackingField = value;
			}
		}

		[CompilerGenerated]
		private string <Directive>k__BackingField;
	}
}
