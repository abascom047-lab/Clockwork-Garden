using Assets.scripts.Clockwork.Parts.Interfaces;
using UnityEngine;

namespace Assets.scripts.Clockwork.Parts.Status {
	public class StickyStatusController : ManyThresholdStatusController {
		private IPartModifiableSpeed partSpeed;

		protected override void OnStart() {
			_type = StatusType.Sticky;
			_thresholds = new int[11] { 0, 10, 20, 30, 40, 50, 60, 70, 80, 90, 100 };
			_reverse = false;
			partSpeed = GetComponent<IPartModifiableSpeed>();

			base.OnStart();
		}

		protected override void ThresholdPassed(int index) {
			if (partSpeed != null) {
				float multiplier = 1f - index / (_thresholds.Length - 1f);
				if (multiplier == 1) {
					partSpeed.DeletePrimarySpeedModifier(_type);
					return;
				}

				partSpeed.UpsertPrimarySpeedModifier(_type, multiplier);
			}

			base.ThresholdPassed(index);
		}
	}
}
