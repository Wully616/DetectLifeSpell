using System;
using System.Collections;
using UnityEngine;
using ThunderRoad;
using UnityEngine.AddressableAssets;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Wully.Extensions;
using Wully.Helpers;
using Wully.Render;
using static Wully.Helpers.BetterLogger;
using static Wully.Helpers.BetterHelpers;
using VLB;
using Wully.Render.Pass;

namespace Wully.Module {
	/// <summary>
	/// This module inherits the LevelModuleGlobalItemModule so we can add a itemModule to all items.
	/// But it also utilizes the LevelModuleMasterLevel to add render features
	/// </summary>
	public class DetectLifeModule : LevelModule {

		public static BetterLogger log = BetterLogger.GetLogger(typeof(DetectLifeModule));

		// Configurables
		/// <summary>
		/// Enable/Disable Better Logging for  this module
		/// </summary>
		public bool enableLogging = true;
		/// <summary>
		/// Set the GetLogLevel for this module
		/// </summary>
		public LogLevel logLevel = LogLevel.Info;

		/// <summary>
		/// Local static reference to the currently loaded level module
		/// </summary>
		/// 
		public static DetectLifeModule local;

		public string layerMaskName = "NPC";
		public string materialAddress;
		public Material material;
		public Color color = Color.magenta;
		public override IEnumerator OnLoadCoroutine(Level level) {

			log.SetLoggingLevel(logLevel);
			log.DisableLogging();
			if ( enableLogging ) {
				log.EnableLogging();
			}

			if ( level.data.id.ToLower() == "master") {

				log.Info().Message($"Initialized Wully's DetectLife Mod");

				if ( local == null) {
					local = this;
				}

				
				// We use a level module which gives us access to the Master Level mono, so we can add 
				// customer render passes that are in a mono onto it

				//check if the module loaded first, otherwise use the event.
				
				if (LevelModuleMasterLevel.local != null) {
					OnCustomPassLevelModuleLoaded(LevelModuleMasterLevel.local);
				} else {
					LevelModuleMasterLevel.OnLevelModuleLoaded += OnCustomPassLevelModuleLoaded;
				}
				

			}

			yield break;

		}

		public CustomPassRenderObjects customPassRenderObjects;


		private void OnCustomPassLevelModuleLoaded( LevelModuleMasterLevel instance ) {

			var op = Addressables.LoadAssetAsync<Material>(materialAddress);
			material = op.WaitForCompletion();
			if (!material) {
				log.Error().Message($"Couldn't load material from {materialAddress}");
				return;
			}

			//Add our custom render passes.
			GameObject holderGameObject = new GameObject("DetectLifeFeature");

			//parent our holderGameobject under the ragdollpart
			holderGameObject.transform.parent = instance.masterLevel.transform;
			customPassRenderObjects = holderGameObject.GetOrAddComponent<CustomPassRenderObjects>();
			/*
			if (customPassRenderObjects == null) {
				log.Error().Message($"Couldnt add customPassRenderObjects");
				return;
			}
			log.Info().Message($"Custom RenderPass added with material {material.name} and shader {material.shader.name}");

			for (int i = 0; i < 31; i++) {
				log.Info().Message($"layer {i} : {LayerMask.LayerToName(i)}");
			}
			*/
			log.Info().Message($"Custom RenderPass added with material {material.name} and shader {material.shader.name} on layermask {layerMaskName} - {LayerMask.NameToLayer(layerMaskName)}");
			customPassRenderObjects.color = color;
			customPassRenderObjects.settings.filterSettings.LayerMask = 1 << LayerMask.NameToLayer(layerMaskName); //ragdolls
			customPassRenderObjects.settings.filterSettings.RenderQueueType = RenderQueueType.Opaque;
			customPassRenderObjects.settings.Event = RenderPassEvent.AfterRenderingOpaques;
			customPassRenderObjects.settings.overrideMaterial = material;
			customPassRenderObjects.settings.overrideDepthState = true;
			customPassRenderObjects.settings.depthCompareFunction = CompareFunction.Greater;
			customPassRenderObjects.settings.enableWrite = true;
			customPassRenderObjects.CreateFeature();

		}
	}

}
