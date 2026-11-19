using Assets.scripts.Clockwork.Parts.Status;
using UnityEngine;

namespace Assets.scripts.Clockwork.Parts.Hinge {
	public class ClockworkPartHingeStatusController : MonoBehaviour {
		public ClockworkPartHingeController HingeControl;

		private ManyThresholdStatusController _stickyControl;

		void Awake() {
			_stickyControl = gameObject.AddComponent<ManyThresholdStatusController>();
			_stickyControl.Type = StatusType.Sticky;
			_stickyControl.Thresholds = new int[11] { 0, 10, 20, 30, 40, 50, 60, 70, 80, 90, 100 };
		}

		void OnEnable() {
			_stickyControl.ThresholdPassed += OnStickyThresholdPassed;
		}

		void OnDisable() {
			_stickyControl.ThresholdPassed -= OnStickyThresholdPassed;
		}

		private void OnStickyThresholdPassed(int threshold) {
			float speedIncrement = HingeControl.Speed / (_stickyControl.Thresholds.Length - 1);
			float newSpeed = HingeControl.Speed - speedIncrement * threshold;

			HingeControl.SetOpenSpeed(newSpeed);
		}
	}
}
