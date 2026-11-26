namespace Assets.scripts.Clockwork.Parts.Status {
	public class ManyThresholdStatusController : BaseStatusController {
		protected int[] _thresholds;
		protected bool _reverse = false;

		private int _currentThreshold = 0;

		protected override void OnUpdate() {
			if (_thresholds.Length == 0)
				return;

			if (CurrentValue > _thresholds[_currentThreshold]) {
				for (int i = _thresholds.Length - 1; i > _currentThreshold; i--) {
					if (IfPassedThreshold(_thresholds[i], _reverse)) {
						ThresholdPassed(i);
						_currentThreshold = i;

						return;
					}
				}
			} else if (CurrentValue < _thresholds[_currentThreshold]) {
				for (int i = _currentThreshold - 1; i >= 0; i--) {
					if (IfPassedThreshold(_thresholds[i], !_reverse)) {
						ThresholdPassed(i);
						_currentThreshold = i;

						return;
					}
				}
			}
		}

		protected virtual void ThresholdPassed(int index) { }

		private bool IfPassedThreshold(int threshold, bool reverse) {
			if (reverse && CurrentValue <= threshold)
				return true;

			if (!reverse && CurrentValue >= threshold)
				return true;

			return false;
		}
	}
}
