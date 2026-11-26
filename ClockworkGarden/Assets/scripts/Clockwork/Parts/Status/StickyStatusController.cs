using Assets.scripts.Clockwork.Parts.Interfaces;
using UnityEngine;

namespace Assets.scripts.Clockwork.Parts.Status {
	public class StickyStatusController : ManyThresholdStatusController {
		public TemperatureStatusController TemperatureControl;

		private IPartSpeed _speedInterface;

		protected override void OnStart() {
			_type = StatusType.Sticky;
			_thresholds = new int[11] { 0, 10, 20, 30, 40, 50, 60, 70, 80, 90, 100 };
			_reverse = false;
			_speedInterface = GetComponent<IPartSpeed>();

			base.OnStart();
		}

		protected override float GetDecaySpeed() {
			if (TemperatureControl == null)
				return base.GetDecaySpeed();

			float percentage = Mathf.Clamp(TemperatureControl.CurrentValue / TemperatureStatusController.TEMPERATURE_THRESHOLD, -1f, 1f);
			return DecaySpeed * (1f - percentage * 0.5f);
		}

		protected override void ThresholdPassed(int index) {
			if (_speedInterface != null) {
				float multiplier = 1f - index / (_thresholds.Length - 1f);
				if (multiplier == 1) {
					_speedInterface.DeletePrimarySpeedModifier(_type);
					return;
				}

				_speedInterface.UpsertPrimarySpeedModifier(_type, multiplier);
			}

			base.ThresholdPassed(index);
		}
	}
}
