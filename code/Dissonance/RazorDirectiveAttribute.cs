using System;

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
		public string Directive { get; private set; }
	}
}
