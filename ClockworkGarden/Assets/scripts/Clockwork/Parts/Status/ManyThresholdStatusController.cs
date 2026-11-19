using System;
using UnityEditor.Rendering;
using UnityEngine;

namespace Assets.scripts.Clockwork.Parts.Status {
	public class ManyThresholdStatusController : BaseStatusController {
		public int[] Thresholds;
		public bool Reverse = false;

		public event Action<int> ThresholdPassed;

		private int _currentThreshold = 0;

		void Update() {
			if (Thresholds.Length == 0)
				return;

			if (CurrentValue > Thresholds[_currentThreshold]) {
				for (int i = _currentThreshold + 1; i < Thresholds.Length; i++) {
					if (IfPassedThreshold(Thresholds[i], Reverse)) {
						ThresholdPassed?.Invoke(i);
						_currentThreshold = i;

						return;
					}
				}
			} else if (CurrentValue < Thresholds[_currentThreshold]) {
				for (int i = _currentThreshold - 1; i >= 0; i--) {
					if (IfPassedThreshold(Thresholds[i], !Reverse)) {
						ThresholdPassed?.Invoke(i);
						_currentThreshold = i;

						return;
					}
				}
			}
		}

		private bool IfPassedThreshold(int threshold, bool reverse) {
			if (reverse && CurrentValue <= threshold)
				return true;

			if (!reverse && CurrentValue >= threshold)
				return true;

			return false;
		}
	}
}
