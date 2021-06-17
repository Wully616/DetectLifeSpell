using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering.Universal;
using Wully.Helpers;

namespace Wully.Render.Pass {

	//TODO: look into this https://github.com/Unity-Technologies/Graphics/pull/1954/commits/70a83ed8060ec671473264e51049d93085f57f12
	[ExecuteInEditMode]
	public class CustomPassRenderObjects : CustomPass<RenderObjects> {

		public new static BetterLogger log = BetterLogger.GetLogger(typeof(CustomPassRenderObjects));

		public RenderObjects.RenderObjectsSettings settings = new RenderObjects.RenderObjectsSettings();
		public Color color = Color.magenta;
		
		public override void UpdateSettings() {
			if(settings == null || feature == null) { return; }
			
			if (feature is RenderObjects renderObjectsFeature) {
				
				//update anything on the material
				settings.overrideMaterial.SetColor("_Color", color);

				var material = renderObjectsFeature.settings.overrideMaterial;

				//check if the material was changed because then we need to disable then enable the feature
				bool restart = material != settings.overrideMaterial;

				//assign it back to the feature
				renderObjectsFeature.settings = settings;
				//restart if the feature is currently active
				if (restart && feature.isActive) {
					DisableFeature();
					EnableFeature();
				}
				
			}
			
			base.UpdateSettings();
		}
	}
}