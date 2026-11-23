using Assets.scripts.Clockwork.Parts.Interfaces;
using UnityEngine;

namespace Assets.scripts.Clockwork.Parts.Status {
	public class StickyStatusController : ManyThresholdStatusController {
		private IPartModifiableSpeed partSpeed;

		protected override void OnStart() {
			Type = StatusType.Sticky;
			Reverse = false;
			partSpeed = GetComponent<IPartModifiableSpeed>();

			base.OnStart();
		}

		protected override void ThresholdPassed(int index) {
			if (partSpeed != null) {
				float multiplier = 1f - index / (Thresholds.Length - 1f);
				if (multiplier == 1) {
					partSpeed.DeleteModifier(Type);
					return;
				}

				partSpeed.UpsertModifier(Type, multiplier);
			}

			base.ThresholdPassed(index);
		}
	}
}
