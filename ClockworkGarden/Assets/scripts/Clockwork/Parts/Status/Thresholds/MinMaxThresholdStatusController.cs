namespace Assets.scripts.Clockwork.Parts.Status {
	public enum ThresholdState {
		None,
		Under,
		Over
	}

	public class MinMaxThresholdStatusController : BaseStatusController {
		protected int _minThreshold = -10;
		protected int _maxThreshold = 10;

		private ThresholdState _state = ThresholdState.None;

		protected override void OnUpdate() {
			if (CurrentValue >= _maxThreshold && _state != ThresholdState.Over) {
				OverMaxThreshold();
				return;
			}

			if (CurrentValue <= _minThreshold && _state != ThresholdState.Under) {
				UnderMinThreshold();
				return;
			}

			if (CurrentValue > _minThreshold && CurrentValue < _maxThreshold && _state != ThresholdState.None) {
				BetweenThresholds();
				return;
			}

			base.OnUpdate();
		}

		protected virtual void BetweenThresholds() {
			_state = ThresholdState.None;
		}

		protected virtual void OverMaxThreshold() {
			_state = ThresholdState.Over;
		}

		protected virtual void UnderMinThreshold() {
			_state = ThresholdState.Under;
		}
	}
}
