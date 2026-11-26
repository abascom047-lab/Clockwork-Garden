using Assets.scripts.Clockwork.Parts.Interfaces;

namespace Assets.scripts.Clockwork.Parts.Status {
	public class StickyStatusController : ManyThresholdStatusController {
		public TemperatureStatusController TemperatureControl;

		private IPartSpeed _speedInterface;
		private float _baseDecaySpeed;

		protected override void OnStart() {
			_type = StatusType.Sticky;
			_thresholds = new int[11] { 0, 10, 20, 30, 40, 50, 60, 70, 80, 90, 100 };
			_reverse = false;
			_speedInterface = GetComponent<IPartSpeed>();
			_baseDecaySpeed = DecaySpeed;

			base.OnStart();
		}

		protected override void OnUpdate() {
			if (TemperatureControl != null) {
				if (TemperatureControl.Hot) {
					DecaySpeed = _baseDecaySpeed * 0.5f;
				} else if (TemperatureControl.Cold) {
					DecaySpeed = _baseDecaySpeed * 1.5f;
				} else {
					DecaySpeed = _baseDecaySpeed;
				}
			}

			base.OnUpdate();
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
