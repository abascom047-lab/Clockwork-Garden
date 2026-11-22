namespace Assets.scripts.Clockwork.Parts.Status {
	public class ManyThresholdStatusController : BaseStatusController {
		public int[] Thresholds;
		public bool Reverse = false;

		private int _currentThreshold = 0;

		protected override void OnUpdate() {
			if (Thresholds.Length == 0)
				return;

			if (CurrentValue > Thresholds[_currentThreshold]) {
				for (int i = Thresholds.Length - 1; i > _currentThreshold; i--) {
					if (IfPassedThreshold(Thresholds[i], Reverse)) {
						ThresholdPassed(i);
						_currentThreshold = i;

						return;
					}
				}
			} else if (CurrentValue < Thresholds[_currentThreshold]) {
				for (int i = _currentThreshold - 1; i >= 0; i--) {
					if (IfPassedThreshold(Thresholds[i], !Reverse)) {
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
