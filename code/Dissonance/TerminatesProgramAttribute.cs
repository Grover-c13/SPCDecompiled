using System;

namespace Dissonance
{
	[AttributeUsage(AttributeTargets.Method)]
	[Obsolete("Use [ContractAnnotation('=> halt')] instead")]
	internal sealed class TerminatesProgramAttribute : Attribute
	{
	}
}
