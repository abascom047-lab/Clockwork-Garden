using Assets.scripts.Clockwork.Parts.Status;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.scripts.Clockwork.Parts.Hinge {
	public class ClockworkPartHingeStatusController : MonoBehaviour {
		public ClockworkPartHingeController HingeControl;
		public BaseStatusController[] StatusControls;

		private Dictionary<StatusType, BaseStatusController> _statusDict;

		void Awake() {
			_statusDict = new Dictionary<StatusType, BaseStatusController>();
			foreach (BaseStatusController control in StatusControls)
				_statusDict.Add(control.Type, control);
		}

		void OnEnable() {
			if (_statusDict.TryGetValue(StatusType.Sticky, out BaseStatusController stickyControl)
				&& stickyControl is ManyThresholdStatusController stickyStatus) {

				stickyStatus.ThresholdPassed += OnStickyThresholdPassed;
			}
		}

		void OnDisable() {
			if (_statusDict.TryGetValue(StatusType.Sticky, out BaseStatusController stickyControl)
				&& stickyControl is ManyThresholdStatusController stickyStatus) {

				stickyStatus.ThresholdPassed -= OnStickyThresholdPassed;
			}
		}

		private void OnStickyThresholdPassed(int threshold) {
			if (_statusDict.TryGetValue(StatusType.Sticky, out BaseStatusController stickyControl)
				&& stickyControl is ManyThresholdStatusController stickyStatus) {

				float speedIncrement = HingeControl.Speed / (stickyStatus.Thresholds.Length - 1);
				float newSpeed = HingeControl.Speed - speedIncrement * threshold;

				HingeControl.SetOpenSpeed(newSpeed);
			}
		}
	}
}
