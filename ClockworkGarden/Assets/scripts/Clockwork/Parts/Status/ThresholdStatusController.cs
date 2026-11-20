using System;

namespace Assets.scripts.Clockwork.Parts.Status {
	public enum ThresholdState {
		None,
		Under,
		Over
	}

	public class ThresholdStatusController : BaseStatusController {
		public int MinThreshold = -10;
		public int MaxThreshold = 10;

		public event Action<ThresholdState> ThresholdExceeded;
		public event Action ThresholdRestored;

		private ThresholdState _currentState = ThresholdState.None;

		protected override void OnUpdate() {
			if (CurrentValue >= MaxThreshold && _currentState != ThresholdState.Over) {
				_currentState = ThresholdState.Over;
				ThresholdExceeded?.Invoke(ThresholdState.Over);

				return;
			}

			if (CurrentValue <= MinThreshold && _currentState != ThresholdState.Under) {
				_currentState = ThresholdState.Under;
				ThresholdExceeded?.Invoke(ThresholdState.Under);

				return;
			}

			if (CurrentValue > MinThreshold && CurrentValue < MaxThreshold && _currentState != ThresholdState.None) {
				_currentState = ThresholdState.None;
				ThresholdRestored?.Invoke();

				return;
			}
		}
	}
}
