using System;

namespace UnityEngine.PostProcessing
{
	public abstract class PostProcessingComponentBase
	{
		public abstract bool active { get; }

		protected PostProcessingComponentBase()
		{
		}

		public virtual DepthTextureMode GetCameraFlags()
		{
			return DepthTextureMode.None;
		}

		public virtual void OnEnable()
		{
		}

		public virtual void OnDisable()
		{
		}

		public abstract PostProcessingModel GetModel();

		public PostProcessingContext context;
	}
}
