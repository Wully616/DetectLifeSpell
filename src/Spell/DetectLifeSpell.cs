using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThunderRoad;
using UnityEngine;
using Wully.Helpers;
using Wully.Module;

namespace Wully.Spell {
	public class DetectLifeSpell : SpellCastCharge {
		private static BetterLogger log = BetterLogger.GetLogger(typeof(DetectLifeSpell));

		private DetectLifeModule detectLife;
		public override void Load(SpellCaster spellCaster) {
			base.Load(spellCaster);

			if (DetectLifeModule.local != null) {
				detectLife = DetectLifeModule.local;
			} else {
				log.Error().Message("Detect life spell unable to get detect life module.");
			}
		}

		public override void Fire(bool active) {
			base.Fire(active);
			if (detectLife != null) {
				if (active) {
					detectLife.customPassRenderObjects.EnableFeature();
				} else {
					detectLife.customPassRenderObjects.DisableFeature();
				}
			}
		}

		public override void Load(Imbue imbue) {}

		public override void UpdateImbue() {}
	}
}
