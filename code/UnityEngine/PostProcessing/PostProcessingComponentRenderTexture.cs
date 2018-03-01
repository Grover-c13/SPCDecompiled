using System;

namespace UnityEngine.PostProcessing
{
	public abstract class PostProcessingComponentRenderTexture<T> : PostProcessingComponent<T> where T : PostProcessingModel
	{
		protected PostProcessingComponentRenderTexture()
		{
		}

		public virtual void Prepare(Material material)
		{
		}
	}
}
