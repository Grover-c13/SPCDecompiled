using System;
using System.Runtime.CompilerServices;

namespace Dissonance
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.GenericParameter)]
	internal sealed class MeansImplicitUseAttribute : Attribute
	{
		public MeansImplicitUseAttribute() : this(ImplicitUseKindFlags.Default, ImplicitUseTargetFlags.Default)
		{
		}

		public MeansImplicitUseAttribute(ImplicitUseKindFlags useKindFlags) : this(useKindFlags, ImplicitUseTargetFlags.Default)
		{
		}

		public MeansImplicitUseAttribute(ImplicitUseTargetFlags targetFlags) : this(ImplicitUseKindFlags.Default, targetFlags)
		{
		}

		public MeansImplicitUseAttribute(ImplicitUseKindFlags useKindFlags, ImplicitUseTargetFlags targetFlags)
		{
			this.UseKindFlags = useKindFlags;
			this.TargetFlags = targetFlags;
		}

		[UsedImplicitly]
		public ImplicitUseKindFlags UseKindFlags
		{
			[CompilerGenerated]
			get
			{
				return this.<UseKindFlags>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<UseKindFlags>k__BackingField = value;
			}
		}

		[UsedImplicitly]
		public ImplicitUseTargetFlags TargetFlags
		{
			[CompilerGenerated]
			get
			{
				return this.<TargetFlags>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				this.<TargetFlags>k__BackingField = value;
			}
		}

		[CompilerGenerated]
		private ImplicitUseKindFlags <UseKindFlags>k__BackingField;

		[CompilerGenerated]
		private ImplicitUseTargetFlags <TargetFlags>k__BackingField;
	}
}
