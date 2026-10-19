using Assets.scripts.Clockwork.Parts;
using Assets.Scripts.Utils;
using UnityEngine;

namespace Assets.scripts.Clockwork.Denizens {
	public class ClockworkCandyDispenserController : MonoBehaviour {
		public ClockworkHingeController TopHingeControl;

		private Timer _openTimer;
		private Timer _closeTimer;

		void Awake() {
			_openTimer = new Timer(0.25f, true);
			_closeTimer = new Timer(0.25f);
		}

		void OnEnable() {
			TopHingeControl.Opened += OnTopHingeOpened;
			TopHingeControl.Closed += OnTopHingeClosed;

			_openTimer.Completed += OnOpenTimerCompleted;
			_closeTimer.Completed += OnCloseTimerCompleted;
		}

		void OnDisable() {
			TopHingeControl.Opened -= OnTopHingeOpened;
			TopHingeControl.Closed -= OnTopHingeClosed;

			_openTimer.Completed -= OnOpenTimerCompleted;
			_closeTimer.Completed -= OnCloseTimerCompleted;
		}

		void Update() {
			_openTimer.Update();
			_closeTimer.Update();
		}

		private void OnTopHingeOpened() {
			_closeTimer.Start();
		}

		private void OnTopHingeClosed() {
			_openTimer.Start();
		}

		private void OnOpenTimerCompleted() {
			_openTimer.Stop();
			TopHingeControl.Open();
		}

		private void OnCloseTimerCompleted() {
			_closeTimer.Stop();
			TopHingeControl.Close();
		}
	}
}
